# DeverQuest Milestone 9 — Goals, Streaks, and Polish

Version: 0.9.0

## What changed

- Daily goal progress, current streak, longest streak, and goal-day count.
- Compact dashboard for an active work session.
- System, Dark, Light, and Echo Neon accents.
- Notification, sound, and reminder auto-open preferences.
- Visible Category terminology migrated to Department without rewriting data.
- Full user guide, compatibility notes, and troubleshooting.

## Validation checklist

1. Install the package and open Tools > DeverQuest > Developer Companion.
2. Confirm old sessions appear under Department and old history still loads.
3. Set a small daily goal and verify completed work appears in the progress bar.
4. Start a session and verify the progress bar advances before finalization.
5. Enter Compact View; test music, pause/resume, and End Session.
6. Finalize from Compact View and confirm the daily timecard is updated.
7. Switch among all four themes and reopen the window to verify persistence.
8. Disable notification sounds and auto-open; close the window and verify a
   reminder does not force it open or beep.
9. Export CSV and confirm the header says Department.
10. Confirm a newly generated Markdown timecard says Department.

## Compatibility note

The stored session member remains named `category`, intentionally. Renaming a
Unity JSON field would orphan existing history. Milestone 9 changes display and
export terminology only, so all prior sidecars remain readable.
