# DeverQuest Developer Companion

DeverQuest is an editor-only Unity productivity utility for deliberate focus
sessions, developer timecards, break reminders, rewards, and music playlists.

This package currently contains:

- **Milestone 1 — Package and Profile Foundation**
- **Milestone 2 — Deliberate Focus Sessions**
- **Milestone 3 — Idle Detection**
- **Milestone 4 — Commit Journal and Timecards**

## Requirements

- Unity 2022.3 LTS or newer

## Installation

1. Copy `com.echodevgames.deverquest` into the project's `Packages` folder.
2. Return to Unity and allow the package to compile.
3. Open **Tools > DeverQuest > Developer Companion**.
4. Complete the first-time setup.

You can also install it through Unity's Package Manager by choosing
**Add package from disk...** and selecting this package's `package.json`.

## Current features

- Reusable editor-only Unity package
- Dockable DeverQuest window
- First-time developer profile
- Project-aware default timecard folder
- Folder existence validation
- Confirmation before creating folders
- Per-user timecard subfolder
- Persistent editor settings
- Reset and reconfigure controls
- Named deliberate focus sessions
- Project, task, category, and goal details
- Start, pause, resume, end, and discard controls
- Live focused and paused timers
- Session recovery through Unity script compilation
- Automatic pause during a normal Unity shutdown
- Last-completed-session summary
- Configurable Windows idle detection
- Warning before automatic idle pause
- Play Mode, compilation, importing, and build exceptions
- Recorded pause reasons
- Timestamped commit journal with optional branch and hash
- Closing notes at session finalization
- Automatic daily Markdown timecards
- Multiple sessions and recalculated daily totals
- Timecard write retry

Settings are stored in Unity's per-user `EditorPrefs`, so they are not committed
to a project's repository. Timecards default to a `DeverQuestTimecards` folder
beside the project's `Assets` folder.

## Planned milestones

1. Package and Profile Foundation
2. Deliberate Focus Sessions — complete
3. Idle Detection — complete
4. Commit Journal and Timecards — complete
5. Break and Wellness System
6. Reward Economy
7. Playlist Player
8. History and Reporting
9. Goals, Streaks, and Polish
