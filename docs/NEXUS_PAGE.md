# Nexus Mods page: Blackshield Company

Early beta: v0.1.0 passed every automated build check but hasn't been playtested in-game. The page says so up front. Exact upload-form values are in [NEXUS_FIELDS.md](NEXUS_FIELDS.md).

## Page settings

- **Game:** Skyrim Special Edition
- **Mod name:** Blackshield Company
- **Version:** 0.1.0 (early beta)
- **Category:** Guilds/Factions (alternative: Quests and Adventures)
- **Suggested tags:** Lore-Friendly, Quests, Factions, Gameplay, Radiant, Followers, ESL, No SKSE, Scripted
- **Requirements (Nexus "Requirements" tab):** none. The base game (SE/AE 1.5.97 or 1.6.x) is implied.
- **Permissions (suggested):** source on GitHub under MIT, so allow modification and conversions with credit. Don't allow re-uploads of the archive itself.
- **File to upload:** `BlackshieldCompany-0.1.0.zip` (FOMOD), main file, named "Blackshield Company".
- **Short summary:** see NEXUS_FIELDS.md.

## Description (paste into the BBCode editor)

```bbcode
[center][size=6][b]BLACKSHIELD COMPANY[/b][/size]
[size=3][i]A free company of sellswords. We keep our contracts. We hold the line.[/i][/size]

[size=3][color=#e0a030][b]EARLY BETA (v0.1.0)[/b]: this version passes every automated build check but hasn't been playtested in-game yet. Please report bugs in the Bugs tab or on GitHub, ideally with your Papyrus log.[/color][/size][/center]

[line]

[size=4][b]Summary[/b][/size]
A courier finds you with a sealed letter: the [b]Blackshield Company[/b] is hiring. Sign, take radiant contracts across Skyrim, earn gold and renown, and climb from [b]Recruit[/b] to [b]Commander[/b]. Every purse feeds the company treasury, and the officers decide what to do with it.

[size=4][b]Features[/b][/size]
[list]
[*][b]Courier recruitment[/b]. No tavern edits. The letter finds you once you're out in the world (level 2+, past Helgen, alternate-start friendly).
[*][b]Company Ledger[/b]. A lesser power and a ledger book open the company menu: take contracts, check renown and rank, manage the treasury.
[*][b]Three radiant contract types[/b] on vanilla locations inside Skyrim's holds:
[list]
[*][i]Clear the Camp[/i]: put down the leader of a bandit camp.
[*][i]Marked for Death[/i]: hunt a named target and their bodyguards near a ruin or cave.
[*][i]Hold the Line[/i]: defend a fort against three waves of raiders.
[/list]
[*][b]Five ranks[/b]: Recruit, Blade, Sergeant, Captain, Commander, earned with renown.
[*][b]Rank rewards[/b]: the Blackshield Blade, Blackshield Shield and Blackshield Plate, plus Commander's Presence.
[*][b]Company treasury[/b]. A share of every contract is banked. Captains and up can withdraw.
[*][b]Company sellsword[/b]. From Captain, call a Blackshield sellsword who uses the vanilla follower system.
[*]Pay scales with your level and rank. Commanders earn 25% more.
[/list]

[size=4][b]Requirements[/b][/size]
[list]
[*]Skyrim Special Edition or Anniversary Edition (1.5.97 or 1.6.x). Works on Steam under Proton on Linux.
[*]Nothing else. No SKSE, no SkyUI, no DLC required.
[/list]

[size=4][b]Installation[/b][/size]
Install with [b]Mod Organizer 2[/b] or [b]Vortex[/b] (FOMOD installer) and enable [b]BlackshieldCompany.esp[/b]. It's ESL-flagged, so it doesn't take a full load-order slot. Safe to add mid-playthrough. The letter arrives within a minute or so of loading a save that meets the conditions.

[size=4][b]Compatibility[/b][/size]
[list]
[*]No edits to vanilla records, cells or navmesh. Everything is new records.
[*]Uses the vanilla radiant system to pick locations, so other radiant quests won't send you to a location this mod has reserved, and the reverse.
[*]The sellsword is recruited through the vanilla follower system. Follower frameworks that hook the vanilla follower quest should work but haven't been tested yet.
[*]Alternate-start mods: supported (the letter only waits for the vanilla Helgen intro if it's running).
[*]Fully compatible with [b]Daedric Ledger: Bounties of Oblivion[/b] (same author). Both can run at the same time.
[*]Gear reuses vanilla meshes, so retextures of the ebony sword, ebony shield and steel plate cuirass apply to it too.
[/list]

[size=4][b]Uninstalling[/b][/size]
As with any scripted mod, don't remove it mid-playthrough. If you must, finish or abandon any active contract and dismiss the sellsword first.

[size=4][b]Known issues / early beta limits[/b][/size]
[list]
[*]Not playtested in-game yet. Found something? Please post it in the Bugs tab.
[*]No voice acting. The company speaks through letters, the ledger and message boxes.
[*]If every eligible location has been cleared or reserved, the ledger will say no contract of that kind is available. Try another type.
[*]Hold the Line raiders spawn at the fort's map marker and run at you. If you trigger it from an odd angle they may take a moment to arrive.
[*]Settings (treasury share, pay multiplier, letter level) are console globals for now. An MCM is planned.
[/list]

[size=4][b]Console tunables[/b][/size]
[code]set BSC_TreasuryShare to 0.2   (share of each purse banked, 0 to 1)
set BSC_PayMult to 1.0         (multiplier on all contract pay)
set BSC_IntroMinLevel to 2     (level before the courier letter arrives)[/code]

[size=4][b]Roadmap[/b][/size]
Recruiter NPC and company hall, escort contracts, an MCM, and more contract types. Also in the series: [i]Daedric Ledger: Bounties of Oblivion[/i]. Coming later: [i]Contract Hunter[/i], [i]Steel Oath[/i] and [i]Ember Smithing[/i].

[size=4][b]Source[/b][/size]
Open source (MIT): [url=https://github.com/synthalorian/blackshield-company]github.com/synthalorian/blackshield-company[/url]

[size=4][b]Credits[/b][/size]
[list]
[*][url=https://github.com/Mutagen-Modding/Mutagen]Mutagen[/url] by Noggog and contributors, which generates the plugin from code.
[*][url=https://github.com/russo-2025/papyrus-compiler]papyrus-compiler[/url] by russo-2025, the open-source Papyrus compiler.
[*][url=https://github.com/GandaG/fomod-schema]FOMOD schema[/url] by GandaG.
[*]Bethesda Game Studios for Skyrim.
[/list]
```

## Screenshots to capture in-game

Take these at 16:9, with the HUD hidden for scenery shots (`tm` in the console toggles menus).

1. **Hero shot.** Your character in Blackshield Plate with the Blackshield Shield and Blade, at sunset or by a fire. This goes on the mod tile.
2. **The courier letter.** The open Sealed Letter in the book menu.
3. **The sign prompt.** The "Sign the Blackshield Company contract?" box.
4. **Company Ledger menu.** The ledger at Captain or Commander showing renown, treasury and buttons.
5. **Contract menu.** The three contract choices.
6. **Journal.** An active contract in the journal with the objective naming a real location.
7. **Map.** The contract location on the world map with the quest marker.
8. **Clear the Camp.** A fight in a bandit camp.
9. **Marked for Death.** The named target (name visible on the crosshair) with bodyguards.
10. **Hold the Line.** A raider wave charging at a fort.
11. **Payout.** The "Contract fulfilled" box.
12. **Promotion.** A "Promoted to Captain/Commander" box.
13. **Sellsword.** You and the Blackshield Sellsword side by side.
14. **Gear close-ups.** Inventory 3D previews of the Blade, Shield and Plate with enchantment names.
