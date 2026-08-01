# DeverQuest System Architecture Manuscript

## Preface

DeverQuest began as a Unity focus timer and grew into a local-first developer companion with structured records, wellness controls, Git and media evidence, guild authority, rewards, content catalogs, inventory and trading, deterministic tactical encounters, Companions, Survival Quests, shared rankings, and compensation estimation. The central design challenge is therefore not adding more systems. It is preserving a single trustworthy thread through all of them:

> A Quest records deliberate work. Everything else may describe, support, reward, or dramatize that work, but nothing else is allowed to invent it.

This manuscript describes package version 0.30.2 as a maintainable system rather than a tour of buttons.

## 1. Package shape

The package has two assemblies:

| Assembly | Role |
|---|---|
| `EchoDevGames.DeverQuest.Runtime` | ScriptableObject definitions and serializable authored data |
| `EchoDevGames.DeverQuest.Editor` | Window, services, persistence, integrations, records, and operational logic |

Although several definitions are in a Runtime assembly, the product itself is an Editor companion. The runtime definitions make authored assets clean and reusable; they do not imply that a built game needs the DeverQuest window or local account system.

### Principal entry points

- `DeverQuestWindow`: IMGUI workspaces and user orchestration.
- `DeverQuestReleaseReadinessService`: preflight findings.
- `DeverQuestSettingsStore`: local profile persistence and migration.
- `DeverQuestSessionStore`: active/recent session persistence and state transitions.
- `DeverQuestTimecardWriter`: finalized local Chronicle output.
- `DeverQuestHistoryService`: history loading, summaries, and exports.
- `DeverQuestGuildAccountService`: local authentication, rank, permission, and audit state.
- `DeverQuestAdventurerService`: persistent RPG identity.

## 2. Architectural layers

### Presentation layer

`DeverQuestWindow` renders one selected workspace at a time. Live Quest and Quest Log views repaint at a controlled cadence; inactive heavy workspaces are intended to remain event-driven. This is important because the window can otherwise trigger file, Git, asset, history, or shared-record work on every timer repaint.

The UI is not the authority boundary. Buttons may be hidden or disabled for usability, but services must independently validate permissions and current state.

### Orchestration layer

The window coordinates service calls, displays validation, and translates user actions into domain operations. It should not become the only place where invariants live. A direct service call must not be able to bypass rules that the UI enforces visually.

### Domain-service layer

Services operate on sessions, accounts, characters, rewards, Contracts, combat, inventory, trades, content, and integrations. Most are static/local services because the package is an Editor utility. This simplifies installation but creates shared-state and test-isolation obligations.

### Persistence layer

DeverQuest has three persistence families:

1. **Unity Editor preferences:** local profile, account/auth state, Adventurer, active session, recent session, wallet, ledgers, selected assets, wellness markers, Git observation, and other local choices.
2. **Project assets:** ScriptableObject content under the Unity project's `Assets` tree.
3. **Disk records:** Chronicles, media, exports, corrections, audits, and optional shared Guild publication.

A project clone can duplicate assets and files but not necessarily the local EditorPrefs identity. Conversely, resetting the visible profile does not necessarily erase every subsystem or disk record. Test and uninstall documentation must respect that split.

## 3. The Quest state machine

`DeverQuestSession` is the durable record model. `DeverQuestSessionStore` owns the active and last-completed local session state.

### Core session states

- Running
- Paused
- Completed

Additional classification is represented through timing/event fields rather than by pretending every interval is Focus.

### Start

A new Quest snapshots relevant context:

- developer/account/Adventurer identity;
- project and Department;
- goal;
- Quest Profile and Contract context;
- Focus Stages and party context;
- timing settings;
- optional encounter state;
- relevant content IDs and display values.

Start must reject or safely resolve an existing active Quest. Double clicks must not create two active records.

### Update

Editor update callbacks advance eligible active timing and poll bounded background systems. Update logic must avoid writing files, refreshing full Git status, scanning AssetDatabase, or rebuilding history on every frame.

### Pause and resume

Pause closes the current eligible interval and preserves the Quest. Resume begins a new eligible interval without changing session identity. Repeated Pause or Resume calls should be idempotent or rejected cleanly.

### Meditation, Approved Break, and idle

These intervals must be recorded separately. Their boundaries affect both time totals and reward eligibility. Approved Break completion depends on the configured threshold; excess duration becomes Idle/Unverified.

### Assembly reload and editor exit

Before scripts reload or Unity exits, active state and audio/recording resources must be made safe. A Quest should recover paused rather than accrue invisible time. Microphone recording and Editor preview audio must be stopped. Persistence should be durable enough to complete or abandon the recovered Quest later.

### Completion

Completion is a transaction-like sequence:

1. validate state and required final context;
2. close the current interval;
3. resolve any permitted stage/encounter outcomes;
4. calculate eligible rewards once;
5. write human and machine Chronicle records;
6. update local character/economy state;
7. publish optional shared records;
8. mark the session completed and persist last-completed state;
9. clear active state;
10. surface partial failures with retry paths.

The precise implementation may not be a database transaction, so idempotency identifiers and ordered error handling are vital. A retry must not double rewards, trades, or shared publication.

### Abandonment

Abandonment terminates active work without representing it as normal success. It should preserve enough evidence for diagnosis and must not award completion-only benefits.

## 4. Time accounting

The time model distinguishes semantic categories:

| Category | Counts as Focus | Typical reward eligibility |
|---|---:|---:|
| Focus | Yes | Yes, after finalization and policy checks |
| Meditation | No | No Focus reward |
| Approved Break | No | Optional break-specific XP/benefit only |
| Idle/Unverified | No | No |
| External Activity evidence | No independent seconds | No independent reward |

Wall-clock duration is not a trustworthy substitute for the sum of classified intervals. Every report and compensation estimate should derive from finalized categorized data.

## 5. Profile and settings

`DeverQuestProfile` currently uses data version 14 and supplies defaults such as:

- 50-minute Focus duration;
- idle detection enabled, 5-minute timeout, 30-second warning;
- activity recognition for Play Mode, compilation, asset import, and builds;
- wellness enabled with 30-minute check-in, 60-minute movement, 45-minute hydration, and 120-minute exercise intervals;
- 10-minute snooze, 5-minute short break, 30-minute meal break, 15-minute quiet break;
- reward blocks at 30 minutes and daily goal at 240 minutes;
- Chronicle integrity enabled, 12-session/512 KB limits;
- suspicious Quest threshold at 240 minutes and suspicious daily count at 8;
- shared healthy daily Focus cap at 600 minutes;
- EchoNeon theme and notification defaults.

Defaults are starting values, not universal health, employment, or productivity policy. Migrations must preserve established user choices while adding new fields safely.

## 6. Guild accounts and authority

`DeverQuestGuildAccountService` stores local accounts, current account, authentication state, and audit journal in EditorPrefs.

### Permission model

`WorkInput` is available to an authenticated account. CEO receives all defined permissions. Boss receives broad authority except `DeleteRecords` and `DeleteProgram`. Project Leader receives supported Contract, correction-review, and project management permissions only when the target project matches the assigned project. Member receives ordinary authenticated work behavior.

### Security boundary

The service is suitable for local trust and lightweight studio use. EditorPrefs is not a secure credential vault, and local authentication does not establish remote identity. Machine access, backups, shared-folder permissions, and organization policy remain essential.

### Audit

Material account and administrative actions should create audit entries. The audit log is evidence, not immutable storage when the same local user controls all EditorPrefs data.

## 7. Adventurer state

`DeverQuestAdventurerService` persists the current account's character model. It can include:

- stable character identity;
- Ancestry, Class, Faith, Alignment, and Department;
- attributes, HP, Mana, progression, and levels;
- coin and inventory;
- equipment and learned abilities;
- Companion roster;
- tactical and survival state.

Every persisted value must be scoped to the correct Guild account. Sign-in changes should load the associated Adventurer without cross-account state bleed.

## 8. Authored identity catalogs

Runtime identity assets include:

- `DeverQuestAncestry`
- `DeverQuestClassDefinition`
- `DeverQuestDeity`/Faith content
- `DeverQuestIdentityCatalog`
- `DeverQuestIdentityCatalogRegistry`

Catalog-driven creation replaced brittle hard-coded choices. Stable IDs protect existing Adventurers when display names or catalog ordering changes. Eligibility validation belongs in services and creation logic, not solely in popup options.

## 9. Quest Profiles, Contracts, parties, and stages

`DeverQuestQuestProfile` defines reusable Quest shape. `DeverQuestQuestContract` represents assignable work and includes status, priority, Focus Stages, party context, and project/reward metadata.

### Snapshot rule

An active/finalized Quest should store a snapshot of selected content. Assets remain editable for future Quests, but historical records must not silently change when a designer edits a Profile, Contract, class, item, or monster.

### Contract service

`DeverQuestContractService` validates creation, selection, assignment, completion, and authority. Project-scoped actions must compare stable project context.

### Focus-stage progression

Stage data can define ordered work segments and pace expectations. The system can calculate cascading pace effects. It must preserve the actual transition record and avoid granting the same stage reward twice.

## 10. Idle monitoring

`DeverQuestIdleMonitor` evaluates configured activity signals. Project activity can include Unity focus/input, Play Mode, compilation, asset import, builds, and supported external activity.

An idle warning gives the user a final acknowledgement window. Passing the threshold closes eligible Focus and records an idle/unverified interval. Idle detection must not be defeated merely by leaving a configured process open without foreground status and recent input.

## 11. External activity monitoring

`DeverQuestExternalActivityProfile` contains providers with process name, optional window-title text, and freshness settings. `DeverQuestExternalActivityMonitor` observes supported foreground activity, principally on Windows.

External activity has two purposes:

1. prevent false idle classification while the developer actively works in a configured tool;
2. append evidence intervals to the Chronicle.

It never grants Focus time on its own.

## 12. Wellness monitoring

`DeverQuestWellnessMonitor` tracks reminder schedule, snooze state, meal/day markers, and quiet-hours events. Session wellness events preserve acknowledgements and Approved Break results.

The monitor must distinguish:

- acknowledgement;
- snooze;
- break start;
- completed break;
- early return;
- excess idle beyond approved duration.

It is a behavioral prompt, not medical care.

## 13. Chronicle writing

`DeverQuestTimecardWriter` translates a finalized session into a daily record. The package writes human-readable Markdown and machine-readable `.deverquest.json` data. A daily record may be rolled or continued when configured session-count or file-size limits are reached.

Chronicle output can include:

- identity and Quest context;
- goal, notes, and final entry;
- timing classifications;
- commits and repository context;
- external activity and media;
- wellness events;
- rewards and transactions;
- Focus Stages and Contracts;
- battle results and typed damage;
- Companion state;
- integrity metadata.

Writer failure must not silently mark the session fully finalized without a retry path.

## 14. Integrity, audit, and corrections

`DeverQuestChronicleIntegrityService` uses `.audit.json` and `.corrections.json` journals around Chronicle records.

### Integrity status

A record can be valid, modified, legacy/unavailable, or otherwise classified by the service. Compensation and shared ranking can include or exclude records based on policy.

### Correction model

Corrections should add an authorized explanation and adjusted interpretation without erasing the original evidence. A corrected record should retain who, when, why, and what changed.

### Threat limit

SHA-256 detects content changes but is not a keyed signature. An attacker with full rewrite access can replace content and hash. External access control and immutable/versioned backups are the true authority boundary.

## 15. History and exports

`DeverQuestHistoryService` loads Chronicle JSON, groups days, calculates named/project/Department summaries, derives goal statistics, and exports filtered CSV or JSON.

History loading must tolerate an invalid individual record without crashing the entire interface. Exports should clearly preserve filters, date range, and record eligibility.

## 16. Rewards

`DeverQuestRewardService` stores a local wallet and categories such as game time and other fun. Quest rewards can include base completion, work blocks, daily goals, tactical/stage outcomes, break-specific XP, coin, items, and character progression.

Reward invariants:

- calculate from finalized eligible data;
- identify the source session/transaction;
- grant once;
- never derive Focus rewards from Meditation, idle, external evidence, or RPG actions;
- preserve a ledger adequate for diagnosis.

## 17. Shop and inventory

Runtime shop types define Shop Item type, rarity, binding, real-reward type, inventory entry, Shop Item, and Shop Profile. `DeverQuestShopService` persists a purchase ledger.

Inventory separates definition identity from ownership identity. Stackable ordinary items may share definition IDs; equipment, redemptions, and rare records can require unique ownership IDs and provenance.

Purchase should validate account, funds, availability, item rules, and duplicate-click protection before charging and granting.

## 18. Trading

`DeverQuestTradeService` persists offers in a local trade ledger. The lifecycle includes Open, Accepted, Rejected, Cancelled, and reclaim behavior.

Escrow is the central invariant: while an offer is open, the sender cannot also spend, equip, or trade the same ownership record. Accept transfers ownership once. Reject keeps it reclaimable. Cancel returns it to the sender. Bound, forbidden, or Redemption records never enter escrow.

## 19. Real-reward redemption

A real-world Shop Item uses the Redemption type. The package records request, leadership approval/reservation, and manual delivery confirmation. It cannot call external fulfillment systems by itself and must never mark delivery before an authorized human confirms it.

## 20. Compensation Preview

`DeverQuestCompensationService` stores local policy and calculates current-week and filtered-history estimates. Policy can choose hourly or annual-equivalent basis, currency, weekly hours, approved-break treatment, integrity policy, and legacy inclusion.

The service excludes active Quests, Meditation, and Idle/Unverified time. Modified/unavailable records are excluded according to integrity policy. Rates are deliberately not written to daily Chronicles or shared public snapshots.

This subsystem is an estimate and should remain visibly disclaimed.

## 21. Shared Guild publication

`DeverQuestSharedGuildService` writes shared Quest records and Adventurer snapshots beneath `Records` and `Adventurers` directories. Hall entries aggregate healthy ranked Focus and RPG/project metrics.

### Publication order

Local finalization should complete before shared publication. Shared failure should be visible and retryable without repeating local rewards or creating duplicate records.

### Healthy ranking

Ranking caps and integrity filters prevent the leaderboard from rewarding extreme duration, high idle ratio, excessive Quest count, or modified records. Raw evidence remains available for review.

## 22. Git integration

`DeverQuestGitService` performs explicit repository status and commit operations. `DeverQuestGitMonitor` observes HEAD on a lightweight interval and expands to full status only when needed.

Git command execution must:

- quote paths and arguments safely;
- avoid blocking the Editor excessively;
- present stdout/stderr meaningfully;
- reject invalid/empty commit requests;
- never claim a push or remote operation occurred when only a local commit was created.

## 23. Media and voice memos

`DeverQuestVoiceMemoService` manages microphone recording and WAV attachment. Existing file attachment copies media into a dated protected folder.

Resource cleanup is mandatory on cancellation, reload, abandonment, and editor shutdown. Media can contain personal or confidential information and needs retention/access policy outside the code.

## 24. Audio architecture

### Constraint

Unity's internal Editor preview API is a shared transport. It does not provide dependable independent mixer channels for package music, ambience, and cues.

### Ownership model

- `DeverQuestEditorAudioBridge` is the low-level compatibility boundary.
- `DeverQuestPlaylistPlayer` owns long-form Playlist state.
- `DeverQuestAudioDirector` arbitrates Playlist, ambience, and short cues.

Playlist and ambience are mutually exclusive. A cue interrupts the current long-form owner, captures state/sample position where possible, plays once, and restores only the valid prior owner. A newer cue replaces an older cue. Stop must clear both pending restoration and active preview audio.

### Lifecycle cleanup

Assembly reload and editor exit must stop preview audio and clear ownership. Automatic track advancement should use actual playback state rather than estimated wall-clock end time.

## 25. Combat typing

Runtime combat assets define creature types, damage types, responses, affinities, and a catalog. `DeverQuestDamageService` resolves raw typed damage into final damage/healing.

Canonical effects:

- resistance: half damage;
- vulnerability: double damage;
- immunity: zero damage;
- absorption: convert qualifying damage to healing;
- resistance plus vulnerability: normal damage;
- duplicate defenses: no unintended multiple stacking.

A `DeverQuestDamageResolution` should preserve raw amount, final amount, response, and source context for the Battle Chronicle.

## 26. Tactical combat

`DeverQuestTacticalCombatService` manages combat state, active effects, ability use, enemy response, Companion contribution, victory, defeat, flee, and safety actions. `DeverQuestEncounterService` supplies encounter/wave data.

Determinism is valuable because QA can reproduce an outcome from the same authored inputs. If randomness is introduced later, preserve seed and roll evidence.

Combat can alter character HP, Mana, items, Companion state, XP, coin, and loot. It cannot alter Focus time except by changing the Quest's explicit running/paused state.

## 27. Companions

Runtime Companion assets define kind, role, state, Profile, and Catalog. `DeverQuestCompanionService` controls recruitment, active selection, progression, damage/recovery, and eligibility.

One active Companion joins a deterministic encounter. Role logic may include striker damage, guardian interception, support restoration, or controller hindrance. Persistent state is scoped to the Adventurer/account.

## 28. Survival Quests

Encounter Profiles can define normal or Survival modes with waves, weighted drop entries, par information, and exit behavior. Survival tests must cover wave transitions, victory, voluntary exit, defeat, safety pause, rewards, loot, and recovery from reload.

## 29. Encumbrance and coin

`DeverQuestEncumbranceService` derives carried burden from weighted inventory and physical coin. Denomination exchange changes representation but must preserve exact total value. Encumbrance may affect RPG state but not rewrite historical work time.

## 30. Rules and deterministic checks

`DeverQuestRulesService` returns structured rule results for character and gameplay checks. A result should expose success/failure, explanation, and any deterministic values needed by the UI or Chronicle. Rule failures should be readable, not merely disable a button without explanation.

## 31. Content generators

Generators create starter content under `Assets/DeverQuest`:

- starter gear, shop, encounters, and combat;
- original identity catalogs;
- original Companion stable;
- tactical content;
- empty studio structure;
- tutorial campaign.

Generators should be rerunnable, preserve user assets, assign stable references, save/import cleanly, and never modify package source. Explicit generic asset types are required for Unity/C# compatibility in the Tactical generator.

## 32. Release readiness

`DeverQuestReleaseReadinessService` examines package version, Unity version, profile, storage writability, integrity settings, shared repository, Editor audio support, and active Quest state. It reports findings by severity.

Readiness is a preflight, not full QA. It cannot prove timing accuracy, permission denial, audio ownership under rapid input, migration, or failure recovery. Quest 1 supplies those proofs.

## 33. Performance model

The principal risks are IMGUI repaint frequency, AssetDatabase scans, file-history enumeration, Git process execution, shared-repository scanning, JSON serialization, and audio/microphone polling.

Performance rules:

- render only the selected workspace;
- repaint live timer views at bounded frequency;
- cache/reuse loaded content where safe;
- use lightweight HEAD observation before full Git status;
- perform explicit refresh for expensive history/shared views;
- avoid disk writes every frame;
- unregister editor callbacks on teardown;
- test large disposable histories and catalogs.

## 34. Error-handling model

Every integration can fail independently:

- Chronicle folder unavailable;
- shared folder unavailable/read-only;
- Git absent or repository invalid;
- microphone denied;
- AudioUtil API changed;
- ScriptableObject missing;
- malformed JSON;
- account disabled;
- insufficient funds;
- item locked in escrow;
- encounter data invalid.

The core principle is graceful degradation. Optional failure should not corrupt the active Quest. A user-visible message should identify the failed subsystem, preserve recoverable state, and offer a safe retry or configuration path.

## 35. Privacy model

DeverQuest can record names, work goals, notes, projects, commits, application activity, voice, attached media, work duration, wellness acknowledgements, compensation estimates, and administrative decisions. That is potentially sensitive even when no field is labeled “private.”

The package itself does not provide encryption, enterprise identity, legal retention, or remote access control. Administrators must define collection purpose, access, retention, deletion, backup, and incident response.

## 36. Scope lock and extension seams

The 0.30 release candidate intentionally defers crafting, banking, housing, biome simulation, and broad tradeskill/weapon-skill progression. Existing seams for later growth include item definitions, identity catalogs, ability assets, inventory ownership, stable IDs, and generator patterns.

New systems should not be introduced until Quest 1 passes and the release has observed real use. The next architectural improvement should favor automated tests, explicit repositories/storage abstraction, and service isolation before another major gameplay layer.

## 37. Maintenance priorities after 0.30

1. Add unit tests for time classification, reward idempotency, permission matrices, typed damage, coin conversion, escrow, and compensation inclusion.
2. Add integration tests for Chronicle write/retry, shared publication idempotency, migration, and recovery.
3. Abstract EditorPrefs stores so QA can isolate accounts and sessions per test.
4. Version every persisted schema and document migration direction.
5. Reduce `DeverQuestWindow` responsibility by extracting workspace presenters/controllers.
6. Replace reflection-based Editor audio compatibility when Unity provides a supported public API.
7. Add structured diagnostics export with secrets/privacy filtering.
8. Adopt a clear license and support policy.

## 38. Definition of finished

DeverQuest 0.30 is finished enough to release when:

- it installs and compiles on the supported Unity version;
- one Quest can be started, paused, recovered, completed, and audited without false time;
- every durable mutation is idempotent or safely retryable;
- rank boundaries resist unauthorized service actions;
- records survive supported migration;
- shared/audio/Git/media failures do not corrupt core state;
- the scope lock is honored;
- documentation matches observed behavior;
- Quest 1 has a signed PASS or approved Conditional Pass.
