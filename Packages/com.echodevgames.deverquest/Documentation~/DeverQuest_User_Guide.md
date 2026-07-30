# DeverQuest User Guide

DeverQuest is an editor-only Unity companion for deliberate work sessions,
timecards, wellness, earned rewards, music, goals, and reporting.

## Install or update

Use Unity 2022.3 LTS or newer. Copy `com.echodevgames.deverquest` into the
project's `Packages` folder, or use Package Manager > Add package from disk and
select `package.json`. To update, replace the package folder while Unity is
closed or use your normal package-source workflow. DeverQuest profile settings
and timecards live outside the package, so updating does not erase them.

Open Tools > DeverQuest > Developer Companion.

## First-time setup

Enter a developer name and choose a timecard root. DeverQuest asks before
creating the root or developer subfolder. Configure session defaults, idle
detection, wellness reminders, reward rules, session-aware music, theme, and
notification preferences, then validate the folders.

## Sessions and idle detection

Enter a project, task or milestone, department, and optional objective, then
accept a quest. Make camp and resume manually, or let the optional idle
detector pause after the configured timeout. An automatic idle pause requires
an explicit return acknowledgment before the quest can resume.

Unity Project Focused activity scope pauses after the current Unity process
loses focus, so work in another application or project does not keep the quest
active. System Wide Input preserves the original honor-system behavior and
counts keyboard or mouse input anywhere on the computer. Windows provides
native keyboard and mouse idle time. Other platforms continue working normally
but may report that native idle detection is unavailable.

Play Mode, compilation, asset importing, and player builds can count as active
work so unattended Unity operations do not pause a valid session.

## Commits and timecards

During a session, add journal entries with a comment and optional branch and
hash. Ending a session pauses the clock while you add closing notes. Finalizing
writes or updates that day's Markdown timecard and JSON sidecar in:

`<Timecard Root>/<Developer Name>/`

The sidecar is DeverQuest's reporting data. Keep it beside the Markdown file.
Multiple sessions on the same day are combined into one daily card.
The newest quest is printed first.

## Git integration

When Git is installed and the Unity project is inside a repository, the Quest
Log displays the current branch, HEAD commit hash, and counts for staged,
modified, and untracked files.

- Add Quest Log Note records a productivity note only. It does not change Git.
- Commit Staged Changes commits only files already staged through another Git
  tool. The Quest Log comment becomes the Git commit message.
- Stage All and Commit stages every modified, deleted, and untracked file,
  then commits. It always requires confirmation.

After a successful commit, DeverQuest records the real branch and resulting
hash in the active quest. It also checks for commits made through an external
Git client during an active quest and records newly detected HEAD commits.

A branch is the current development path. Staging selects changes for the next
commit. A commit is a saved repository snapshot. Its hash uniquely identifies
that snapshot. DeverQuest does not push, pull, merge, switch branches, discard
changes, or rewrite history.

Older data uses the serialized field name `category`. Milestone 9 displays it
as Department without rewriting or risking existing records.

## Wellness and notifications

Focus, hydration, movement, exercise, meal, and quiet-hours prompts can be
snoozed, acknowledged, or used to pause for a break. Preferences control
whether DeverQuest shows editor notifications, plays sounds, or opens a closed
window for reminders. Disabling auto-open never disables the underlying timer.
Focus check-ins accept a comma-separated schedule such as `15, 30, 45, 60`.
Leave the schedule empty to use the repeating fallback interval.

## Rewards

Focused work blocks earn minutes in Game Time, Other Fun, and custom reward
categories. A daily goal can award bonuses. Spending subtracts from the
persistent wallet; session processing is protected against duplicate awards.

## Playlists

Create a DeverQuest playlist asset from the player, add AudioClips in its
Inspector, and select it in the window. Controls include play/pause, previous,
next, stop, shuffle, repeat, and volume where the Unity editor exposes preview
audio volume. Session-aware options can play, pause, resume, or stop music with
the work session.

## Goals, streaks, and compact mode

The daily progress bar combines completed timecards with today's active
session. A goal day is completed when its finalized focused time reaches the
configured daily target. The current streak tolerates an unfinished current
day by counting backward from yesterday; completing today extends it.

Compact View keeps the live goal, reminders, music, timer, meditate/resume/end
controls, and reward balances visible in a smaller workspace. Use Full View to
start a new session or access journals, reports, and settings.

## History and exports

History reads JSON sidecars from the developer folder. Filter by date, project,
or department; review daily, weekly, project, department, and reward summaries;
then export the current view as CSV or JSON.

## Themes

System follows Unity's normal label colors. Dark and Light provide explicit
readable accents. Echo Neon uses DeverQuest teal and pink accents. Themes tint
DeverQuest headings and timers; they do not replace the Unity editor skin.

## Backup and privacy

Back up the entire developer timecard folder to preserve Markdown and JSON
together. Profile and wallet preferences use per-user Unity EditorPrefs and are
not committed to the project by default. DeverQuest runs locally and does not
upload timecards, activity, playlists, or personal data.

## Troubleshooting

- No timecard: finalize the session, then inspect the status shown under the
  last completed session. Use Retry Timecard Write if needed.
- Empty history: verify the configured developer folder contains
  `.deverquest.json` files and press Refresh.
- Idle detector unavailable: use manual pause or disable idle detection; the
  rest of DeverQuest remains supported.
- Audio does not play: ensure the playlist contains AudioClips. Unity preview
  audio APIs vary by editor release; DeverQuest shows a warning when playback,
  status, or volume control is unavailable.
- Settings look wrong after an update: choose Reconfigure Profile, review the
  values, and finish setup. This does not delete timecards.
- Work in another application paused the quest: choose System Wide Input if
  that external activity should count. Unity Project Focused is intentionally
  strict.

## Compatibility

The package targets Unity 2022.3 LTS and is editor-only. Timecard data remains
backward-compatible across Milestones 4–9. Runtime builds do not include the
editor window or productivity monitors; the playlist asset type is kept in the
runtime assembly so assets remain valid.
