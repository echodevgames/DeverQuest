# DeverQuest — Milestone 1: Package and Profile Foundation

## Checkpoint Purpose

Build the reusable editor-only foundation that every future DeverQuest system
will depend on.

This checkpoint establishes:

- The Unity Package Manager structure
- A dockable DeverQuest editor window
- The first-time setup workflow
- Persistent per-user settings
- Timecard root-folder validation
- A separate developer folder

## Why This Matters

The timer, idle detector, commit journal, rewards, playlists, and reports all
need a stable developer identity and storage location. Completing this
checkpoint prevents each later system from inventing its own configuration or
file path.

## Package Structure

```text
com.echodevgames.deverquest/
├── package.json
├── README.md
├── CHANGELOG.md
└── Editor/
    ├── EchoDevGames.DeverQuest.Editor.asmdef
    ├── DeverQuestProfile.cs
    ├── DeverQuestSettingsStore.cs
    ├── DeverQuestPathUtility.cs
    └── DeverQuestWindow.cs
```

## System Responsibilities

### DeverQuestProfile

Stores the current developer's name, timecard root, default focus length, and
idle-detection defaults.

### DeverQuestSettingsStore

Loads and saves the profile through `EditorPrefs`. This keeps personal settings
out of the game's repository.

### DeverQuestPathUtility

Builds safe folder names, identifies the recommended project-level timecard
root, and creates approved directories.

### DeverQuestWindow

Provides the first-time setup and completed-profile dashboard.

## Installation

1. Copy `com.echodevgames.deverquest` into the Unity project's `Packages`
   folder.
2. Allow Unity to compile.
3. Open `Tools > DeverQuest > Developer Companion`.
4. Enter the developer name.
5. Select or accept the recommended timecard root.
6. Confirm folder creation when prompted.

## Inspector and Scene Setup

None. DeverQuest is editor-only and does not require:

- A scene object
- A prefab
- A MonoBehaviour
- An application bootstrap
- A runtime singleton

## Test Checklist

### Package

- [ ] Unity recognizes the package without compiler errors.
- [ ] `Tools > DeverQuest > Developer Companion` appears.
- [ ] The window can be docked and resized.

### First-Time Setup

- [ ] A blank developer name prevents setup completion.
- [ ] The recommended root resolves beside the project's `Assets` folder.
- [ ] Browse can select an existing folder.
- [ ] Missing root folder produces a confirmation dialog.
- [ ] Cancel prevents folder creation and setup completion.
- [ ] Approve creates the root folder.
- [ ] A missing developer folder produces its own confirmation dialog.
- [ ] The developer folder uses a filesystem-safe name.
- [ ] Setup completes only after both folders exist.

### Persistence

- [ ] Close and reopen the DeverQuest window.
- [ ] Close and reopen Unity.
- [ ] Developer profile remains configured.
- [ ] Reconfigure returns to setup without deleting folders.
- [ ] Reset clears settings without deleting folders.

### Project Safety

- [ ] No scripts are added to a scene.
- [ ] No DeverQuest profile data appears inside `Assets`.
- [ ] Reset does not delete existing user files.

## Goal Line

Milestone 1 passes when a new user can install the package, open the window,
configure their name and timecard location, approve any required folders, close
Unity, reopen it, and find their profile intact.

## Commit-Ready Scope

Suggested commit:

```text
feat(deverquest): add package and developer profile foundation
```

## Next Checkpoint

Milestone 2 — Deliberate Focus Sessions

- Start a named work session
- Count focused time
- Pause and resume deliberately
- Preserve active state through script compilation
- End or discard a session
- Prepare session data for the future timecard journal
