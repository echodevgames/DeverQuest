# DeverQuest Milestone 11 — Git Integration

Version: 0.11.0

## Delivered

- Git and repository detection for the current Unity project.
- Current branch, HEAD hash, and working-tree counts.
- Automatic Quest Log branch/hash values.
- External commit detection while a quest is active.
- Commit Staged Changes.
- Confirmed Stage All and Commit.
- Automatic recording of successful Git commits.
- Beginner-friendly Git vocabulary.
- Meditate terminology.

## Safety model

Commit Staged Changes never stages files. Stage All and Commit describes its
scope and requires confirmation. DeverQuest does not push, pull, merge, switch
branches, reset, restore, discard files, amend, rebase, or rewrite Git history.

If a commit fails, the pending message remains available. If Stage All succeeds
but the commit fails, those files remain staged so the developer can inspect
and resolve the problem without losing work.

## Validation checklist

1. Install the package and start a Quest.
2. Verify the Git panel shows the expected branch and current short hash.
3. Compare staged, modified, and untracked counts with your normal Git client.
4. Type a note and use Add Quest Log Note. Confirm Git history does not change.
5. Stage one safe test file through your normal Git client.
6. Refresh DeverQuest and use Commit Staged Changes.
7. Confirm Git contains the new commit and the Quest Log contains its real hash.
8. Make another safe change and test Stage All and Commit after reviewing the
   confirmation.
9. Create a commit through your normal Git client while the Quest is active.
10. Wait about five seconds and confirm DeverQuest records the external commit.
11. Meditate and verify Meditation Time increments before resuming.
12. Complete the Quest and verify every Git-backed entry appears in the ledger.

## Terminology

- **Branch:** the development path currently checked out.
- **Staged:** changes selected for the next commit.
- **Commit:** a saved snapshot of staged repository changes.
- **Hash:** Git's unique identifier for a commit.
- **Quest Log note:** a DeverQuest record that does not modify Git.

## Next direction

The next planned system is Session Profiles and rank permissions. The coin
economy remains scheduled after the shared profile and authority foundation so
reward rules can be controlled consistently.
