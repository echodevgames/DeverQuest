# Guild Administrator Guide

## Administrative responsibility

A DeverQuest administrator controls local Guild identities, project assignments, content scaffolding, shared publication, correction review, reward/redemption policy, and optional Compensation Preview policy. The administrator is responsible for truthful policy, safe storage, backups, access control, licensed content, and a documented recovery process.

Do not treat hidden UI as security. Authority must be enforced by the service layer and by operating-system or server permissions around shared files.

## Rank model

| Rank | Intended authority |
|---|---|
| CEO | Full local Guild administration, including highest-risk actions |
| Boss | Broad administration except CEO-only destructive record/program actions |
| Project Leader | Manage Contracts, corrections, and project actions only within the assigned project |
| Member | Ordinary work input and personal use; no Guild administration |

The current service treats ordinary authenticated work input separately from administrative permissions. Verify every release with the four-rank matrix in Quest 1.

## Founding the Guild

1. Install and compile the package.
2. Open Guild Hall.
3. Create the founding CEO through **Secure Founding Account**.
4. Authenticate, sign out, and authenticate again.
5. Record where local account and audit state is stored.
6. Create a second authorized administrative account so recovery does not depend on one identity.
7. Create project-scoped leaders only after stable project IDs/names are decided.
8. Disable departed or test accounts instead of repurposing their identities.

Local DeverQuest accounts are not a substitute for enterprise identity management. Protect the machine, avoid password reuse, and do not store production secrets in ordinary notes.

## Account administration

For each account, maintain:

- stable account ID;
- display name;
- rank;
- enabled/disabled status;
- assigned project where relevant;
- associated Adventurer;
- date and reason for material changes.

Review the local audit trail after creating, disabling, re-enabling, or changing authority. Test denied operations, not just allowed ones.

## Project Leader scope

A Project Leader should receive authorized project operations only for the assigned project. When projects are renamed or reorganized:

1. preserve stable identifiers where possible;
2. update assignments deliberately;
3. test access to the old and new project contexts;
4. preserve existing Chronicle snapshots;
5. record the administrative change.

## Profile and organization policy

Define and publish a team policy for:

- expected Focus duration;
- idle timeout and warning;
- approved external activity providers;
- wellness reminder cadence;
- Approved Break definitions;
- Chronicle integrity and rollover limits;
- suspicious Quest length and daily Quest-count flags;
- healthy daily ranked Focus cap;
- rewards and economy values;
- shared repository publication;
- compensation-preview inclusion rules;
- data retention and deletion.

These values are productivity/game settings, not employment law. Obtain qualified advice for payroll, privacy, retention, accessibility, and labor requirements that apply to the organization.

## Chronicle root

Choose a writable, backed-up location. Avoid temporary folders and undocumented personal desktop paths. The Chronicle root can contain:

- daily Markdown timecards;
- machine-readable `.deverquest.json` session data;
- integrity/audit/correction records;
- media attachments and voice memos;
- history exports;
- rollover or continuation artifacts.

Use least-privilege folder access. A user who needs to create personal records does not necessarily need permission to rewrite every shared record.

## Chronicle integrity

Integrity hashes detect changed content and accidental corruption. They are not secret signatures because anyone with full write access can change a file and recompute an unkeyed hash.

Administrative rules:

1. Keep integrity enabled for ordinary use.
2. Protect original files and backups externally.
3. Do not “repair” a modified record by silently recomputing its hash.
4. Preserve the original, record the discrepancy, and use the correction workflow.
5. Restrict access to audit and correction records.
6. Test restoration from backup.

## Shared Guild repository

### Recommended structure

A configured repository publishes finalized Quest records beneath:

`<Guild Repository>/Records/<Account>/<date>/`

and current public Adventurer summaries beneath:

`<Guild Repository>/Adventurers/`

### Deployment requirements

- Studio-controlled folder ownership.
- Backups with tested restoration.
- Restricted modification permissions for ordinary Members.
- Clear policy for who can publish, correct, archive, and delete.
- Monitoring for sync conflicts and mass replacement.
- A separate test repository for upgrades.

### Validation and publication

1. Enable Shared Guild records.
2. Select the administrator-controlled root.
3. Configure the healthy daily ranking cap.
4. Validate the repository.
5. Enable automatic publication only after validation succeeds.
6. Complete a disposable Quest and inspect the published record.
7. Use **Publish Last Quest** as an idempotent retry and verify no duplicate.
8. Test unavailable and read-only states.

The folder-backed model is suitable for trusted local/team deployments. Internet-scale authority requires authenticated server-side validation, immutable or append-only storage, permissioned corrections, and protected signing keys or equivalent controls.

## Hall of Heroes and healthy ranking

The Hall of Heroes can compare eligible Focus, raw Focus, XP, coin, levels, streaks, Quests, Contracts, projects, and Departments. Competitive ranking should never incentivize unsafe duration or false records.

Keep the healthy daily Focus cap visible and documented. Suspiciously long sessions, high idle ratios, excessive daily frequency, modified records, or failed integrity should be capped, excluded, or flagged for review while preserving raw evidence.

## Content scaffolding

### Empty studio structure

Use **Create Empty Studio Structure** to establish organized project folders and blank templates. It must be safe to rerun and must preserve existing assets.

### Tutorial campaign

Use **Create Tutorial Campaign** to create the connected walkthrough `Trouble in the Tutorial Crypt`. Keep demonstration content separate from production content.

### Catalog generators

Generate and review:

- Identity Catalogs;
- Guild Combat Codex;
- Companion Stable;
- Tactical Starter Kit;
- Quest Profiles and Contracts;
- Shop and starter-loadout content.

Generated content is original starter material, but administrators remain responsible for the rights to every name, image, sound, font, story, and imported asset added later.

## Contract governance

A Quest Contract should identify:

- stable ID;
- title and objective;
- project and Department context;
- issuer/assignee policy;
- completion criteria;
- expected reward;
- lifecycle state;
- optional Quest Profile or encounter context.

Require project-scoped authorization for assignment and completion. Finalized sessions should store Contract snapshots so future edits do not rewrite history.

## Reward and economy administration

Separate motivational game economy from real compensation.

### Virtual rewards

Configure XP, coin, daily goals, blocks, completion bonuses, item prices, rarity, binding, trade eligibility, and loot intentionally. Test every change for duplicate grants and negative/overflow edge cases.

### Trading

Trading uses escrow and a durable ledger. Audit Accept, Reject, Cancel, and Reclaim. Do not manually move ownership records to “fix” a trade without preserving evidence.

### Real-world Redemption

A Redemption must remain a three-stage administrative process:

1. user request;
2. authorized approval/reservation;
3. manual delivery confirmation with an external reference.

DeverQuest does not deliver the benefit. Define who may approve, who may confirm delivery, how cancelled requests are handled, and how external receipts are retained.

## Compensation Preview administration

Only use Compensation Preview after adopting a written policy. Configure:

- hourly rate or annual-salary tracking equivalent;
- currency code;
- scheduled weekly hours;
- Approved Break inclusion;
- legacy Chronicle inclusion;
- integrity eligibility;
- long/frequent Quest review thresholds.

Rates are stored in local Editor preferences and are not encrypted payroll storage. The preview excludes active Quests and does not transfer money. Never represent it as payroll, a wage statement, a contractual promise, tax advice, or timekeeping approval without independent validation and appropriate legal/process controls.

## Audio administration

Use only audio that the organization has permission to distribute or use. Playlist and ambience share Unity's Editor preview transport and are mutually exclusive. Warning cues temporarily own the channel and should restore music safely.

For release testing, use recognizable clips and execute rapid Play/Stop/Next/Previous and cue-interruption tests. Layered audio, unstoppable audio, or audio continuing after assembly reload is a Critical release defect.

## External activity policy

An External Activity Profile identifies foreground tools using process names and optional window-title rules. On supported Windows behavior, recent keyboard or pointer activity is also required.

Approve only tools that represent legitimate project work. External activity can prevent false idle classification and can be recorded as evidence, but it must not independently create Focus time.

## Data backup schedule

Minimum recommendation:

- project assets: normal source-control cadence;
- local Chronicle root: daily or continuous protected backup;
- shared Guild repository: versioned backup with recovery points;
- release package and documentation: immutable release archive;
- EditorPrefs-backed local state: export or machine/profile backup before upgrades when practical;
- evidence and audit records: retained according to written policy.

Perform a restore drill before calling the product finished.

## Migration procedure

1. Freeze new feature work.
2. Complete or safely pause active Quests.
3. Record current Unity/package versions.
4. Back up project assets, local Chronicles, shared records, and local profile state.
5. Test the upgrade in a clone with disposable shared paths.
6. Run readiness and Quest 1 migration phases.
7. Compare account, Adventurer, inventory, Companion, Contract, and Chronicle counts.
8. Approve production migration only after evidence review.
9. Keep a rollback package and restoration instructions.

## Incident response

Treat these as high-priority incidents:

- false Focus time;
- duplicate completion or reward;
- missing/corrupt Chronicle;
- unauthorized Guild mutation;
- ownership or escrow inconsistency;
- modified shared records;
- exposed credentials or private media;
- layered/orphaned audio that persists beyond normal controls.

Immediate actions:

1. stop the affected workflow;
2. preserve original files and Console logs;
3. record exact account, project, Unity, and package versions;
4. copy the active-session/local state before resetting anything;
5. reproduce only in a disposable clone;
6. restore from known-good backup where necessary;
7. document correction and release impact.

## Administrator closeout checklist

- [ ] No active or unexplained recovered Quest.
- [ ] Latest Chronicle and shared publication verified.
- [ ] Failed writes/publications resolved or queued with evidence.
- [ ] Account changes and redemptions reviewed.
- [ ] Shared repository and Chronicle backups succeeded.
- [ ] Release-readiness blockers absent.
- [ ] Known issues and scope document current.
