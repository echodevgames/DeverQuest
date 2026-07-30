# DeverQuest — Milestone 4: Commit Journal and Timecards

## Checkpoint Purpose

Turn deliberate focus sessions into useful development records by adding a
timestamped commit journal, closing notes, and automatically generated daily
Markdown timecards.

## New Features

### Commit Journal

Each active session now accepts:

- Commit details
- Optional branch
- Optional commit hash

Every entry records:

- Local timestamp
- Focused time reached when the entry was added
- Comment
- Branch
- Commit hash

Entries persist through recompilation and may be removed before finalization.

### Finalization

End Session now opens an inline finalization panel. The timer pauses while the
developer reviews the session and enters closing notes.

Available actions:

- Finalize and Write Timecard
- Continue Working

Continuing resumes the timer if it was running before finalization began.

### Daily Timecards

Finalizing writes:

```text
DeverQuestTimecards/
└── Jesse_Adams/
    ├── 2026-07-30_Jesse_Adams_Timecard.md
    └── 2026-07-30_Jesse_Adams_Timecard.deverquest.json
```

The Markdown file is the human-readable timecard. The JSON sidecar preserves
structured session data so DeverQuest can safely regenerate the daily totals
when more sessions are completed.

## Timecard Contents

- Developer
- Date
- Session count
- Total focused time
- Total paused time
- Total commit entries
- Project and task for each session
- Category
- Start and end times
- Goal
- Commit journal
- Closing notes

Multiple sessions completed on the same date update the same timecard.

## Failure Handling

If the timecard cannot be written:

- The completed session remains saved.
- The error is displayed.
- Retry Timecard Write is available.
- Existing timecard data is not intentionally deleted.

## Test Checklist

### Commit Entries

- [ ] Start a session.
- [ ] Add a commit comment.
- [ ] Entry shows its comment and focused-time position.
- [ ] Add an entry with branch and hash.
- [ ] Recompile scripts and confirm both entries remain.
- [ ] Remove one entry.
- [ ] Confirm the correct entry was removed.
- [ ] Blank commit comments cannot be added.

### Finalization

- [ ] End Session pauses a running timer.
- [ ] Closing-notes field appears.
- [ ] Continue Working closes the panel and resumes.
- [ ] End again and enter closing notes.
- [ ] Finalize completes the session.

### First Daily Timecard

- [ ] Markdown timecard is created in the developer folder.
- [ ] JSON sidecar is created beside it.
- [ ] Developer and date are correct.
- [ ] Focused and paused totals are correct.
- [ ] Project, task, category, and goal are correct.
- [ ] Commit comments are present.
- [ ] Branch and hash appear when provided.
- [ ] Closing notes are present.

### Multiple Sessions

- [ ] Start and finish a second session on the same date.
- [ ] A second Markdown timecard is not created.
- [ ] The existing timecard contains both sessions.
- [ ] Daily totals equal both sessions combined.
- [ ] Total commit count includes both sessions.

### Recovery

- [ ] Temporarily make the configured folder unavailable or read-only.
- [ ] Finalize a test session.
- [ ] A write error is displayed.
- [ ] Restore folder access.
- [ ] Retry Timecard Write succeeds.
- [ ] Reveal Timecard opens the completed file.

## Goal Line

Milestone 4 passes when session notes and commit entries persist, finalization
writes a readable Markdown timecard, and multiple sessions reliably combine
into one daily report with accurate totals.

## Commit-Ready Scope

Suggested commit:

```text
feat(deverquest): add commit journal and daily timecards
```

## Next Checkpoint

Milestone 5 — Break and Wellness System

- Recurring work warnings
- Break prompts
- Exercise reminders
- Meal reminders
- Hydration reminders
- Quiet hours
