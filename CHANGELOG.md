# Changelog

All notable changes to Blackshield Company. Versions follow [Semantic Versioning](https://semver.org/).

## [Unreleased]

## [0.1.2] - 2026-10-10

Still early beta, not yet playtested in game.

### Fixed
- Wrye Bash could not load the plugin (IndexError "list index out of range"). The first stage's quest log entry was written without its QSDT (log entry flags) subrecord, which every log entry must start with. Every log entry now has one. No gameplay changes.

## [0.1.1] - 2026-10-08

### Fixed
- The recruitment letter now arrives. On 1.6 the company quest does not start without `SEQ/BlackshieldCompany.seq`, so a new game never got the courier. Save/load also restarts the recruitment timer and an active contract's poll.

## [0.1.0] - 2026-10-06

Early beta. First public release. It passes every automated check in the build (Papyrus compile, plugin validation, round trip, FOMOD schema) but has not been playtested in-game yet. Please report problems on the Nexus page or on GitHub.

### Added
- Recruitment by courier letter once you are level 2 and out of the Helgen intro.
- Company Ledger lesser power and ledger book: take contracts, check renown and rank, use the treasury.
- Three radiant contract types on vanilla locations inside Skyrim's holds:
  - Clear the Camp: kill the leader of a bandit camp.
  - Marked for Death: hunt one of five named targets and their bodyguards near a dungeon.
  - Hold the Line: defend a fort against three waves of raiders.
- Pay scales with level and rank. A share (20% by default) goes to the company treasury.
- Ranks from renown: Recruit, Blade (100), Sergeant (300), Captain (700), Commander (1500).
- Rank rewards: Blackshield Blade, Blackshield Shield, Blackshield Plate, Commander's Presence.
- Treasury withdrawals from Captain.
- Company sellsword from Captain, using the vanilla follower system.
- ESL-flagged plugin. Skyrim.esm is the only master. No SKSE or other requirements.
- FOMOD installer with optional Papyrus sources.
