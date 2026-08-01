# Quick Start and Installation

## Requirements

- Unity 2022.3 LTS or a later compatible Unity Editor.
- Permission to install a local Unity Package Manager tarball.
- A writable folder for Chronicles and media.
- Optional: a Git repository, microphone permission, audio clips, and a protected shared folder.

## Clean installation

1. Back up the Unity project and any existing DeverQuest Chronicle folder.
2. Open **Window > Package Manager**.
3. Select **+ > Add package from tarball**.
4. Choose `com.echodevgames.deverquest-0.30.2.tgz`.
5. Wait for import and script reload to finish.
6. Confirm the Console has no red compilation errors.
7. Open **Tools > DeverQuest > Developer Companion**.
8. Run **Tools > DeverQuest > Run Release Readiness Check**.

## First profile

During first-time setup:

1. Enter a stable developer/profile name. Avoid renaming it casually after records exist.
2. Choose a Chronicle/timecard location you can back up.
3. Set Focus and idle defaults.
4. Configure wellness reminders and Approved Break durations.
5. Keep Chronicle integrity enabled unless a controlled migration requires otherwise.
6. Save the profile and reopen the window to confirm persistence.

## First Guild account

1. Open **Guild Hall**.
2. Use **Secure Founding Account** to create the initial CEO identity.
3. Use a unique credential appropriate for the local machine.
4. Sign out and authenticate once before adding other accounts.
5. Create an Adventurer and select generated identity content if available.

DeverQuest's local account store is a local authority convenience, not an internet identity provider. Protect the machine and do not reuse sensitive passwords.

## First Quest

1. Open the **Quest** workspace.
2. Enter a concrete goal.
3. Optionally select a Quest Profile and Quest Contract.
4. Start the Quest.
5. Add notes in **Quest Log & Git** while you work.
6. Use Pause, Meditation, or an Approved Break for the correct kind of interruption.
7. Complete the Quest and enter final log details.
8. Open **Rewards & History** and inspect the finalized Chronicle.

## Recommended first-day settings

| Setting | Safe starting point | Reason |
|---|---:|---|
| Focus duration | 25-50 minutes | Long enough for meaningful work |
| Idle timeout | 5 minutes | Detects absence without punishing brief reading |
| Idle warning | 30 seconds | Gives a chance to acknowledge activity |
| Short break | 5-10 minutes | Clear recovery boundary |
| Meal break | 20-45 minutes | Separates meal time from Focus |
| Daily Focus goal | Personal/team policy | Motivation only, not payroll |

## Optional integrations

- **Git:** use a project already initialized as a Git repository. Review staged changes before committing.
- **External Activity:** configure a ScriptableObject profile for supported foreground creative tools. External activity is evidence, not Focus time.
- **Voice memo:** grant microphone permission to Unity Editor.
- **Shared Guild repository:** use an administrator-controlled path with backups and restricted rewrite permissions.
- **Audio:** create Playlist, Warning, and Ambience Profile assets and assign licensed clips.

## Upgrade installation

1. Finish or safely pause the active Quest.
2. Back up local Chronicles, the Unity project, and shared records.
3. Record the current package and Unity versions.
4. Replace the package through Package Manager.
5. Allow migration and script reload to finish.
6. Run Release Readiness Check.
7. Verify profile, accounts, Adventurers, inventory, Companions, Contracts, settings, and recent Chronicles before normal work.

## Uninstall

Uninstalling the package removes package code, not necessarily local Editor preferences, generated project assets, Chronicle files, media, exports, or shared Guild records. Back up first, then remove each storage category intentionally. See the Data, Backup, Privacy, and Migration guide.
