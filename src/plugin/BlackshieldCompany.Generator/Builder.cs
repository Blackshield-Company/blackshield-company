using Modkit.Mutagen;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using static BlackshieldCompany.Generator.Vanilla;

namespace BlackshieldCompany.Generator;

internal sealed class Builder
{
    private readonly SkyrimMod _mod;
    private readonly FormIdRegistry _ids;
    private static readonly SkyrimRelease R = SkyrimRelease.SkyrimSE;

    // Contract types (must match BSC_ContractScript / BSC_CompanyScript)
    private const int ContractClear = 0, ContractBounty = 1, ContractHold = 2;

    public Builder(SkyrimMod mod, FormIdRegistry ids)
    {
        _mod = mod;
        _ids = ids;
    }

    private FormKey Id(string editorId) => _ids.Get(editorId);
    private static FormLink<T> L<T>(FormKey k) where T : class, IMajorRecordGetter => new(k);

    // ----------------------------------------------------------------------------------------
    public void Build()
    {
        // FormIDs are reserved in a fixed order on first generation; afterwards formids.json rules.
        var questCompany = Id("BSC_CompanyQuest");
        var questClear = Id("BSC_ContractClear");
        var questBounty = Id("BSC_ContractBounty");
        var questHold = Id("BSC_ContractHold");

        // --- Globals ---------------------------------------------------------------------------
        var gJoined = Global("BSC_Joined", 0);
        var gRank = Global("BSC_Rank", 0);
        var gRenown = Global("BSC_Renown", 0);
        var gTreasury = Global("BSC_Treasury", 0);
        var gDone = Global("BSC_ContractsDone", 0);
        var gActive = Global("BSC_ContractActive", 0);
        var gShare = Global("BSC_TreasuryShare", 0.2f);
        var gMinLevel = Global("BSC_IntroMinLevel", 2);
        var gPayMult = Global("BSC_PayMult", 1.0f);

        // --- Magic: ledger power, commander ability, item enchantments ------------------------
        var mgefLedger = Add(new MagicEffect(Id("BSC_LedgerEffect"), R)
        {
            EditorID = "BSC_LedgerEffect",
            Name = "Company Ledger",
            Flags = MagicEffect.Flag.NoDuration | MagicEffect.Flag.NoMagnitude | MagicEffect.Flag.NoArea
                    | MagicEffect.Flag.Painless | MagicEffect.Flag.NoHitEvent | MagicEffect.Flag.HideInUI,
            BaseCost = 0,
            MagicSkill = ActorValue.None,
            ResistValue = ActorValue.None,
            SecondActorValue = ActorValue.None,
            Archetype = new MagicEffectArchetype { Type = MagicEffectArchetype.TypeEnum.Script, ActorValue = ActorValue.None },
            CastType = CastType.FireAndForget,
            TargetType = TargetType.Self,
            CastingSoundLevel = SoundLevel.Normal,
            Description = "Open the Blackshield Company ledger.",
            VirtualMachineAdapter = Vmad.Adapter(Vmad.Script("BSC_LedgerEffectScript", Vmad.Obj("Company", questCompany))),
        });

        var powerLedger = Add(new Spell(Id("BSC_PowerLedger"), R)
        {
            EditorID = "BSC_PowerLedger",
            Name = "Company Ledger",
            Description = "Open the Blackshield Company ledger: take contracts, check your renown and rank, and manage the company treasury.",
            Type = SpellType.LesserPower,
            CastType = CastType.FireAndForget,
            TargetType = TargetType.Self,
            Flags = SpellDataFlag.ManualCostCalc | SpellDataFlag.NoAbsorbOrReflect,
            BaseCost = 0,
            EquipmentType = L<IEquipTypeGetter>(EquipVoice).AsNullable(),
            Effects = { Effect(mgefLedger.FormKey, 0, 0) },
        });

        MagicEffect FortifyAbility(string edid, string name, ActorValue av) => Add(new MagicEffect(Id(edid), R)
        {
            EditorID = edid,
            Name = name,
            Flags = MagicEffect.Flag.Recover | MagicEffect.Flag.NoDuration | MagicEffect.Flag.NoArea,
            BaseCost = 0,
            MagicSkill = ActorValue.None,
            ResistValue = ActorValue.None,
            SecondActorValue = ActorValue.None,
            Archetype = new MagicEffectArchetype { Type = MagicEffectArchetype.TypeEnum.ValueModifier, ActorValue = av },
            CastType = CastType.ConstantEffect,
            TargetType = TargetType.Self,
            CastingSoundLevel = SoundLevel.Normal,
            Description = $"{av} is increased by <mag> points.",
        });
        var mgefCmdHealth = FortifyAbility("BSC_CommanderHealthEffect", "Commander's Presence", ActorValue.Health);
        var mgefCmdStamina = FortifyAbility("BSC_CommanderStaminaEffect", "Commander's Presence", ActorValue.Stamina);

        var abCommander = Add(new Spell(Id("BSC_AbCommander"), R)
        {
            EditorID = "BSC_AbCommander",
            Name = "Commander's Presence",
            Description = "Commander of the Blackshield Company: +25 Health and +25 Stamina.",
            Type = SpellType.Ability,
            CastType = CastType.ConstantEffect,
            TargetType = TargetType.Self,
            Flags = SpellDataFlag.ManualCostCalc,
            EquipmentType = L<IEquipTypeGetter>(EquipEitherHand).AsNullable(),
            Effects = { Effect(mgefCmdHealth.FormKey, 25, 0), Effect(mgefCmdStamina.FormKey, 25, 0) },
        });

        var enchBlade = Add(new ObjectEffect(Id("BSC_EnchBladeEmber"), R)
        {
            EditorID = "BSC_EnchBladeEmber",
            Name = "Ember Edge",
            CastType = CastType.FireAndForget,
            TargetType = TargetType.Touch,
            EnchantType = ObjectEffect.EnchantTypeEnum.Enchantment,
            EnchantmentAmount = 0,
            EnchantmentCost = 30, // ~50 hits per full charge
            Flags = ObjectEffect.Flag.NoAutoCalc,
            Effects = { Effect(EnchFireDamageFFContact, 10, 0) },
        });
        var enchShield = Add(new ObjectEffect(Id("BSC_EnchShieldBlock"), R)
        {
            EditorID = "BSC_EnchShieldBlock",
            Name = "Shieldwall",
            CastType = CastType.ConstantEffect,
            TargetType = TargetType.Self,
            EnchantType = ObjectEffect.EnchantTypeEnum.Enchantment,
            Flags = ObjectEffect.Flag.NoAutoCalc,
            Effects = { Effect(EnchFortifyBlockConstantSelf, 15, 0) },
        });
        var enchPlate = Add(new ObjectEffect(Id("BSC_EnchPlateHealth"), R)
        {
            EditorID = "BSC_EnchPlateHealth",
            Name = "Company Vigor",
            CastType = CastType.ConstantEffect,
            TargetType = TargetType.Self,
            EnchantType = ObjectEffect.EnchantTypeEnum.Enchantment,
            Flags = ObjectEffect.Flag.NoAutoCalc,
            Effects = { Effect(EnchFortifyHealthConstantSelf, 40, 0) },
        });

        // --- Reward gear (vanilla meshes, new names and stats) ---------------------------------
        var blade = Add(new Weapon(Id("BSC_BlackshieldBlade"), R)
        {
            EditorID = "BSC_BlackshieldBlade",
            Name = "Blackshield Blade",
            ObjectBounds = Bounds(-2, -13, -2, 4, 70, 2),
            Model = new Model { File = MeshEbonySword },
            ObjectEffect = L<IObjectEffectGetter>(enchBlade.FormKey).AsNullable(),
            EnchantmentAmount = 1500,
            EquipmentType = L<IEquipTypeGetter>(EquipRightHand).AsNullable(),
            BlockBashImpact = L<IImpactDataSetGetter>(WPNBashBladeImpactSet).AsNullable(),
            AlternateBlockMaterial = L<IMaterialTypeGetter>(MaterialBlade1HandEbony).AsNullable(),
            Keywords = new() { L<IKeywordGetter>(WeapTypeSword), L<IKeywordGetter>(WeapMaterialEbony), L<IKeywordGetter>(VendorItemWeapon), L<IKeywordGetter>(MagicDisallowEnchanting) },
            Description = "",
            ImpactDataSet = L<IImpactDataSetGetter>(WPNzBlade1HandImpactSet).AsNullable(),
            FirstPersonModel = L<IStaticGetter>(FirstPersonEbonySword).AsNullable(),
            AttackFailSound = L<ISoundDescriptorGetter>(WPNSwingFail).AsNullable(),
            EquipSound = L<ISoundDescriptorGetter>(WPNBlade1HandDrawSD).AsNullable(),
            UnequipSound = L<ISoundDescriptorGetter>(WPNBlade1HandSheatheSD).AsNullable(),
            BasicStats = new WeaponBasicStats { Value = 600, Weight = 13, Damage = 10 },
            Data = new WeaponData
            {
                AnimationType = WeaponAnimationType.OneHandSword,
                Speed = 1, Reach = 1, NumProjectiles = 1, AnimationAttackMult = 1,
                Unknown2 = 1061997773,
                RumbleLeftMotorStrength = 0.5f, RumbleRightMotorStrength = 1, RumbleDuration = 0.33f,
                Skill = Skill.OneHanded,
                Resist = ActorValue.None,
                Stagger = 0.75f,
            },
            Critical = new CriticalData { Damage = 5, PercentMult = 1, Flags = CriticalData.Flag.OnDeath },
            DetectionSoundLevel = SoundLevel.Normal,
        });

        var shield = Add(new Armor(Id("BSC_BlackshieldShield"), R)
        {
            EditorID = "BSC_BlackshieldShield",
            Name = "Blackshield Shield",
            ObjectBounds = Bounds(-23, -35, -9, 23, 27, 2),
            ObjectEffect = L<IObjectEffectGetter>(enchShield.FormKey).AsNullable(),
            WorldModel = new GenderedItem<ArmorModel?>(new ArmorModel { Model = new Model { File = MeshEbonyShield } }, null),
            BodyTemplate = new BodyTemplate { FirstPersonFlags = BipedObjectFlag.Shield, ArmorType = ArmorType.HeavyArmor, ActsLike44 = true },
            EquipmentType = L<IEquipTypeGetter>(EquipShield).AsNullable(),
            BashImpactDataSet = L<IImpactDataSetGetter>(WPNBashShieldHeavyImpactSet).AsNullable(),
            AlternateBlockMaterial = L<IMaterialTypeGetter>(MaterialShieldHeavy).AsNullable(),
            Race = L<IRaceGetter>(DefaultRace).AsNullable(),
            Keywords = new() { L<IKeywordGetter>(ArmorHeavy), L<IKeywordGetter>(ArmorMaterialEbony), L<IKeywordGetter>(VendorItemArmor), L<IKeywordGetter>(ArmorShield), L<IKeywordGetter>(MagicDisallowEnchanting) },
            Description = "",
            Armature = { L<IArmorAddonGetter>(EbonyShieldAA) },
            Value = 500,
            Weight = 12,
            ArmorRating = 28,
            MajorFlags = Armor.MajorFlag.Shield,
        });

        var plate = Add(new Armor(Id("BSC_BlackshieldPlate"), R)
        {
            EditorID = "BSC_BlackshieldPlate",
            Name = "Blackshield Plate",
            ObjectBounds = Bounds(-23, -22, -1, 23, 22, 18),
            ObjectEffect = L<IObjectEffectGetter>(enchPlate.FormKey).AsNullable(),
            WorldModel = new GenderedItem<ArmorModel?>(new ArmorModel { Model = new Model { File = MeshSteelPlateCuirassGround } }, null),
            BodyTemplate = new BodyTemplate { FirstPersonFlags = BipedObjectFlag.Body, ArmorType = ArmorType.HeavyArmor, ActsLike44 = true },
            Race = L<IRaceGetter>(DefaultRace).AsNullable(),
            Keywords = new() { L<IKeywordGetter>(ArmorHeavy), L<IKeywordGetter>(ArmorMaterialSteelPlate), L<IKeywordGetter>(ArmorCuirass), L<IKeywordGetter>(VendorItemArmor), L<IKeywordGetter>(MagicDisallowEnchanting) },
            Description = "",
            Armature = { L<IArmorAddonGetter>(NordPlateCuirassAA) },
            Value = 900,
            Weight = 35,
            ArmorRating = 42,
        });

        // --- Letter and ledger books -----------------------------------------------------------
        var letter = Add(new Book(Id("BSC_Letter"), R)
        {
            EditorID = "BSC_Letter",
            Name = Texts.LetterName,
            ObjectBounds = Bounds(-8, -5, 0, 8, 5, 0),
            Model = new Model { File = MeshNote },
            BookText = Texts.Letter,
            PickUpSound = L<ISoundDescriptorGetter>(ITMNoteUp).AsNullable(),
            Keywords = new() { L<IKeywordGetter>(VendorItemBook) },
            Type = Book.BookType.NoteOrScroll,
            Teaches = new BookTeachesNothing { RawContent = 0xFFFFFFFF },
            Value = 0,
            Weight = 0,
            InventoryArt = L<IStaticGetter>(InvArtNote).AsNullable(),
            Description = "",
            VirtualMachineAdapter = Vmad.Adapter(Vmad.Script("BSC_LetterScript", Vmad.Obj("Company", questCompany))),
        });

        var ledger = Add(new Book(Id("BSC_Ledger"), R)
        {
            EditorID = "BSC_Ledger",
            Name = Texts.LedgerName,
            ObjectBounds = Bounds(-11, -15, -3, 11, 14, 3),
            Model = new Model { File = MeshBook },
            BookText = Texts.Ledger,
            Keywords = new() { L<IKeywordGetter>(VendorItemBook) },
            Type = Book.BookType.BookOrTome,
            Teaches = new BookTeachesNothing { RawContent = 0xFFFFFFFF },
            Value = 0,
            Weight = 1,
            InventoryArt = L<IStaticGetter>(InvArtBook).AsNullable(),
            Description = "",
            VirtualMachineAdapter = Vmad.Adapter(Vmad.Script("BSC_LedgerBookScript", Vmad.Obj("Company", questCompany))),
        });

        // --- Actors -----------------------------------------------------------------------------
        var outfit = Add(new Outfit(Id("BSC_SellswordOutfit"), R)
        {
            EditorID = "BSC_SellswordOutfit",
            Items = new()
            {
                L<IOutfitTargetGetter>(plate.FormKey),
                L<IOutfitTargetGetter>(ArmorSteelPlateBoots),
                L<IOutfitTargetGetter>(ArmorSteelPlateGauntlets),
                L<IOutfitTargetGetter>(ArmorSteelPlateHelmet),
            },
        });

        var sellsword = Add(TemplatedNpc("BSC_Sellsword", "Blackshield Sellsword", "Sellsword", LCharBanditMeleeAny,
            NpcConfiguration.TemplateFlag.Traits | NpcConfiguration.TemplateFlag.Stats | NpcConfiguration.TemplateFlag.SpellList
            | NpcConfiguration.TemplateFlag.AttackData | NpcConfiguration.TemplateFlag.ModelAnimation));
        sellsword.Factions.Add(new RankPlacement { Faction = L<IFactionGetter>(PotentialFollowerFaction), Rank = 0 });
        sellsword.AIData = new AIData
        {
            Aggression = Aggression.Unaggressive,
            Confidence = Confidence.Brave,
            EnergyLevel = 50,
            Responsibility = Responsibility.NoCrime,
            Mood = Mood.Neutral,
            Assistance = Assistance.HelpsFriendsAndAllies,
        };
        sellsword.DefaultOutfit = L<IOutfitGetter>(outfit.FormKey).AsNullable();
        sellsword.Items = new()
        {
            new ContainerEntry { Item = new ContainerItem { Item = L<IItemGetter>(blade.FormKey), Count = 1 } },
            new ContainerEntry { Item = new ContainerItem { Item = L<IItemGetter>(shield.FormKey), Count = 1 } },
        };

        var allButName = NpcConfiguration.TemplateFlag.Traits | NpcConfiguration.TemplateFlag.Stats | NpcConfiguration.TemplateFlag.Factions
            | NpcConfiguration.TemplateFlag.SpellList | NpcConfiguration.TemplateFlag.AIData | NpcConfiguration.TemplateFlag.AIPackages
            | NpcConfiguration.TemplateFlag.ModelAnimation | NpcConfiguration.TemplateFlag.Inventory | NpcConfiguration.TemplateFlag.Script
            | NpcConfiguration.TemplateFlag.DefPackList | NpcConfiguration.TemplateFlag.AttackData | NpcConfiguration.TemplateFlag.Keywords;
        var targets = new (string edid, string name, string shortName)[]
        {
            ("BSC_Target01", "Hrolf Ashjaw", "Ashjaw"),
            ("BSC_Target02", "Grey-Hand", "Grey-Hand"),
            ("BSC_Target03", "Oathbreaker Vane", "Vane"),
            ("BSC_Target04", "Two-Coins Marrow", "Two-Coins"),
            ("BSC_Target05", "The Red Ferryman", "Ferryman"),
        };
        var targetNpcs = targets.Select(t => Add(TemplatedNpc(t.edid, t.name, t.shortName, LCharBanditBoss, allButName))).ToList();

        var lvlTargets = Add(new LeveledNpc(Id("BSC_LCharMarkedTarget"), R)
        {
            EditorID = "BSC_LCharMarkedTarget",
            Flags = LeveledNpc.Flag.CalculateFromAllLevelsLessThanOrEqualPlayer,
            Entries = targetNpcs.Select(n => new LeveledNpcEntry
            {
                Data = new LeveledNpcEntryData { Level = 1, Count = 1, Reference = L<INpcSpawnGetter>(n.FormKey) },
            }).ToExtendedList(),
        });

        // --- Messages --------------------------------------------------------------------------
        var msgCourier = Msg("BSC_MsgCourier", Texts.Courier);
        var msgJoin = Msg("BSC_MsgJoinOffer", Texts.JoinOffer, Btn("Sign"), Btn("Not yet"));
        var msgWelcome = Msg("BSC_MsgWelcome", Texts.Welcome);
        var msgNotMember = Msg("BSC_MsgNotMember", Texts.NotMember);
        var ledgerMenus = Enumerable.Range(0, 5).Select(rank => Msg($"BSC_MsgLedger{rank}", Texts.LedgerMenu(rank),
            Btn("Take a contract", Cond.Global(gActive, CompareOperator.EqualTo, 0)),
            Btn("Current contract", Cond.Global(gActive, CompareOperator.EqualTo, 1)),
            Btn("Company treasury"),
            Btn("Summon a company sellsword", Cond.Global(gRank, CompareOperator.GreaterThanOrEqualTo, 3)),
            Btn("Close"))).ToList();
        var msgContractMenu = Msg("BSC_MsgContractMenu", Texts.ContractMenu,
            Btn("Clear a bandit camp"), Btn("Collect a bounty"), Btn("Hold a fort"), Btn("Back"));
        var msgNoContract = Msg("BSC_MsgNoContract", Texts.NoContract);
        var msgStatus = Msg("BSC_MsgContractStatus", Texts.ContractStatus, Btn("Back to work"), Btn("Abandon the contract (-10 renown)"));
        var msgPaid = Msg("BSC_MsgContractPaid", Texts.ContractPaid);
        var msgTreasury = Msg("BSC_MsgTreasury", Texts.Treasury, Btn("Withdraw 100 gold"), Btn("Withdraw 500 gold"), Btn("Withdraw everything"), Btn("Leave it"));
        var msgTreasuryLocked = Msg("BSC_MsgTreasuryLocked", Texts.TreasuryLocked);
        var promotions = Enumerable.Range(1, 4).Select(rank => Msg($"BSC_MsgPromoted{rank}", Texts.Promotions[rank])).ToList();

        // --- Quests ----------------------------------------------------------------------------
        var company = Add(new Quest(questCompany, R)
        {
            EditorID = "BSC_CompanyQuest",
            Name = "Blackshield Company",
            Flags = Quest.Flag.StartGameEnabled,
            Priority = 50,
            Type = Quest.TypeEnum.None,
            NextAliasID = 1,
        });
        company.Aliases.Add(new QuestAlias
        {
            ID = 0,
            Type = QuestAlias.TypeEnum.Reference,
            Name = "Sellsword",
            Flags = QuestAlias.Flag.Optional | QuestAlias.Flag.Protected,
        });
        company.VirtualMachineAdapter = Vmad.QuestAdapter(Vmad.Script("BSC_CompanyScript",
            Vmad.Obj("BSC_Joined", gJoined), Vmad.Obj("BSC_Rank", gRank), Vmad.Obj("BSC_Renown", gRenown),
            Vmad.Obj("BSC_Treasury", gTreasury), Vmad.Obj("BSC_ContractsDone", gDone), Vmad.Obj("BSC_ContractActive", gActive),
            Vmad.Obj("BSC_TreasuryShare", gShare), Vmad.Obj("BSC_IntroMinLevel", gMinLevel), Vmad.Obj("BSC_PayMult", gPayMult),
            Vmad.Obj("PlayerFollowerCount", PlayerFollowerCount),
            Vmad.Obj("MQ101", MQ101), Vmad.Obj("DialogueFollower", DialogueFollower),
            Vmad.ObjList("Contracts", questClear, questBounty, questHold),
            Vmad.Obj("BSC_Letter", letter.FormKey), Vmad.Obj("BSC_Ledger", ledger.FormKey),
            Vmad.Obj("BSC_PowerLedger", powerLedger.FormKey), Vmad.Obj("BSC_AbCommander", abCommander.FormKey),
            Vmad.Obj("BSC_BlackshieldBlade", blade.FormKey), Vmad.Obj("BSC_BlackshieldShield", shield.FormKey),
            Vmad.Obj("BSC_BlackshieldPlate", plate.FormKey), Vmad.Obj("BSC_Sellsword", sellsword.FormKey),
            Vmad.Obj("Gold001", Gold001),
            Vmad.Alias("Sellsword", questCompany, 0),
            Vmad.Obj("BSC_MsgCourier", msgCourier), Vmad.Obj("BSC_MsgJoinOffer", msgJoin), Vmad.Obj("BSC_MsgWelcome", msgWelcome),
            Vmad.Obj("BSC_MsgNotMember", msgNotMember),
            Vmad.ObjList("LedgerMenus", ledgerMenus.ToArray()),
            Vmad.Obj("BSC_MsgContractMenu", msgContractMenu), Vmad.Obj("BSC_MsgNoContract", msgNoContract),
            Vmad.Obj("BSC_MsgContractStatus", msgStatus), Vmad.Obj("BSC_MsgContractPaid", msgPaid),
            Vmad.Obj("BSC_MsgTreasury", msgTreasury), Vmad.Obj("BSC_MsgTreasuryLocked", msgTreasuryLocked),
            // index 0 is unused (no promotion to Recruit); repeat Blade's message to keep the array dense
            Vmad.ObjList("PromotionMessages", new[] { promotions[0] }.Concat(promotions).ToArray())));

        // Contract: clear a bandit camp ----------------------------------------------------------
        var clear = ContractQuest(questClear, "BSC_ContractClear", "Blackshield Contract: Clear the Camp",
            "The Blackshield Company ledger lists a contract on the bandits holed up at <Alias=ContractLocation>. Their leader dies, the contract pays.",
            ContractClear);
        clear.Aliases.Add(LocationAlias(0, LocTypeBanditCamp, requireBoss: true, requireMapMarker: false, allowCleared: false));
        clear.Aliases.Add(new QuestAlias
        {
            ID = 1, Type = QuestAlias.TypeEnum.Reference, Name = "MapMarker",
            Flags = QuestAlias.Flag.Optional,
            Location = new LocationAliasReference { AliasID = 0, RefType = L<ILocationReferenceTypeGetter>(RefTypeMapMarker).AsNullable() },
        });
        clear.Aliases.Add(new QuestAlias
        {
            ID = 2, Type = QuestAlias.TypeEnum.Reference, Name = "Target",
            Flags = QuestAlias.Flag.ReservesLocationOrReference | QuestAlias.Flag.StoresText,
            Location = new LocationAliasReference { AliasID = 0, RefType = L<ILocationReferenceTypeGetter>(RefTypeBoss).AsNullable() },
        });
        clear.NextAliasID = 3;
        clear.Objectives.Add(Objective(10, "Kill the bandit leader at <Alias=ContractLocation>", 2));
        clear.VirtualMachineAdapter = ContractScript(questClear, questCompany, ContractClear, hasRaiders: false);

        // Contract: bounty on a marked target -------------------------------------------------
        var bounty = ContractQuest(questBounty, "BSC_ContractBounty", "Blackshield Contract: Marked for Death",
            "A client has posted a bounty through the Blackshield Company on <Alias=Target>, who is hiding out near <Alias=ContractLocation>.",
            ContractBounty);
        bounty.Aliases.Add(LocationAlias(0, LocTypeDungeon, requireBoss: false, requireMapMarker: true, allowCleared: true));
        bounty.Aliases.Add(new QuestAlias
        {
            ID = 1, Type = QuestAlias.TypeEnum.Reference, Name = "MapMarker",
            Location = new LocationAliasReference { AliasID = 0, RefType = L<ILocationReferenceTypeGetter>(RefTypeMapMarker).AsNullable() },
        });
        bounty.Aliases.Add(new QuestAlias
        {
            ID = 2, Type = QuestAlias.TypeEnum.Reference, Name = "Target",
            Flags = QuestAlias.Flag.StoresText,
            CreateReferenceToObject = new CreateReferenceToObject
            {
                Object = L<ISkyrimMajorRecordGetter>(lvlTargets.FormKey), AliasID = 1,
                Create = CreateReferenceToObject.CreateEnum.At, Level = Level.Hard,
            },
        });
        for (int i = 0; i < 2; i++)
        {
            bounty.Aliases.Add(new QuestAlias
            {
                ID = (uint)(3 + i), Type = QuestAlias.TypeEnum.Reference, Name = $"Bodyguard{i + 1}",
                Flags = QuestAlias.Flag.Optional,
                CreateReferenceToObject = new CreateReferenceToObject
                {
                    Object = L<ISkyrimMajorRecordGetter>(LCharBanditMeleeAny), AliasID = 1,
                    Create = CreateReferenceToObject.CreateEnum.At, Level = Level.Medium,
                },
            });
        }
        bounty.NextAliasID = 5;
        bounty.Objectives.Add(Objective(10, "Kill <Alias=Target> near <Alias=ContractLocation>", 2));
        bounty.VirtualMachineAdapter = ContractScript(questBounty, questCompany, ContractBounty, hasRaiders: false);

        // Contract: hold a fort -----------------------------------------------------------------
        var hold = ContractQuest(questHold, "BSC_ContractHold", "Blackshield Contract: Hold the Line",
            "Raiders are moving on <Alias=ContractLocation>. The company wants the ground held until they break.",
            ContractHold);
        hold.Aliases.Add(LocationAlias(0, LocTypeMilitaryFort, requireBoss: false, requireMapMarker: true, allowCleared: true));
        hold.Aliases.Add(new QuestAlias
        {
            ID = 1, Type = QuestAlias.TypeEnum.Reference, Name = "MapMarker",
            Location = new LocationAliasReference { AliasID = 0, RefType = L<ILocationReferenceTypeGetter>(RefTypeMapMarker).AsNullable() },
        });
        hold.NextAliasID = 2;
        hold.Objectives.Add(Objective(10, "Travel to <Alias=ContractLocation>", 1));
        hold.Objectives.Add(Objective(20, "Hold <Alias=ContractLocation> until the raiders break", 1));
        hold.VirtualMachineAdapter = ContractScript(questHold, questCompany, ContractHold, hasRaiders: true);
    }

    // ----------------------------------------------------------------------------------------
    // Record helpers
    // ----------------------------------------------------------------------------------------

    private T Add<T>(T record) where T : SkyrimMajorRecord
    {
        _mod.GetTopLevelGroup<T>().Add(record);
        return record;
    }

    private FormKey Global(string edid, float value)
    {
        var g = new GlobalFloat(Id(edid), R) { EditorID = edid, Data = value };
        _mod.Globals.Add(g);
        return g.FormKey;
    }

    private static Effect Effect(FormKey mgef, float magnitude, int duration) => new()
    {
        BaseEffect = new FormLinkNullable<IMagicEffectGetter>(mgef),
        Data = new EffectData { Magnitude = magnitude, Area = 0, Duration = duration },
    };

    private static ObjectBounds Bounds(short x1, short y1, short z1, short x2, short y2, short z2)
        => new() { First = new P3Int16(x1, y1, z1), Second = new P3Int16(x2, y2, z2) };

    private static MessageButton Btn(string text, params Condition[] conditions)
    {
        var b = new MessageButton { Text = text };
        b.Conditions.AddRange(conditions);
        return b;
    }

    private FormKey Msg(string edid, string text, params MessageButton[] buttons)
    {
        var m = new Message(Id(edid), R)
        {
            EditorID = edid,
            Description = text,
            Flags = Message.Flag.MessageBox,
            INAM = new byte[4],
        };
        m.MenuButtons.AddRange(buttons);
        _mod.Messages.Add(m);
        return m.FormKey;
    }

    private Npc TemplatedNpc(string edid, string name, string shortName, FormKey template, NpcConfiguration.TemplateFlag flags) => new(Id(edid), R)
    {
        EditorID = edid,
        Name = name,
        ShortName = shortName,
        ObjectBounds = Bounds(-22, -14, 0, 22, 14, 128),
        Template = new FormLinkNullable<INpcSpawnGetter>(template),
        Race = L<IRaceGetter>(NordRace),
        Class = L<IClassGetter>(ClassWarrior1H),
        CombatStyle = L<ICombatStyleGetter>(CsHumanMelee).AsNullable(),
        Configuration = new NpcConfiguration
        {
            Flags = NpcConfiguration.Flag.AutoCalcStats,
            TemplateFlags = flags,
            Level = new PcLevelMult { LevelMult = 1 },
            CalcMinLevel = 1,
            CalcMaxLevel = 0,
            SpeedMultiplier = 100,
            DispositionBase = 35,
        },
        AIData = new AIData { Aggression = Aggression.Aggressive, Confidence = Confidence.Average, Assistance = Assistance.HelpsAllies, EnergyLevel = 50 },
        PlayerSkills = new PlayerSkills(),
        Height = 1,
        Weight = 50,
    };

    private Quest ContractQuest(FormKey fk, string edid, string name, string startLog, int type)
    {
        var q = Add(new Quest(fk, R)
        {
            EditorID = edid,
            Name = name,
            Priority = 60,
            Type = Quest.TypeEnum.SideQuest,
        });
        q.Stages.Add(new QuestStage { Index = 0, Flags = QuestStage.Flag.StartUpStage });
        q.Stages.Add(new QuestStage { Index = 10, LogEntries = { new QuestLogEntry { Entry = startLog } } });
        q.Stages.Add(new QuestStage
        {
            Index = 100,
            LogEntries = { new QuestLogEntry { Flags = QuestLogEntry.Flag.CompleteQuest, Entry = "I fulfilled the contract and the Blackshield Company paid out." } },
        });
        q.Stages.Add(new QuestStage
        {
            Index = 200,
            Flags = QuestStage.Flag.ShutDownStage,
            LogEntries = { new QuestLogEntry { Flags = QuestLogEntry.Flag.FailQuest, Entry = "I abandoned the contract." } },
        });
        return q;
    }

    private static QuestAlias LocationAlias(uint id, FormKey locType, bool requireBoss, bool requireMapMarker, bool allowCleared)
    {
        var a = new QuestAlias
        {
            ID = id,
            Type = QuestAlias.TypeEnum.Location,
            Name = "ContractLocation",
            Flags = QuestAlias.Flag.ReservesLocationOrReference | QuestAlias.Flag.StoresText
                    | (allowCleared ? QuestAlias.Flag.AllowCleared : 0),
        };
        a.Conditions.Add(Cond.InCurrentLocFormList(CompanionsRadiantAllowedHolds));
        a.Conditions.Add(Cond.LocationHasKeyword(locType));
        if (requireBoss) a.Conditions.Add(Cond.LocationHasRefType(RefTypeBoss));
        if (requireMapMarker) a.Conditions.Add(Cond.LocationHasRefType(RefTypeMapMarker));
        return a;
    }

    private static QuestObjective Objective(ushort index, string text, int targetAlias) => new()
    {
        Index = index,
        DisplayText = text,
        Targets = { new QuestObjectiveTarget { AliasID = targetAlias } },
    };

    private static QuestAdapter ContractScript(FormKey quest, FormKey company, int type, bool hasRaiders)
    {
        var props = new List<ScriptProperty>
        {
            Vmad.Obj("Company", company),
            Vmad.Int("ContractType", type),
            Vmad.Alias("ContractLocation", quest, 0),
            Vmad.Alias("MapMarker", quest, 1),
        };
        if (type != ContractHold) props.Add(Vmad.Alias("Target", quest, 2));
        if (hasRaiders) props.Add(Vmad.Obj("RaiderList", LCharBanditMeleeAny));
        return Vmad.QuestAdapter(Vmad.Script("BSC_ContractScript", props.ToArray()));
    }
}
