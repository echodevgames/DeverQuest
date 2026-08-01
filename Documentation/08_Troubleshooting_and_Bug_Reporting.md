# Troubleshooting and Bug Reporting

## First rule: preserve evidence

Before resetting, reinstalling, deleting preferences, editing a Chronicle, or moving folders, capture:

- Unity version;
- DeverQuest package version;
- operating system;
- current account/rank and project;
- active Quest state;
- Console log and stack trace;
- exact steps and button order;
- affected Chronicle/file paths;
- screenshots or short recording;
- whether the issue survives editor restart.

Copy affected files before modifying them.

## Package does not compile

1. Clear the Console and capture the first error, not only downstream errors.
2. Confirm package version 0.30.2.
3. Confirm Unity 2022.3 LTS or supported later version.
4. Remove stale embedded copies or duplicate package folders.
5. Reinstall from the original tarball.
6. Confirm no local assembly-definition edits conflict.
7. Report the full file, line, error code, and message.

## Window does not open

- Use Tools > DeverQuest > Developer Companion.
- Resolve compilation errors first.
- Try a direct workspace menu.
- Check for exceptions during `OnEnable`/GUI rendering.
- Preserve EditorPrefs and active session before resetting layout or profile.

## Unexpected recovered Quest

1. Do not start another Quest.
2. Capture goal, start time, account, and state.
3. Confirm whether Unity previously reloaded or closed during an active Quest.
4. Resume, complete, or abandon only after verifying identity.
5. Report duplicate or stale recovery with the active-session preference copy when possible.

## Focus time looks wrong

Compare:

- wall-clock start/end;
- Focus intervals;
- Pause intervals;
- Meditation;
- Approved Break;
- Idle/Unverified;
- external activity evidence;
- editor reload/restart time.

False Focus, missing Focus, or category conversion is Critical. Preserve both Markdown and `.deverquest.json` records.

## Chronicle not written

- Confirm the configured root exists and is writable.
- Run Release Readiness Check.
- Check invalid filename characters and path length.
- Check disk space, sync locks, antivirus, and read-only permissions.
- Use the package retry action rather than completing the Quest again.
- Do not manually create a fake record to hide the failure.

## Chronicle integrity fails

1. Copy the original file and related audit/correction files.
2. Do not recompute the hash silently.
3. Check text editor, sync conflict, line-ending conversion, merge, restore, or manual edit history.
4. Use authorized correction/review workflow.
5. Restore a known-good original when appropriate.

## Shared publication fails

- Validate the repository path.
- Confirm folder permissions and availability.
- Confirm local Chronicle finalization succeeded.
- Check for sync conflicts and duplicate session IDs.
- Use Publish Last Quest as a retry.
- Verify retry does not duplicate the record.

## Git panel fails

- Confirm `git` is installed and available to Unity's process environment.
- Confirm the selected path is a repository.
- Run `git status` outside DeverQuest for comparison.
- Check repository locks, hooks, credentials, and line-ending prompts.
- DeverQuest may create a local commit, but normal remote/push authentication remains external.

## Voice memo fails

- Confirm the OS grants microphone permission to Unity Editor.
- Confirm a device appears in Unity.
- Close other exclusive audio applications.
- Use a short test memo.
- Confirm cancellation/reload leaves no corrupt attachment.
- Attach an existing audio file as a fallback when appropriate.

## External activity is not recognized

- Confirm the operating system is supported by the configured provider behavior.
- Match executable process name without `.exe`.
- Verify optional window-title text exactly enough to match but not so broad that unrelated windows qualify.
- Keep the tool foreground and provide recent keyboard/pointer input.
- Re-select/save the External Activity Profile.

## Rewards duplicated or missing

Treat duplication as Critical. Preserve:

- session ID;
- reward transaction IDs;
- wallet before/after;
- last-completed session;
- Chronicle;
- button/retry sequence.

Do not press Complete, Purchase, Accept, Approve, or Publish repeatedly while diagnosing.

## Trade item missing

Inspect the trade lifecycle:

- open escrow;
- accepted ownership transfer;
- rejected awaiting reclaim;
- cancelled return;
- bound/non-tradeable rejection.

Preserve sender and recipient inventory plus ledger before repair.

## Combat is stuck

- Capture active encounter/wave, turn, HP/Mana, effects, Companion, and current action.
- Try only documented exit/recovery controls once.
- Do not grant rewards manually.
- Reload in a project clone to test persistence.
- Report any state where the Quest or character cannot exit safely.

## Multiple audio clips play

1. Press Stop once.
2. Capture the exact Play/Next/Previous/ambience/cue order.
3. Note repeat/shuffle mode and clip lengths.
4. Test whether assembly reload or Unity exit stops it.
5. Treat reproducible layering or orphaned audio as Critical.

Playlist and ambience should never coexist. A cue should temporarily replace long-form audio and restore at most one owner.

## Window is slow

- Identify the selected workspace.
- Measure whether the slowdown occurs only with live Quest/Log views.
- Count Chronicle records and generated assets.
- Check Git repository size and status cost.
- Check shared repository size/network latency.
- Capture profiler/Editor logs if available.
- Confirm inactive workspaces are not continuously repainting.

## Bug report format

Use the included template and provide:

1. concise title;
2. severity;
3. environment;
4. preconditions;
5. exact numbered steps;
6. expected result;
7. actual result;
8. reproducibility rate;
9. impact/data risk;
10. evidence paths;
11. safe workaround;
12. whether a clean profile/project reproduces it.

## Safe reset ladder

Use the least destructive step that can prove the issue:

1. close/reopen DeverQuest window;
2. switch workspace and refresh;
3. stop optional audio/recording;
4. safely pause active Quest;
5. restart Unity;
6. test in a clone/disposable project;
7. reinstall matching package;
8. restore known-good local files/preferences;
9. reset only the affected subsystem with a backup;
10. full local removal only under documented administrator control.
