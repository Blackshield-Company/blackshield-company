# In-game test checklist (v0.1.0, early beta)

v0.1.0 ships as an early beta. It passed every automated build check but hasn't been played in-game yet. This checklist is for players and testers who want to confirm everything works, and for the next round of fixes.

Open the console with `~`. Commands that take a quest or global accept the EditorID directly. `player.additem` needs a full FormID, which depends on your load order, so look it up first:

```
help "Sealed Letter" 4          -> e.g. BOOK: (FE012818) 'Sealed Letter (Blackshield Company)'
```

ESL-flagged plugins show up as `FE` + a 3-digit slot (`xxx`) + the record's own 3 digits. The record IDs are:

| Record | Local ID | Full FormID |
|---|---|---|
| Sealed Letter | 818 | `FExxx818` |
| Company Ledger (book) | 819 | `FExxx819` |
| Company Ledger (power) | 80E | `FExxx80E` |
| Blackshield Blade | 815 | `FExxx815` |
| Blackshield Shield | 816 | `FExxx816` |
| Blackshield Plate | 817 | `FExxx817` |
| Blackshield Sellsword (NPC) | 81B | `FExxx81B` |

**Turn on Papyrus logging before testing.** Add this to `Skyrim.ini` (under Proton: `.../compatdata/489830/pfx/drive_c/users/steamuser/Documents/My Games/Skyrim Special Edition/Skyrim.ini`):

```ini
[Papyrus]
bEnableLogging=1
bEnableTrace=1
bLoadDebugInformation=1
```

The log is written to `.../My Games/Skyrim Special Edition/Logs/Script/Papyrus.0.log`. After each session, run `grep -n "BSC_" Papyrus.0.log` and report anything it finds.

---

## 0. Plugin loads
- [ ] `BlackshieldCompany.esp` is enabled and the game reaches the main menu.
- [ ] Load a save. `show BSC_Joined` prints `0.00`.
- [ ] `sqv BSC_CompanyQuest` shows the quest as running.

## 1. Recruitment letter
- [ ] Use a character that's level 2+ and past Helgen. Within about 1 minute you get the courier message and a **Sealed Letter**.
  - Faster: `set BSC_IntroMinLevel to 1`.
  - Bypass: `player.additem FExxx818 1`.
- [ ] Read the letter and close the inventory. The *Sign / Not yet* box appears.
- [ ] *Not yet* closes it. Reading again offers it again.
- [ ] *Sign*: the welcome box appears, you get +100 gold, the **Blackshield Company Ledger** book, and the **Company Ledger** power (Magic > Powers). `show BSC_Joined` is `1.00`.

## 2. Ledger
- [ ] Cast the power. The ledger shows **Rank: Recruit**, renown 0 (next rank at 100), treasury 0, contracts 0.
- [ ] Buttons: *Take a contract*, *Company treasury*, *Close*. *Current contract* and *Summon a company sellsword* are hidden.
- [ ] Read the ledger book. The pages show the articles, then the ledger menu opens after you close the book.
- [ ] *Company treasury* as Recruit shows the "Only a Captain…" box.

## 3. Contract: Clear the Camp
- [ ] *Take a contract > Clear a bandit camp*. The journal gets **Blackshield Contract: Clear the Camp**, the objective names a real bandit camp, and the camp shows up on the map.
- [ ] The ledger now shows *Current contract* instead of *Take a contract*.
- [ ] Fast completion: `sqv BSC_ContractClear` and note the `Target` ref ID. Then `prid <id>`, `moveto player`, `kill` (or travel there and fight).
- [ ] Within about 5 seconds the "Contract fulfilled" box shows pay, treasury share and renown. The quest moves to Completed. `show BSC_Treasury` is above 0.

## 4. Contract: Marked for Death
- [ ] Take *Collect a bounty*. The objective reads "Kill **<name>** near **<dungeon>**" with a real name, e.g. *Hrolf Ashjaw*.
- [ ] Go to the marker (`sqv BSC_ContractBounty`, then `player.moveto <MapMarker id>`). The target and up to 2 bandit bodyguards are there and hostile.
- [ ] Killing the target pays out. Bodyguards don't need to die.

## 5. Contract: Hold the Line
- [ ] Take *Hold a fort*. The objective reads "Travel to **<fort>**".
- [ ] `sqv BSC_ContractHold`, then `player.moveto <MapMarker id>`. The objective switches to "Hold … until the raiders break" and 2+ raiders spawn and attack.
- [ ] Kill each wave. "More raiders are coming" appears twice. After the 3rd wave the contract pays out.
- [ ] Also try walking into the fort's interior before triggering it. It shouldn't trigger indoors.

## 6. Abandon
- [ ] Take any contract, then *Ledger > Current contract > Abandon*. The journal shows the quest as failed, renown drops by 10 (not below 0), and *Take a contract* is back.

## 7. Ranks and rewards
- [ ] `set BSC_Renown to 95`, then finish any contract. You get the **Promoted to Blade** box and the **Blackshield Blade**.
- [ ] `set BSC_Renown to 690`, then finish a contract. Sergeant and Captain boxes appear in a row, with the **Blackshield Shield** and **Blackshield Plate**.
- [ ] Check each item in the inventory. The 3D preview shows the ebony sword, ebony shield and steel plate cuirass. Names and enchantments are *Ember Edge*, *Shieldwall* and *Company Vigor*. The items can't be disenchanted.
- [ ] Equip all three and check them in third person, on both a male and a female character if you can.
- [ ] `set BSC_Renown to 1490`, finish a contract. **Promoted to Commander**, and *Commander's Presence* shows in Active Effects. The next payout is about 25% higher.

## 8. Treasury (Captain+)
- [ ] *Company treasury* offers Withdraw 100 / 500 / everything. Gold moves to you and `show BSC_Treasury` drops.
- [ ] Withdrawing from an empty treasury shows "The company treasury is empty."

## 9. Sellsword (Captain+)
- [ ] With no follower, *Summon a company sellsword*. A Blackshield Sellsword appears in steel plate and Blackshield gear and follows you.
- [ ] Talk to them. The vanilla follower options work (trade, wait, dismiss).
- [ ] Dismiss, walk away, summon again. The same sellsword is teleported to you and rejoins.
- [ ] With another follower (e.g. Lydia), summoning shows "Dismiss your current follower…" and doesn't break the other follower.
- [ ] Kill the sellsword (`prid <id>`, `kill`), then summon. A new one appears.

## 10. Persistence
- [ ] Save during an active contract, quit to desktop, reload. The contract still completes when the target dies.
- [ ] Save as Captain and reload. The treasury, rank and sellsword are kept.

## Stuck? Reset commands
```
stopquest BSC_ContractClear      (or BSC_ContractBounty / BSC_ContractHold)
set BSC_ContractActive to 0
set BSC_Rank to 3                (jump to Captain; renown is separate: set BSC_Renown to 700)
```
