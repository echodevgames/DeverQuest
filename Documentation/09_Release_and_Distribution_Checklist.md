# Release and Distribution Checklist

## Release identity

| Field | Value |
|---|---|
| Package name | `com.echodevgames.deverquest` |
| Version | |
| Unity minimum | |
| Git commit/tag | |
| Package SHA-256 | |
| Release owner | |
| Quest 1 run ID | |

## Scope gate

- [ ] Release scope is frozen.
- [ ] Deferred systems remain deferred.
- [ ] Every change since the last tested package is listed.
- [ ] No undocumented schema or EditorPrefs key change.
- [ ] No production content added without rights review.

## Build/package gate

- [ ] `package.json` is valid JSON.
- [ ] Package name, display name, version, Unity minimum, description, author, and dependencies are correct.
- [ ] Runtime and Editor assembly definitions are valid.
- [ ] Editor-only APIs are not referenced by Runtime assembly code.
- [ ] No generated project assets or user data are accidentally packed.
- [ ] Documentation is included under an appropriate package documentation folder or release bundle.
- [ ] Tarball root layout installs correctly through Package Manager.
- [ ] Checksum is generated from the final tarball.

## Compile gate

- [ ] Clean disposable Unity 2022.3 project imports with no red errors.
- [ ] Existing supported project upgrades without red errors.
- [ ] Release Readiness Check reports the correct version.
- [ ] No warnings indicate missing required assets or broken serialization.

## Quest 1 gate

- [ ] Full test run completed.
- [ ] No unresolved Blocker or Critical defect.
- [ ] Every Major has an approved disposition.
- [ ] Core timing/recovery repeated after final code change.
- [ ] Rapid audio stress repeated after final code change.
- [ ] Migration repeated after final schema/persistence change.
- [ ] Final readiness report captured.

## Data gate

- [ ] Active-session recovery verified.
- [ ] Markdown and JSON Chronicles agree.
- [ ] Integrity and correction behavior verified.
- [ ] Reward, trade, purchase, redemption, and publication mutations are idempotent.
- [ ] Backup and restore drill passes.
- [ ] Clean uninstall/data-retention language is accurate.

## Authority/security gate

- [ ] CEO/Boss/Project Leader/Member matrix tested.
- [ ] Unauthorized service mutations denied.
- [ ] Shared repository permission guidance current.
- [ ] No real secrets, credentials, rates, or personal media in package/demo content.
- [ ] Security document and privacy boundaries reviewed.

## Documentation gate

- [ ] README current.
- [ ] Quick Start current.
- [ ] User and Admin guides current.
- [ ] Architecture manuscript matches code.
- [ ] Known limitations and scope lock current.
- [ ] Troubleshooting includes new failure modes.
- [ ] Changelog and release notes current.
- [ ] License and support contacts decided.

## Distribution gate

- [ ] Release tarball archived immutably.
- [ ] SHA-256 published with release.
- [ ] Git tag points to exact source.
- [ ] Release notes list breaking changes, migration, known issues, and rollback.
- [ ] Installation and update instructions tested from a clean download.
- [ ] Support/bug-report channel is active.

## Post-release observation

- [ ] Monitor install/compile reports.
- [ ] Monitor false time, duplication, corruption, permission, and audio incidents first.
- [ ] Triage documentation defects separately from code defects.
- [ ] Freeze feature additions until high-risk production workflows have evidence.
- [ ] Schedule the first patch review.

## Release verdict

- [ ] Ship
- [ ] Ship with documented conditions
- [ ] Hold

Decision, rationale, and approvers:
