# Data, Backup, Privacy, and Migration

## Storage map

DeverQuest does not use one monolithic save file. Backups and clean tests must cover every storage family.

### Local Unity Editor preferences

The package uses EditorPrefs keys for local state, including:

| Area | Representative key/prefix |
|---|---|
| Profile | `EchoDevGames.DeverQuest.Profile.v1` |
| Active/last session | `EchoDevGames.DeverQuest.ActiveSession.v1`, `...LastCompletedSession.v1` |
| Guild accounts/auth/audit | `...GuildAccounts.v1`, `...CurrentGuildAccount.v1`, `...GuildAudit.v1`, `...GuildAuthenticated.v1` |
| Adventurer | `EchoDevGames.DeverQuest.Adventurer.v1` |
| Rewards | `EchoDevGames.DeverQuest.RewardWallet.v1` |
| Shop/trade | `...GuildShopLedger.v1`, `...TradeLedger.v1` |
| Playlist/audio | playlist GUID/index, Warning Profile, Ambience Profile/index |
| External activity | selected External Activity Profile |
| Git | observed repository and HEAD |
| Wellness | snooze, lunch, dinner, and quiet-hour markers |
| Compensation | local policy data managed by Compensation Service |

EditorPrefs are scoped to the local operating-system user and Unity installation behavior, not automatically to one project. Do not assume a fresh project means a fresh DeverQuest identity.

### Project assets

Generated and authored ScriptableObjects live under the project `Assets` tree, commonly `Assets/DeverQuest`. They should be versioned with `.meta` files.

### Local Chronicle root

The configured root generally contains a `DeverQuestTimecards` hierarchy or resolves to an administrator-selected timecard path. It can contain Markdown, `.deverquest.json`, `.audit.json`, `.corrections.json`, Media, exports, and continuation/rollover artifacts.

### Shared Guild repository

Optional shared publication uses `Records` and `Adventurers` directories beneath the configured root.

## Backup plan

### Before every upgrade

- [ ] Copy the package tarball and record checksum.
- [ ] Commit or archive project assets.
- [ ] Copy the full local Chronicle root.
- [ ] Copy the full Shared Guild repository.
- [ ] Record active Quest state and finish/pause it safely.
- [ ] Preserve local EditorPrefs-backed state through an OS/profile backup or a documented export strategy where available.
- [ ] Record Unity, package, operating-system, and Git versions.

### Routine schedule

| Data | Suggested protection |
|---|---|
| Project assets | Git with remote backup |
| Local Chronicles/media | Daily versioned backup or approved sync |
| Shared records | Administrator-controlled versioned backup |
| Release tarballs/docs | Immutable release archive with checksum |
| Audit/corrections | Same or stronger protection than Chronicles |
| Local preferences | Machine/profile backup before upgrades or device replacement |

## Restore drill

1. Restore into a disposable path and project clone.
2. Keep restored shared publication disabled initially.
3. Install the matching package version.
4. Restore project assets and local preference state where available.
5. Point DeverQuest at the restored Chronicle root.
6. Validate recent records and integrity.
7. Restore the shared repository and validate it separately.
8. Compare account, Adventurer, inventory, Companion, Contract, and Chronicle counts.
9. Run a short disposable Quest and verify new writes.
10. Document the drill date and result.

## Privacy inventory

Potentially sensitive data includes:

- developer and account names;
- project and Department membership;
- work goals, notes, blockers, and commit messages;
- timestamps and duration classifications;
- foreground external-application evidence;
- voice recordings and attached files;
- wellness reminder acknowledgements and break history;
- character/economy/trade/redemption activity;
- compensation rates and estimates;
- administrative audit and correction decisions.

Adopt a written purpose, access, retention, export, correction, deletion, and incident policy before organizational use. Do not collect data merely because the interface can.

## Data minimization

- Use concise work notes.
- Avoid secrets, credentials, medical details, customer data, or unrelated personal information.
- Attach only necessary media.
- Do not use real compensation values in QA.
- Limit shared snapshots to the intended public fields.
- Separate demo/test and production records.

## Deletion and reset

`Reset DeverQuest Profile` should not be interpreted as a universal eraser. Local subsystem keys, generated assets, Chronicles, media, exports, and shared records may remain.

For an intentional full local removal:

1. back up required records;
2. uninstall the package;
3. remove generated project assets deliberately through Unity/Git;
4. archive or delete the selected Chronicle root according to policy;
5. archive or delete shared records only with administrator authority;
6. remove relevant local EditorPrefs values using a documented tool/process;
7. verify backups and deletion scope.

Never advise a user to wipe all Unity EditorPrefs indiscriminately on a production workstation.

## Migration rules

- Every persisted schema needs a data version.
- Migrations move forward predictably and should be safe to rerun.
- Stable IDs survive display-name and catalog-order changes.
- Older records remain readable or are clearly classified as legacy.
- A failed migration preserves the original input.
- Migrations do not silently recalculate historical rewards or compensation.
- Shared publication remains disabled until local migration is verified.

## Moving to another machine

1. Install a compatible Unity version.
2. Clone/copy the Unity project with assets and `.meta` files.
3. Install the matching DeverQuest package.
4. Copy the Chronicle root and media.
5. Restore or recreate local Guild/account/profile state carefully.
6. Reconnect the Shared Guild repository with appropriate permissions.
7. Re-select local ScriptableObject assets referenced by EditorPrefs if GUID/path resolution changes.
8. Validate recent history and run a QA Quest.

## Integrity and authority

A valid SHA-256 means the file matches the hash stored for it. It does not prove who created it. Stronger authority requires protected permissions, server-side validation, append-only storage, digital signatures using protected keys, or equivalent controls.

## Retention template

| Data class | Owner | Access | Retention | Deletion trigger | Backup |
|---|---|---|---|---|---|
| Local Chronicles | | | | | |
| Shared records | | | | | |
| Media/voice | | | | | |
| Audit/corrections | | | | | |
| Compensation policy/export | | | | | |
| QA evidence | | | | | |
