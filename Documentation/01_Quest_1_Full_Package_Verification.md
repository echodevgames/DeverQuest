# Quest 1: The First Full Expedition

## Full Package Verification for DeverQuest 0.30.2

This is the first official DeverQuest Quest: prove that the package can be installed, configured, exercised, recovered, and released without losing the truth of a developer's work. Run it in order. A test may be marked Not Applicable only with a written reason.

> **Primary victory condition:** no unresolved Blocker or Critical defect, no unexplained data loss or duplication, no false Focus time, no unauthorized administrative mutation, and no reproducible layered audio.

## Test identity and evidence

| Field | Entry |
|---|---|
| Test run ID | `DQ-0302-________` |
| Tester | |
| Release owner | |
| Date started | |
| Date completed | |
| Unity version | |
| Operating system | |
| Package SHA-256 | |
| Project path | |
| Chronicle root | |
| Shared Guild repository | |
| Git branch/commit | |

Create four disposable accounts unless a test states otherwise:

- `DQ-QA-CEO`
- `DQ-QA-BOSS`
- `DQ-QA-LEAD`
- `DQ-QA-MEMBER`

Do not reuse personal passwords, real compensation rates, real employee records, production Chronicles, or a production Shared Guild repository.

## Result notation

For every test, mark exactly one result:

- `[ ] PASS`
- `[ ] FAIL`
- `[ ] BLOCKED`
- `[ ] N/A`, with a reason

For failed or blocked tests, add a bug ID and evidence path.

## Severity

| Severity | Meaning | Release effect |
|---|---|---|
| Blocker | Package cannot install/compile/open, core data is destroyed, or testing cannot continue | Stop the run; release forbidden |
| Critical | False or duplicated Focus/rewards, unauthorized control, unrecoverable active Quest, corrupted Chronicle, severe audio/resource leak | Release forbidden |
| Major | Important system fails with a viable workaround, migration defect without data loss, broken administrative workflow | Fix or formally defer before release |
| Minor | Limited defect that does not threaten records or core use | May ship with documented disposition |
| Cosmetic | Presentation defect only | May ship if legible and recorded |

## Rule of evidence

Capture the Unity Console, relevant panel, generated file, and reproduction steps for every failure. Preserve original failing files before attempting repair. A test is not a PASS merely because it worked once; repeat timing, recovery, trading, reward, publication, and audio tests whenever a failure could duplicate or corrupt persistent state.

## Phase A: Prepare the Expedition
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 1 | Create a disposable Unity 2022.3 LTS project dedicated to DeverQuest QA. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 2 | Record the exact Unity editor version, operating system, package file name, package SHA-256, tester, date, and test-machine name. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 3 | Create a dedicated QA evidence folder outside the Unity project. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 4 | Copy the original 0.30.2 package tarball into the evidence folder without modifying it. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 5 | Confirm the project is under source control or has a restorable snapshot before testing package migration or generated assets. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 6 | Choose a disposable Chronicle root that contains no real work records. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 7 | Choose a disposable Shared Guild repository that contains no real studio records. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 8 | When practical, use a disposable operating-system user account because Unity Editor preferences outlive a project. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 9 | Disable automatic cloud synchronization for the disposable test folders unless sync behavior is part of the test. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 10 | Close unrelated Unity editors that could share microphone or Editor audio resources. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 11 | Record the expected release scope and agree not to add features during this verification run. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 12 | Create bug identifiers using the format `DQ-0302-###`. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |

## Phase B: Install, Compile, and Prove Readiness
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 13 | Open Window > Package Manager and install `com.echodevgames.deverquest-0.30.2.tgz` using Add package from tarball. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 14 | Allow the first import and script reload to finish without cancelling it. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 15 | Confirm there are no red compilation errors in the Unity Console. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 16 | Confirm both `EchoDevGames.DeverQuest.Runtime` and `EchoDevGames.DeverQuest.Editor` assemblies compile. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 17 | Open Tools > DeverQuest > Developer Companion. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 18 | Open every Tools > DeverQuest > Workspaces menu entry once. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 19 | Run Tools > DeverQuest > Run Release Readiness Check. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 20 | Confirm the readiness report identifies package version 0.30.2. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 21 | Confirm the readiness report recognizes the supported Unity version. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 22 | Confirm the readiness report can write to the selected Chronicle/timecard location after configuration. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 23 | Confirm no active Quest is unexpectedly recovered on a truly clean QA identity. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 24 | Capture the readiness report and Console as evidence. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 25 | Restart Unity and confirm the package still opens without errors. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |

## Phase C: Create the Local Profile and Storage Boundary
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 26 | Open first-time setup and enter a clearly marked QA developer/profile name. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 27 | Choose the disposable Chronicle root and confirm its resolved path. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 28 | Configure a short Focus duration suitable for testing, such as 2 minutes. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 29 | Enable idle detection and use a short test timeout. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 30 | Configure warning lead time so the warning can be observed during a short Quest. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 31 | Configure short, meal, hydration, movement, and quiet-hours reminders to testable values. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 32 | Enable Chronicle integrity and record the configured session and file-size limits. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 33 | Confirm the daily safety limits for long Quests and Quest count are visible. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 34 | Save the profile and close/reopen the window. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 35 | Confirm all saved values return after reopening Unity. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 36 | Confirm Reset/Reconfigure actions display enough context to avoid accidental production-data loss. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 37 | Document which settings are stored locally versus in assets or Chronicle files. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |

## Phase D: Establish Guild Authority and Character Identities
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 38 | Enter Guild Hall and create the founding CEO account `DQ-QA-CEO`. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 39 | Confirm the founding account can authenticate and sign out. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 40 | Create enabled accounts `DQ-QA-BOSS`, `DQ-QA-LEAD`, and `DQ-QA-MEMBER`. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 41 | Assign the Boss, Project Leader, and Member ranks respectively. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 42 | Assign the Project Leader to one QA project and record the exact project ID/name. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 43 | Confirm disabled accounts cannot authenticate. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 44 | Confirm incorrect credentials do not authenticate. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 45 | Confirm the CEO can manage accounts, contracts, shared settings, and corrections. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 46 | Confirm the Boss has expected administrative powers but cannot perform CEO-only destructive actions. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 47 | Confirm the Project Leader can manage only the assigned project scope. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 48 | Confirm the Member cannot access administrative mutations. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 49 | Inspect the local authority audit trail after account changes. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 50 | Create or migrate an Adventurer for each enabled QA account. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 51 | Confirm each Adventurer remains attached to the correct account after sign-out/sign-in. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 52 | Confirm Adventurer identity, progression, inventory, coin, and Companion roster do not bleed between accounts. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |

## Phase E: Generate and Validate Content Assets
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 53 | As CEO or Boss, open Guild Hall > Campaign Content Scaffolding. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 54 | Run Create Empty Studio Structure in the disposable project. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 55 | Confirm organized DeverQuest content folders and blank templates are created. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 56 | Run the empty-structure generator again and confirm existing assets are preserved. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 57 | Generate the original identity catalogs. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 58 | Generate the Guild Combat Codex. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 59 | Generate the original Companion Stable. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 60 | Generate the Tactical Starter Kit, including short and Survival Quest templates. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 61 | Create the Tutorial Campaign `Trouble in the Tutorial Crypt`. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 62 | Run each generator a second time and confirm it is safe to rerun. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 63 | Confirm generated ScriptableObjects have stable IDs and valid references. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 64 | Confirm no unexpected assets were written into package folders. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 65 | Confirm generated content remains under the project Assets hierarchy. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 66 | Commit or snapshot generated content before destructive authoring tests. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |

## Phase F: Prove the Core Quest Lifecycle
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 67 | Authenticate as `DQ-QA-MEMBER` and start a basic Quest. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 68 | Confirm Focus time advances while the Quest is running. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 69 | Confirm the compact and full Quest views show the same active state. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 70 | Pause manually and confirm Focus time stops advancing. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 71 | Resume and confirm the same Quest continues rather than creating a duplicate. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 72 | Enter a Quest goal and confirm it persists while active. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 73 | Add a Quest Log note and confirm it remains attached to the active Quest. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 74 | Enter Commit Details and a Final Quest Log Entry. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 75 | Start Meditation and confirm it is classified separately from Focus. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 76 | Resume ordinary Focus after Meditation. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 77 | Complete the Quest through the normal completion path. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 78 | Confirm exactly one finalized session is added to history. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 79 | Confirm Markdown and machine-readable Chronicle files are written. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 80 | Confirm Focus, Meditation, Approved Break, and Idle/Unverified totals are not conflated. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 81 | Confirm rewards are based on eligible finalized work rather than wall-clock duration. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 82 | Restart Unity and confirm the completed Quest remains in history. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 83 | Start another Quest, abandon it, and confirm the abandonment is explicit and does not masquerade as completion. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 84 | Start another Quest, restart Unity while it is active, and confirm recovery pauses/preserves it safely. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 85 | Complete the recovered Quest and confirm it finalizes once. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |

## Phase G: Exercise Idle Detection and External Activity
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 86 | Start a Quest with a short idle timeout. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 87 | Leave Unity and all configured external tools inactive until the idle warning appears. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 88 | Confirm the warning occurs once per expected idle event rather than repeatedly stacking. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 89 | Allow the idle threshold to pass and confirm the Quest enters the correct paused/unverified state. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 90 | Return and acknowledge activity; confirm the idle interval is recorded. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 91 | Confirm idle time does not become Focus time or generate Focus rewards. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 92 | Create or select the included Aseprite External Activity Profile on Windows, or document platform limitation elsewhere. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 93 | With the configured external tool in the foreground and recent input, confirm Unity-project-focus loss does not create false idle time. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 94 | Leave the configured tool open in the background and confirm it does not qualify as active work. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 95 | Use an unconfigured application and confirm it does not qualify. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 96 | Confirm external activity appears as Chronicle evidence without adding Focus seconds. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 97 | Open or reveal a Chronicle and confirm the intentional external-action grace period prevents an immediate false idle pause. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |

## Phase H: Verify Wellness and Approved Breaks
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 98 | Trigger each configured wellness reminder at least once. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 99 | Choose Acknowledge Only and confirm no break, XP, or character benefit is claimed. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 100 | Choose Snooze and confirm the reminder is delayed without creating a completed action. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 101 | Choose Take Approved Break and confirm the active Quest pauses. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 102 | Wait at least 80 percent of the planned break, resume, and confirm Completed Break XP and the intended wellness benefit are granted once. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 103 | Repeat an Approved Break but resume early and confirm `Break Ended Early` with no benefit. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 104 | Exceed the approved duration and confirm extra time is Idle/Unverified rather than Approved Break. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 105 | Confirm Meditation is not reclassified as an Approved Break. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 106 | Confirm quiet-hours/day reminders respect their configured schedule. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 107 | Confirm reminder sounds obey the global audio ownership rules. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 108 | Complete the Quest and verify all wellness acknowledgments and breaks in the Chronicle. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |

## Phase I: Validate Quest Profiles, Contracts, Parties, and Focus Stages
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 109 | Create a reusable Quest Profile asset and configure clear QA values. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 110 | Create a Quest Contract with project, objective, reward, and completion data. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 111 | Assign or select the Contract as permitted by rank. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 112 | Start a Quest from the Profile and Contract. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 113 | Confirm the active Quest stores snapshots so later asset edits do not rewrite historical truth. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 114 | Configure multiple Focus Stages and confirm stage order and transitions. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 115 | Confirm warning/audio cues fire for stage transitions without stacking. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 116 | Complete a stage ahead of, at, and behind configured pace across separate test runs. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 117 | Confirm cascading pace bonuses follow the documented rules. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 118 | Confirm party or participant information, when used, persists in the Quest record. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 119 | Complete the Contract and confirm its state and reward are finalized once. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 120 | Attempt to complete or assign the Contract using an unauthorized account and confirm denial. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 121 | Edit the source Contract after completion and confirm the prior Chronicle remains unchanged. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |

## Phase J: Test Quest Log, Git, Media, and Voice Evidence
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 122 | Use a Unity project that is a valid Git repository for this phase. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 123 | Open Quest Log and Git and refresh repository status. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 124 | Confirm the current branch and HEAD commit are reported correctly. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 125 | Make an unstaged change and confirm it appears after refresh. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 126 | Stage and commit a harmless QA file through the DeverQuest workflow where permitted. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 127 | Confirm the commit succeeds and the Chronicle can reference the resulting commit. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 128 | Try an empty commit message and confirm validation prevents an invalid action. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 129 | Confirm Git observation does not continuously perform heavy full-status scans while hidden. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 130 | Attach an existing small media file to an active Quest. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 131 | Confirm DeverQuest copies it into the dated protected Media folder. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 132 | Unlink the attachment and confirm the copied file is not silently deleted. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 133 | When a microphone is available and permission is granted, record and attach a short voice memo. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 134 | Cancel a voice recording and confirm no corrupt attachment is added. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 135 | Reload scripts during recording and confirm recording is cancelled safely. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 136 | Complete the Quest and verify media metadata in the Chronicle. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |

## Phase K: Verify Chronicles, Integrity, History, and Shared Guild Records
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 137 | Open the latest finalized Chronicle from Rewards & History. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 138 | Compare displayed totals against the Markdown and JSON records. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 139 | Confirm the integrity hash validates before modification. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 140 | Make a controlled copy, alter a hashed field, and confirm integrity validation reports the modification. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 141 | Restore the original record and confirm validation succeeds. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 142 | Create a legacy/unhashed test record only in the disposable test set and verify its classification. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 143 | Exercise correction/review workflow as an authorized account. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 144 | Confirm corrections are append-only/auditable rather than silent destructive edits. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 145 | Filter history by a known range and export CSV. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 146 | Export JSON and compare record count with the filtered view. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 147 | Enable Shared Guild records using the disposable shared repository. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 148 | Validate the repository and capture the result. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 149 | Complete and publish a Quest automatically. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 150 | Confirm the record appears under `Records/<Account>/<date>/`. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 151 | Confirm the latest Adventurer snapshot appears under `Adventurers/`. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 152 | Use Publish Last Quest and confirm an existing session is not duplicated. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 153 | Test daily ranking caps with controlled records and confirm raw time remains visible while eligible ranked time is capped. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 154 | Confirm modified, suspiciously long, excessive-idle, or excessive-frequency records are excluded or flagged as documented. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 155 | Confirm ordinary Member permissions cannot rewrite the externally protected repository in the intended deployment. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |

## Phase L: Test Rewards, Inventory, Shop, Trading, and Compensation Preview
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 156 | Complete a short eligible Quest and confirm XP, coin, and configured rewards are granted once. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 157 | Confirm Meditation, Idle/Unverified time, and unfinalized Quests do not create Focus rewards. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 158 | Open the Shop with an account that has enough QA coin. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 159 | Purchase a normal consumable or inventory item. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 160 | Purchase or grant an equipment item and confirm ownership metadata. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 161 | Confirm rare-or-better and equipment items receive durable ownership identity. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 162 | Attempt to trade an eligible unbound item to another enabled QA account. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 163 | Confirm the item enters escrow and is removed from sender availability. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 164 | Accept the offer as recipient and confirm ownership transfer and ledger entry. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 165 | Create another offer, reject it, and reclaim it as sender. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 166 | Create another offer and cancel it as sender. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 167 | Attempt to trade a bound, non-tradeable, or redemption item and confirm rejection. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 168 | Create a Redemption request and confirm it requires leadership approval. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 169 | Approve it as authorized leadership and confirm the reward is reserved/charged once. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 170 | Mark it delivered with a QA confirmation reference and confirm the administrative trail. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 171 | Confirm DeverQuest does not claim an external reward was delivered automatically. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 172 | Configure a Compensation Preview policy as CEO or Boss. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 173 | Confirm Meditation and Idle/Unverified time never qualify. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 174 | Confirm an active Quest is excluded until finalized. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 175 | Confirm modified Chronicles are excluded. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 176 | Toggle legacy-record policy and verify the effect on a disposable legacy record. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 177 | Export the filtered preview and verify the planning-only disclaimer. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 178 | Confirm local rate data is not written into daily Chronicles or shared Guild snapshots. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |

## Phase M: Exercise Character, Rules, Equipment, Companions, Combat, Survival, and Encumbrance
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 179 | Create a character using generated Ancestry, Class, Faith, and Identity catalogs. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 180 | Confirm invalid or ineligible combinations are blocked. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 181 | Confirm starting attributes, Department, HP, Mana, traits, languages, and loadout match selected assets. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 182 | Grant and equip a QA weapon and defensive item. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 183 | Confirm equipment bonuses and typed affinities affect derived character state once. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 184 | Teach a Spell and an Attack Technique. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 185 | Assign a Class-linked Ability Profile. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 186 | Recruit an eligible Companion and set it active. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 187 | Confirm only one active Companion joins deterministic combat. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 188 | Run a normal deterministic encounter and record raw and final typed damage. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 189 | Verify resistance halves, vulnerability doubles, immunity prevents, and absorption heals in controlled test encounters. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 190 | Verify resistance plus vulnerability cancels to normal damage. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 191 | Confirm duplicate defenses do not stack unexpectedly. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 192 | Confirm Companion role behavior occurs as documented: striker, guardian, support, or controller. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 193 | Confirm Companion HP, loyalty, battle count, victories, XP, and level persist after editor restart. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 194 | Run one shortened Survival expedition with multiple waves. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 195 | Confirm wave progression, par rewards, weighted loot, and exit behavior. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 196 | Reduce character health to the configured danger threshold and confirm the low-health safety pause. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 197 | Test Attempt Flee, Homeward Passage, and Guild Wagon paths where available. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 198 | Confirm defeat/recovery cannot duplicate rewards or loot. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 199 | Add physical coin or heavy items until encumbered and confirm the expected penalty/state. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 200 | Exchange coin denominations and confirm total value is preserved exactly. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 201 | Confirm combat and RPG outcomes never create Focus time by themselves. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |

## Phase N: Stress the Shared Editor Audio Transport
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 202 | Create a playlist with at least three clips and distinct, recognizable audio. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 203 | Play, Next, Previous, Stop, and Play at normal pace. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 204 | Rapidly alternate Next and Previous at least twenty times. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 205 | Rapidly alternate Play and Stop at least twenty times. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 206 | Confirm only one long-form clip is audible at every step. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 207 | Confirm Stop produces silence with no hidden short loop. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 208 | Pause and resume in the middle of a track. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 209 | Let a track end and confirm automatic advancement. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 210 | Test Repeat Off, Repeat One, Repeat All, and weighted Shuffle. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 211 | Start playlist music, then ambience, and confirm playlist ownership stops. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 212 | Start playlist music again and confirm ambience ownership clears. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 213 | Use Next Ambience repeatedly and confirm no hidden playlist resumes. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 214 | Trigger Idle Warning, Victory, and Level Up cues during music. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 215 | Confirm each cue interrupts once and music resumes near its captured sample position. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 216 | Trigger several cues rapidly and confirm the newest replaces the previous cue without layering. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 217 | Press Stop during a cue and confirm both cue and music remain silent. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 218 | Pause during a cue and confirm silence; resume and confirm one music track continues. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 219 | Trigger assembly reload while audio is playing and confirm cleanup. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 220 | Close Unity while audio is playing and confirm no orphaned preview sound remains. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |

## Phase O: Inspect Interface, Performance, and Accessibility Basics
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 221 | Open each workspace: Quest, Quest Log & Git, Character, Guild Hall, Rewards & History, Audio & Wellness, and Settings. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 222 | Confirm only the selected workspace renders its expensive panels. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 223 | Leave a non-live workspace open and confirm it does not repaint continuously without cause. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 224 | Leave Quest or Quest Log visible and confirm live timers update smoothly. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 225 | Resize the DeverQuest window from narrow to wide and look for clipping or unreachable controls. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 226 | Test the compact/full-view transitions. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 227 | Use Unity light and dark editor themes and confirm critical text remains readable. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 228 | Use keyboard navigation where Unity IMGUI permits and record inaccessible critical actions. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 229 | Confirm destructive or irreversible actions have clear labels and context. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 230 | Confirm empty states explain the next action instead of showing a silent blank panel. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 231 | Confirm long names and notes do not break layout. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 232 | Open history with a large disposable record set and observe responsiveness. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 233 | Observe Console allocations/errors during ordinary timer use for at least ten minutes. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 234 | Confirm background Git observation uses lightweight periodic checks rather than constant full status. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |

## Phase P: Migration, Failure Injection, Cleanup, and Release Verdict
| # | Test | Result | Bug / evidence |
|---:|---|---|---|
| 235 | Create a backup copy of a known-good older DeverQuest profile and Chronicle set. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 236 | Install 0.30.2 over the supported prior package in a disposable migration project. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 237 | Confirm profile, accounts, Adventurers, inventory, Companions, Contracts, settings, and Chronicles remain intact. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 238 | Confirm stable IDs migrate without losing character names or progression. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 239 | Make the Chronicle root temporarily unavailable and confirm failure is reported without corrupting active state. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 240 | Make the Shared Guild repository unavailable and confirm local finalization remains understandable and retryable. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 241 | Deny microphone permission and confirm voice-memo failure is safe and explanatory. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 242 | Use a non-Git project and confirm Git features fail gracefully without blocking the timer. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 243 | Remove or invalidate an optional content asset and confirm the relevant panel reports the problem without crashing the window. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 244 | Reload scripts during an active Quest and confirm recovery behavior. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 245 | Run Release Readiness Check after all tests. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 246 | Confirm no Quest remains active before archiving evidence. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 247 | Export the final QA Chronicles, history exports, screenshots, Console log, and bug list. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 248 | Restore any changed file or folder permissions. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 249 | Delete only disposable QA data and preserve the original package and evidence archive. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 250 | Review every Blocker, Critical, and Major issue with an owner and disposition. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 251 | Record the final verdict: PASS, CONDITIONAL PASS, or FAIL. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |
| 252 | Obtain tester and release-owner sign-off. | `[ ] PASS` `[ ] FAIL` `[ ] BLOCKED` `[ ] N/A` | |


## Cross-cutting invariants

These truths must hold throughout the expedition:

1. A second button press must not duplicate a Quest, reward, trade, redemption, publication, battle result, attachment, or audio owner.
2. Focus time must come only from eligible active Quest time.
3. Meditation, Approved Break, Idle/Unverified, and external evidence must remain distinguishable.
4. Finalized historical snapshots must not be rewritten merely because a ScriptableObject changes later.
5. Administrative authority must be checked by service logic, not only hidden buttons.
6. A failed disk, Git, microphone, audio, or shared-repository operation must leave recoverable state and a useful error.
7. Local reset language must not imply that every EditorPrefs-backed subsystem or Chronicle file was deleted when it was not.
8. A Chronicle hash can detect a changed file, but it must not be described as a secret signature or proof of authorship.
9. Compensation Preview must remain visibly planning-only.
10. RPG activity must never manufacture professional time records.

## Defect ledger

| Bug ID | Severity | Phase/test | Summary | Reproduction confidence | Owner | Disposition |
|---|---|---|---|---|---|---|
| | | | | | | |
| | | | | | | |
| | | | | | | |

## Final release gates

| Gate | Pass criteria | Result |
|---|---|---|
| Installation | Clean import and compilation | |
| Core records | No loss, false time, or duplicate finalization | |
| Recovery | Active Quest survives reload/restart safely | |
| Authority | Rank boundaries enforced | |
| Chronicle | Human and machine records agree and validate | |
| Audio | One shared owner; no layered or orphaned clips | |
| Migration | Supported prior data preserved | |
| Failure safety | Optional integration failures do not corrupt core work | |
| Documentation | Setup, limitations, and known issues match observed behavior | |

## Verdict

- [ ] **PASS:** all release gates pass; no unresolved Blocker, Critical, or Major defects.
- [ ] **CONDITIONAL PASS:** no Blocker or Critical defects; each remaining Major issue has an approved written disposition, workaround, owner, and target version.
- [ ] **FAIL:** any Blocker or Critical remains, or the evidence is insufficient to trust the release.

### Sign-off

| Role | Name | Decision | Date | Signature / reference |
|---|---|---|---|---|
| Tester | | | | |
| Release owner | | | | |
| Guild administrator | | | | |

### Quest completion reward

The reward for Quest 1 is not fictional coin. It is a defensible release decision, a preserved evidence archive, and permission to call the next build a finished product rather than an optimistic rumor.
