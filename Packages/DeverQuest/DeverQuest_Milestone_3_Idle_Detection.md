# DeverQuest — Milestone 3: Idle Detection

## Checkpoint Purpose

Automatically pause a running focus session when the developer leaves the
computer without manually pausing.

## Why This Matters

Deliberate sessions are only trustworthy if unattended time cannot silently
inflate focused-work totals. Idle detection protects future timecards and
reward calculations.

## New System

### DeverQuestIdleMonitor

Runs as an editor service and:

- Checks Windows' system-wide last keyboard or mouse input
- Operates only while a deliberate session is running
- Warns before the configured threshold
- Automatically pauses at the threshold
- Records `Idle Detection` as the pause reason
- Resets the warning when activity returns
- Suspends detection during configured development operations

## Default Configuration

- Idle detection: enabled
- Idle timeout: 5 minutes
- Warning: 30 seconds before pause
- Play Mode counts as active work
- Compilation counts as active work
- Asset importing counts as active work
- Player builds count as active work

## Behavior

```text
Running session
      ↓
No keyboard or mouse input
      ↓
Warning threshold reached
      ↓
Visible warning + Unity beep
      ↓
Idle threshold reached
      ↓
Session pauses with reason: Idle Detection
```

Returning to the computer does not automatically resume the session. The
developer must press **Resume**, preserving the deliberate-session rule.

## Exception Behavior

When an enabled exception is active, idle detection is suspended. When the
operation finishes, DeverQuest grants a fresh idle interval before it can
automatically pause.

This prevents a long build or compile from causing an immediate pause as soon
as Unity becomes responsive again.

## Platform Scope

Milestone 3 uses the Windows `GetLastInputInfo` API because the current
development environment is Windows. Non-Windows editors display that idle
detection is unavailable and continue running sessions normally.

Cross-platform native input providers can be added during packaging and polish.

## Setup

1. Open `Tools > DeverQuest > Developer Companion`.
2. Select **Reconfigure Profile**.
3. Enable Idle Detection.
4. Set the timeout and warning duration.
5. Choose which Unity operations count as active work.
6. Validate the profile again.

For quick testing, use:

- Idle timeout: 1 minute
- Warning: 10 seconds

## Test Checklist

### Basic Detection

- [ ] Start a deliberate session.
- [ ] The window displays current input-idle time.
- [ ] Move the mouse and confirm idle time resets.
- [ ] Press a key and confirm idle time resets.

### Warning

- [ ] Set timeout to 1 minute and warning to 10 seconds.
- [ ] Stop providing keyboard and mouse input.
- [ ] A warning appears near 50 seconds.
- [ ] Unity produces a warning beep.
- [ ] Moving the mouse after the warning prevents automatic pause.
- [ ] A future idle period can produce a new warning.

### Automatic Pause

- [ ] Remain idle through the full timeout.
- [ ] Focused time stops increasing.
- [ ] Session state becomes paused.
- [ ] Pause reason displays `Idle Detection`.
- [ ] Returning input does not automatically resume.
- [ ] Press Resume and confirm focused time continues.

### Exceptions

- [ ] Enable Play Mode exception and enter Play Mode.
- [ ] The session does not idle-pause during Play Mode.
- [ ] Exit Play Mode and confirm a fresh idle interval is granted.
- [ ] Trigger compilation and confirm it does not cause an immediate pause.
- [ ] Test importing and builds when practical.

### Disabled State

- [ ] Disable idle detection.
- [ ] Start a session and remain idle beyond the former threshold.
- [ ] The session continues running.

## Goal Line

Milestone 3 passes when real keyboard or mouse inactivity warns the developer,
pauses focused work at the configured threshold, records the reason, and never
resumes without deliberate input.

## Commit-Ready Scope

Suggested commit:

```text
feat(deverquest): add automatic idle detection and pause
```

## Next Checkpoint

Milestone 4 — Commit Journal and Timecards

- Add timestamped commit notes during a session
- Add closing notes at finalization
- Write Markdown timecards into the developer folder
- Append multiple sessions to the same daily timecard
- Recalculate daily focused and paused totals
