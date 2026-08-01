# Contributing

## Contribution goals

Contributions should improve the reliability, clarity, testability, accessibility, or maintainability of DeverQuest without weakening the truth of Quest records.

## Before coding

- Search existing issues and documentation.
- State the problem, user impact, and storage/migration consequences.
- Identify whether the change affects time, rewards, authority, integrity, ownership, audio, or privacy.
- For a major feature, obtain scope approval before implementation.

## Branch and change discipline

1. Create a focused branch.
2. Keep generated/user data out of commits.
3. Preserve Unity `.meta` files.
4. Avoid unrelated formatting churn.
5. Add or update documentation with behavior changes.
6. Add a Quest 1 regression item or automated test for every fixed defect.
7. Rebase/merge according to project policy and resolve serialization conflicts carefully.

## Code quality

- Target the supported C# language level for Unity 2022.3.
- Avoid preview-only syntax.
- Fully qualify ambiguous Unity/System types where needed.
- Keep Editor APIs in the Editor assembly.
- Enforce permissions and invariants in services.
- Make durable operations idempotent.
- Unregister Editor callbacks and release audio/microphone resources.
- Avoid expensive work on every IMGUI repaint.
- Version persistent data and provide migrations.
- Use stable content IDs rather than display names.
- Produce actionable errors without leaking secrets.

## Testing expectations

At minimum, run the relevant Quest 1 phases. Changes to these areas require the listed high-risk tests:

| Area | Required regression |
|---|---|
| Session/timing | start, pause, idle, reload, complete, duplicate-click |
| Persistence | migration, malformed/missing storage, restore |
| Rewards/economy | idempotency, insufficient funds, ownership |
| Accounts | full rank/assigned-project matrix |
| Chronicle | write, integrity, correction, export |
| Shared records | publication retry and duplicate prevention |
| Audio | rapid transport, cue interruption, reload cleanup |
| Combat | typed responses, defeat/recovery, duplicate reward |
| UI/performance | each workspace, narrow layout, idle repaint behavior |

## Pull request description

Include:

- summary and motivation;
- affected systems/files;
- persistence/schema changes;
- migration/rollback plan;
- privacy/security impact;
- tests run and evidence;
- screenshots for interface changes;
- documentation changed;
- known limitations.

## Content contributions

Only submit original or properly licensed content. Include source/license information for every external asset. Avoid third-party trademarks, copyrighted lore, or ripped game audio/art.

## Release ownership

A merged change is not automatically a release. The release owner controls version bump, changelog, package archive, checksum, Quest 1 verdict, tag, and distribution.
