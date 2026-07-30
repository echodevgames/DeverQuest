//----- DeverQuestSession.cs START -----

using System;
using System.Collections.Generic;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestSessionState
    {
        None = 0,
        Running = 1,
        Paused = 2,
        Completed = 3
    }

    [Serializable]
    internal sealed class DeverQuestCommitEntry
    {
        public string entryId = string.Empty;
        public string comment = string.Empty;
        public string branch = string.Empty;
        public string commitHash = string.Empty;
        public long createdUtcTicks;
        public double focusedSecondsAtEntry;

        public void Sanitize()
        {
            entryId = entryId?.Trim() ?? string.Empty;
            comment = comment?.Trim() ?? string.Empty;
            branch = branch?.Trim() ?? string.Empty;
            commitHash = commitHash?.Trim() ?? string.Empty;
            focusedSecondsAtEntry =
                Math.Max(0d, focusedSecondsAtEntry);
        }
    }

    [Serializable]
    internal sealed class DeverQuestSession
    {
        public string sessionId = string.Empty;
        public string developerName = string.Empty;
        public string projectName = string.Empty;
        public string taskName = string.Empty;
        public string category = string.Empty;
        public string goal = string.Empty;

        public DeverQuestSessionState state =
            DeverQuestSessionState.None;

        public long startedUtcTicks;
        public long lastStateChangeUtcTicks;
        public long completedUtcTicks;

        public double accumulatedFocusedSeconds;
        public double accumulatedPausedSeconds;
        public bool pausedByEditorShutdown;
        public string pauseReason = string.Empty;
        public string closingNotes = string.Empty;
        public List<DeverQuestCommitEntry> commitEntries =
            new List<DeverQuestCommitEntry>();

        public bool timecardWriteSucceeded;
        public bool timecardWriteAttempted;
        public string timecardPath = string.Empty;
        public string timecardWriteError = string.Empty;

        public bool IsActive =>
            state == DeverQuestSessionState.Running ||
            state == DeverQuestSessionState.Paused;

        public void Sanitize()
        {
            sessionId = sessionId?.Trim() ?? string.Empty;
            developerName = developerName?.Trim() ?? string.Empty;
            projectName = projectName?.Trim() ?? string.Empty;
            taskName = taskName?.Trim() ?? string.Empty;
            category = category?.Trim() ?? string.Empty;
            goal = goal?.Trim() ?? string.Empty;
            pauseReason = pauseReason?.Trim() ?? string.Empty;
            closingNotes = closingNotes?.Trim() ?? string.Empty;
            timecardPath = timecardPath?.Trim() ?? string.Empty;
            timecardWriteError = timecardWriteError?.Trim() ?? string.Empty;

            if (commitEntries == null)
            {
                commitEntries =
                    new List<DeverQuestCommitEntry>();
            }

            foreach (DeverQuestCommitEntry entry in commitEntries)
            {
                entry?.Sanitize();
            }

            accumulatedFocusedSeconds =
                Math.Max(0d, accumulatedFocusedSeconds);

            accumulatedPausedSeconds =
                Math.Max(0d, accumulatedPausedSeconds);
        }
    }
}

//----- DeverQuestSession.cs END -----
