# Changelog

## [0.11.0] - 2026-07-30

### Added

- Git installation and Unity-project repository detection.
- Current repository branch, HEAD hash, and staged, modified, and untracked
  file counts.
- Automatic branch and hash values for Quest Log notes.
- Active-quest monitoring for commits created in external Git tools.
- Commit Staged Changes action using the Quest Log message.
- Separately confirmed Stage All and Commit action.
- Plain-language explanations of branches, staging, commits, and hashes.
- Git errors displayed without losing the developer's pending message.

### Changed

- Make Camp is now Meditate.
- Camped Time is now Meditation Time.
- Manual notes are explicitly labeled as not creating Git commits.

### Safety

- DeverQuest never stages files through the staged-commit action.
- Stage All always requires a confirmation describing its full scope.
- Git commands run only against the repository containing the Unity project.
- Successful Git commits are recorded with their real branch and hash.

## [0.10.1] - 2026-07-30

### Fixed

- Complete Quest now opens the closing-notes panel even when the quest was
  previously paused by idle or project-focus detection.
- Finalization takes priority over the forced return-acknowledgment gate.
- Beginning finalization safely clears a pending idle acknowledgment without
  resuming focused time.

## [0.10.0] - 2026-07-30

### Added

- Forced return acknowledgment after idle and project-focus pauses.
- Unity-project-focused and system-wide input activity modes.
- Last-used Project and Department defaults.
- Optional locked project name for project-contained installations.
- Comma-separated focus check-in schedules such as 15, 30, 45, 60.
- Initial medieval quest terminology throughout the active workflow.

### Fixed

- Focus, paused, and idle timers now have dedicated non-overlapping rows.
- External input no longer keeps a quest active in project-focused mode.
- Opening a ledger or leaving Unity no longer falsely advances a playlist.
- Track completion requires elapsed clip duration and a confirmed stop.

### Changed

- Newest quests appear first in generated daily ledgers.
- Session actions are presented as Accept Quest, Make Camp, Resume Quest,
  Complete Quest, and Abandon Quest.

## [0.9.0] - 2026-07-30

### Added

- Daily-goal progress including the current active session.
- Current streak, longest streak, and total goal-day statistics.
- Compact session dashboard with timer, controls, music, wellness, and wallet.
- System, Dark, Light, and Echo Neon visual accents.
- Preferences for editor notifications, sounds, and reminder window auto-open.
- Complete user guide and Milestone 9 validation checklist.

### Changed

- User-facing Category terminology is now Department.
- Existing serialized `category` fields remain intact for compatibility.
- New CSV exports and generated Markdown timecards use Department labels.
- Reminder delivery respects the user's notification preferences.

## [0.8.0] - 2026-07-30

### Added

- Daily history browser backed by timecard sidecars.
- All-time, today, 7-day, 30-day, and custom date ranges.
- Project and category text filters.
- Overall focused, paused, session, commit, break, and reward totals.
- Weekly, project, and category summaries.
- Reward-wallet balance, earned, and spent statistics.
- CSV and JSON exports for the current filtered view.
- Open and reveal controls for individual timecards.

## [0.7.0] - 2026-07-30

### Added

- Reusable DeverQuest playlist assets.
- AudioClip track lists.
- Play, pause, resume, stop, previous, and next controls.
- Shuffle and Off/All/One repeat modes.
- Automatic track advancement.
- Preview-volume control when supported by the installed Unity editor.
- Selected-playlist persistence.
- Optional session start, pause, resume, end, and discard integration.

## [0.6.0] - 2026-07-30

### Added

- Persistent reward wallet.
- Configurable focused-work reward blocks.
- Default Game Time and Other Fun Time rewards.
- Custom reward categories.
- Reward spending with balance validation.
- Unfinished work carries toward the next reward block.
- Configurable daily focused-work goal and category bonuses.
- Idempotent session processing to prevent duplicate awards.
- Reward transactions in session timecards.

## [0.5.0] - 2026-07-30

### Added

- Recurring focus check-ins, movement breaks, hydration, and exercise prompts.
- Lunch and dinner reminders.
- Quiet-hours warning.
- Configurable reminder intervals and meal times.
- Snooze, dismiss, and pause-for-break actions.
- Persistent reminder scheduling through script recompilation.
- Wellness-event records in session timecards.

## [0.4.0] - 2026-07-30

### Added

- Timestamped in-session commit journal.
- Optional branch and commit-hash fields.
- Closing notes during session finalization.
- Automatic daily Markdown timecards.
- Multiple completed sessions per daily timecard.
- Recalculated focused, paused, session, and commit totals.
- JSON sidecar data used to regenerate reliable daily reports.
- Timecard write-status feedback and manual retry.

## [0.3.0] - 2026-07-30

### Added

- Windows keyboard-and-mouse idle detection.
- Configurable idle threshold and warning countdown.
- Automatic session pause when the threshold is reached.
- Visible idle warning and automatic-pause notifications.
- Pause-reason tracking.
- Configurable exceptions for Play Mode, compilation, asset importing, and
  player builds.

## [0.2.0] - 2026-07-30

### Added

- Deliberate focus-session creation.
- Project, task, category, and goal fields.
- Start, pause, resume, end, and discard controls.
- Live focused-time and paused-time display.
- Active-session persistence through editor recompilation.
- Automatic pause when Unity closes normally.
- Last-session summary retained until the next session begins.

## [0.1.0] - 2026-07-30

### Added

- Initial editor-only package.
- Dockable DeverQuest window.
- First-time profile setup.
- Timecard root and developer-folder validation.
- Per-user persistent settings.
