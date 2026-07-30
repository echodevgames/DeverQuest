# DeverQuest — Milestone 2: Deliberate Focus Sessions

## Checkpoint Purpose

Add intentional work sessions to DeverQuest. Work time only begins when the
developer presses **Start Focus Session**.

## Why This Matters

This establishes the source of truth for future idle detection, commit entries,
timecards, rewards, wellness reminders, and history reports.

## New Scripts

### DeverQuestSession

Serializable session data containing:

- Unique session ID
- Developer
- Project
- Task or milestone
- Category
- Goal
- State
- Start and completion timestamps
- Accumulated focused time
- Accumulated paused time

### DeverQuestSessionStore

Owns the active session and provides:

- Start
- Pause
- Resume
- Complete
- Discard
- Live focused duration
- Live paused duration
- Persistence through script recompilation
- Automatic pause when Unity closes normally

## Updated Window

When no session is active, the window displays:

- Project field
- Task or milestone field
- Category field
- Session goal
- Start Focus Session button

During a session, the window displays:

- Current state
- Large focused-time clock
- Paused-time clock
- Session details
- Pause or Resume
- End Session
- Discard Session

## Timing Rules

- Time counts only after Start Focus Session is pressed.
- Running time is accumulated as focused time.
- Paused time is tracked separately.
- Resuming never includes paused time in focused work.
- Script recompilation does not lose or reset the session.
- Closing Unity normally pauses an active session.
- Reopening Unity requires the developer to resume deliberately.
- Discarding deletes the unfinished session.
- Ending stores a completed-session summary for Milestone 4.

## Setup

No scene or Inspector setup is needed.

1. Install or update the package.
2. Open `Tools > DeverQuest > Developer Companion`.
3. Complete Milestone 1 profile setup if required.
4. Enter a project and task.
5. Press Start Focus Session.

## Test Checklist

### Starting

- [ ] Start is disabled when Project is blank.
- [ ] Start is disabled when Task is blank.
- [ ] Category and Goal may be left blank.
- [ ] Starting displays the live session dashboard.
- [ ] Focused time begins at zero and increases.

### Pause and Resume

- [ ] Pause stops the focused timer.
- [ ] Paused time begins increasing.
- [ ] Resume stops the paused timer.
- [ ] Resume continues focused time from its previous value.
- [ ] Repeated pause/resume cycles preserve accurate totals.

### Compilation Recovery

- [ ] Begin a session.
- [ ] Cause Unity to recompile a script.
- [ ] Reopen the DeverQuest window if necessary.
- [ ] The same active session remains.
- [ ] Focused time was not reset.

### Unity Restart

- [ ] Begin a session.
- [ ] Close Unity normally.
- [ ] Reopen the project.
- [ ] The session is present and paused.
- [ ] Work time did not accumulate while Unity was closed.
- [ ] Resume must be pressed deliberately.

### Ending

- [ ] End Session opens a finalization confirmation.
- [ ] Continue Working leaves the session unchanged.
- [ ] Finalize ends the session.
- [ ] The completed summary displays project, task, time, and completion date.
- [ ] A new session can be started afterward.

### Discarding

- [ ] Discard requires confirmation.
- [ ] Keep Session cancels the discard.
- [ ] Confirming removes the active session.
- [ ] Discarded work does not become the last completed session.

## Goal Line

Milestone 2 passes when a deliberate session survives pause/resume cycles,
script recompilation, and a clean Unity restart without counting paused or
closed-editor time as focused work.

## Commit-Ready Scope

Suggested commit:

```text
feat(deverquest): add deliberate focus session tracking
```

## Next Checkpoint

Milestone 3 — Idle Detection

- Detect activity inside the Unity Editor
- Warn before the idle threshold is reached
- Automatically pause an unattended running session
- Keep Play Mode, compilation, importing, and builds configurable
