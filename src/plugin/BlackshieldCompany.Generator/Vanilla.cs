using Mutagen.Bethesda.Plugins;

namespace BlackshieldCompany.Generator;

/// <summary>
/// Vanilla Skyrim.esm records this mod links to. Only FormIDs are referenced;
/// no Bethesda data or assets are copied into the plugin or the repo.
/// </summary>
internal static class Vanilla
{
    public static readonly ModKey Skyrim = ModKey.FromFileName("Skyrim.esm");
    private static FormKey K(uint id) => new(Skyrim, id);

    // Actors / factions / races
    public static readonly FormKey Player = K(0x000014);              // PlayerRef
    public static readonly FormKey NordRace = K(0x013746);
    public static readonly FormKey DefaultRace = K(0x000019);
    public static readonly FormKey PotentialFollowerFaction = K(0x05C84D);
    public static readonly FormKey ClassWarrior1H = K(0x013176);       // CombatWarrior1H
    public static readonly FormKey CsHumanMelee = K(0x068849);         // csHumanMeleeLvl3
    public static readonly FormKey LCharBanditMeleeAny = K(0x03DECD);
    public static readonly FormKey LCharBanditBoss = K(0x03DF16);

    // Quests / globals
    public static readonly FormKey MQ101 = K(0x03372B);
    public static readonly FormKey DialogueFollower = K(0x0750BA);
    public static readonly FormKey PlayerFollowerCount = K(0x0BCC98);

    // Location data
    public static readonly FormKey LocTypeBanditCamp = K(0x0130DF);
    public static readonly FormKey LocTypeDungeon = K(0x0130DB);
    public static readonly FormKey LocTypeMilitaryFort = K(0x0130E7);
    public static readonly FormKey RefTypeBoss = K(0x0130F7);
    public static readonly FormKey RefTypeMapMarker = K(0x10F63C);
    public static readonly FormKey CompanionsRadiantAllowedHolds = K(0x0F9073); // keeps contracts inside Skyrim's holds

    // Items
    public static readonly FormKey Gold001 = K(0x00000F);
    public static readonly FormKey ArmorSteelPlateBoots = K(0x01395B);
    public static readonly FormKey ArmorSteelPlateGauntlets = K(0x01395D);
    public static readonly FormKey ArmorSteelPlateHelmet = K(0x01395E);

    // Keywords
    public static readonly FormKey ArmorHeavy = K(0x06BBD2);
    public static readonly FormKey ArmorShield = K(0x0965B2);
    public static readonly FormKey ArmorCuirass = K(0x06C0EC);
    public static readonly FormKey ArmorMaterialEbony = K(0x06BBD8);
    public static readonly FormKey ArmorMaterialSteelPlate = K(0x06BBE7);
    public static readonly FormKey VendorItemArmor = K(0x08F959);
    public static readonly FormKey VendorItemWeapon = K(0x08F958);
    public static readonly FormKey VendorItemBook = K(0x0937A2);
    public static readonly FormKey WeapTypeSword = K(0x01E711);
    public static readonly FormKey WeapMaterialEbony = K(0x01E71E);
    public static readonly FormKey MagicDisallowEnchanting = K(0x0C27BD);

    // Armor addons (worn meshes) and equip types
    public static readonly FormKey EbonyShieldAA = K(0x0E400D);
    public static readonly FormKey NordPlateCuirassAA = K(0x0E4AD7);  // Steel Plate cuirass
    public static readonly FormKey EquipShield = K(0x0141E8);
    public static readonly FormKey EquipRightHand = K(0x013F42);
    public static readonly FormKey EquipEitherHand = K(0x013F44);
    public static readonly FormKey EquipVoice = K(0x025BEE);

    // Impacts / materials / sounds / statics
    public static readonly FormKey WPNBashShieldHeavyImpactSet = K(0x0183FE);
    public static readonly FormKey WPNBashBladeImpactSet = K(0x0183FF);
    public static readonly FormKey WPNzBlade1HandImpactSet = K(0x013CAC);
    public static readonly FormKey MaterialShieldHeavy = K(0x016979);
    public static readonly FormKey MaterialBlade1HandEbony = K(0x0774C2);
    public static readonly FormKey WPNBlade1HandDrawSD = K(0x03C72E);
    public static readonly FormKey WPNBlade1HandSheatheSD = K(0x03C72F);
    public static readonly FormKey WPNSwingFail = K(0x03C730);
    public static readonly FormKey ITMNoteUp = K(0x0C7A54);
    public static readonly FormKey FirstPersonEbonySword = K(0x036BDF);
    public static readonly FormKey InvArtNote = K(0x097788);
    public static readonly FormKey InvArtBook = K(0x08ADAE);

    // Magic effects reused for enchantments
    public static readonly FormKey EnchFireDamageFFContact = K(0x04605A);
    public static readonly FormKey EnchFortifyBlockConstantSelf = K(0x07A0F3);
    public static readonly FormKey EnchFortifyHealthConstantSelf = K(0x0493AA);

    // Vanilla mesh paths (referenced by path only; the meshes ship with the game)
    public const string MeshEbonyShield = @"Weapons\Ebony\EbonyShield.nif";
    public const string MeshSteelPlateCuirassGround = @"Armor\NordPlate\Cuirass_GO.nif";
    public const string MeshEbonySword = @"Weapons\Ebony\EbonySword.nif";
    public const string MeshNote = @"Clutter\Books\Note01.nif";
    public const string MeshBook = @"Clutter\Books\Book02LowPoly02.nif";
}
