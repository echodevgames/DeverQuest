//----- DeverQuestTimecardWriter.cs START -----

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [Serializable]
    internal sealed class DeverQuestDailyRecord
    {
        public string developerName = string.Empty;
        public string localDate = string.Empty;
        public int chronicleIndex = 1;
        public List<DeverQuestSession> sessions =
            new List<DeverQuestSession>();
    }

    internal static class DeverQuestTimecardWriter
    {
        public static bool TryWriteSession(
            DeverQuestProfile profile,
            DeverQuestSession completedSession,
            out string markdownPath,
            out string errorMessage)
        {
            markdownPath = string.Empty;
            errorMessage = string.Empty;

            if (profile == null || completedSession == null)
            {
                errorMessage = "Profile or completed session was missing.";
                return false;
            }

            try
            {
                string developerFolder =
                    DeverQuestPathUtility.GetDeveloperFolder(
                        profile.timecardRootPath,
                        profile.developerName);

                Directory.CreateDirectory(developerFolder);

                DateTime completedLocal =
                    DeverQuestSessionStore.GetLocalCompletionTime(
                        completedSession);

                string dateKey =
                    completedLocal.ToString(
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture);

                string safeDeveloperName =
                    DeverQuestPathUtility.MakeSafeFolderName(
                        profile.developerName);

                int chronicleIndex =
                    DeverQuestChronicleIntegrityService
                        .GetRequestedChronicleIndex(
                            developerFolder, dateKey);
                string baseFileName = BuildBaseFileName(
                    dateKey, safeDeveloperName, chronicleIndex);

                string dataPath = Path.Combine(
                    developerFolder,
                    baseFileName + ".deverquest.json");

                DeverQuestDailyRecord record =
                    LoadRecord(
                        dataPath,
                        profile.developerName,
                        dateKey);
                record.chronicleIndex = chronicleIndex;

                if (ShouldRollOver(profile, dataPath, record,
                        completedSession.sessionId))
                {
                    chronicleIndex =
                        DeverQuestChronicleIntegrityService
                            .StartNewChronicle(
                                developerFolder, dateKey);
                    baseFileName = BuildBaseFileName(
                        dateKey, safeDeveloperName, chronicleIndex);
                    dataPath = Path.Combine(
                        developerFolder,
                        baseFileName + ".deverquest.json");
                    record = CreateRecord(
                        profile.developerName, dateKey);
                    record.chronicleIndex = chronicleIndex;
                }

                markdownPath = Path.Combine(
                    developerFolder,
                    baseFileName + ".md");

                int existingIndex = record.sessions.FindIndex(
                    session => session != null &&
                               session.sessionId ==
                               completedSession.sessionId);

                if (existingIndex >= 0)
                {
                    record.sessions[existingIndex] = completedSession;
                }
                else
                {
                    record.sessions.Add(completedSession);
                }

                record.sessions = record.sessions
                    .Where(session => session != null)
                    .OrderByDescending(
                        session => session.startedUtcTicks)
                    .ToList();

                string json = JsonUtility.ToJson(record, true);
                File.WriteAllText(dataPath, json, Encoding.UTF8);

                string markdown = BuildMarkdown(
                    record,
                    completedLocal.Date,
                    DeverQuestChronicleIntegrityService
                        .LoadCorrections(dataPath));

                File.WriteAllText(
                    markdownPath,
                    markdown,
                    new UTF8Encoding(false));

                if (profile.chronicleIntegrityEnabled)
                {
                    DeverQuestChronicleIntegrityService.Seal(
                        dataPath,
                        profile.developerName,
                        existingIndex >= 0
                            ? "Chronicle Rewritten"
                            : "Quest Appended",
                        completedSession.sessionId);
                }

                return true;
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
                return false;
            }
        }

        private static DeverQuestDailyRecord LoadRecord(
            string dataPath,
            string developerName,
            string dateKey)
        {
            if (!File.Exists(dataPath))
            {
                return CreateRecord(developerName, dateKey);
            }

            string json = File.ReadAllText(dataPath);

            DeverQuestDailyRecord record =
                JsonUtility.FromJson<DeverQuestDailyRecord>(json);

            if (record == null)
            {
                throw new InvalidDataException(
                    "The existing DeverQuest daily-data file could not be read.");
            }

            record.developerName = developerName;
            record.localDate = dateKey;

            if (record.sessions == null)
            {
                record.sessions = new List<DeverQuestSession>();
            }

            return record;
        }

        private static DeverQuestDailyRecord CreateRecord(
            string developerName,
            string dateKey)
        {
            return new DeverQuestDailyRecord
            {
                developerName = developerName,
                localDate = dateKey,
                chronicleIndex = 1,
                sessions = new List<DeverQuestSession>()
            };
        }

        internal static bool TryRegenerate(
            string dataPath,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                DeverQuestDailyRecord record =
                    JsonUtility.FromJson<DeverQuestDailyRecord>(
                        File.ReadAllText(dataPath));
                DateTime date = DateTime.ParseExact(
                    record.localDate, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture);
                string suffix = ".deverquest.json";
                string markdownPath = dataPath.EndsWith(suffix)
                    ? dataPath.Substring(
                        0, dataPath.Length - suffix.Length) + ".md"
                    : Path.ChangeExtension(dataPath, ".md");
                File.WriteAllText(markdownPath,
                    BuildMarkdown(record, date,
                        DeverQuestChronicleIntegrityService
                            .LoadCorrections(dataPath)),
                    new UTF8Encoding(false));
                return true;
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
                return false;
            }
        }

        private static string BuildMarkdown(
            DeverQuestDailyRecord record,
            DateTime localDate,
            IReadOnlyList<DeverQuestCorrection> corrections)
        {
            double totalFocused = record.sessions.Sum(
                session => session.accumulatedFocusedSeconds);

            double totalPaused = record.sessions.Sum(
                session => session.accumulatedPausedSeconds);

            int totalCommits = record.sessions.Sum(
                session => session.commitEntries?.Count ?? 0);

            int totalBreaks = record.sessions.Sum(
                session => session.wellnessEvents?.Count(
                    wellnessEvent =>
                        wellnessEvent.action == "Break Started") ?? 0);

            double totalRewardMinutes = record.sessions.Sum(
                session => session.rewardTransactions?.Where(
                    transaction => transaction.minutes > 0d)
                    .Sum(transaction => transaction.minutes) ?? 0d);
            long totalCopper = record.sessions.Sum(
                session => session.rewardTransactions?.Where(
                    transaction => transaction.copper > 0L)
                    .Sum(transaction => transaction.copper) ?? 0L);
            long totalExperience = record.sessions.Sum(
                session => session.rewardTransactions?.Where(
                    transaction => transaction.experience > 0L)
                    .Sum(transaction => transaction.experience) ?? 0L);
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("# DeverQuest Daily Timecard");
            builder.AppendLine();
            builder.AppendLine($"**Developer:** {Escape(record.developerName)}  ");
            builder.AppendLine(
                $"**Adventurer:** {Escape(string.IsNullOrWhiteSpace(adventurer.characterName) ? record.developerName : adventurer.characterName)}  ");
            builder.AppendLine(
                $"**Class:** {Escape(adventurer.characterClass)} · " +
                $"**Level:** {adventurer.level} · " +
                $"**Guild Rank:** {Escape(adventurer.guildRank)}  ");
            builder.AppendLine(
                $"**Character Rules:** " +
                $"HP {adventurer.currentHitPoints}/" +
                $"{adventurer.maximumHitPoints} · " +
                $"AC {DeverQuestRulesService.ArmorClass(adventurer)} · " +
                $"Proficiency +" +
                $"{DeverQuestRulesService.ProficiencyBonus(adventurer.level)}  ");
            builder.AppendLine($"**Date:** {localDate:MMMM d, yyyy}  ");
            builder.AppendLine(
                $"**Chronicle:** {Math.Max(1, record.chronicleIndex)}  ");
            builder.AppendLine($"**Sessions:** {record.sessions.Count}");
            builder.AppendLine();
            builder.AppendLine("## Daily Totals");
            builder.AppendLine();
            builder.AppendLine($"- **Focused Work:** {FormatDuration(totalFocused)}");
            builder.AppendLine($"- **Paused Time:** {FormatDuration(totalPaused)}");
            builder.AppendLine($"- **Commit Entries:** {totalCommits}");
            builder.AppendLine($"- **Breaks Taken:** {totalBreaks}");
            builder.AppendLine(
                $"- **Coin Earned:** {DeverQuestAdventurerService.FormatCoins(totalCopper)}");
            builder.AppendLine(
                $"- **Experience Earned:** {totalExperience} XP");
            builder.AppendLine(
                $"- **Ending Coin Purse:** " +
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.copperBalance));
            if (totalRewardMinutes > 0d)
            {
                builder.AppendLine(
                    $"- **Legacy Reward Time:** " +
                    $"{totalRewardMinutes:0.#} minutes");
            }
            builder.AppendLine();
            builder.AppendLine("---");

            for (int index = 0; index < record.sessions.Count; index++)
            {
                AppendSession(builder, record.sessions[index], index + 1);
            }

            AppendCorrections(builder, corrections);

            builder.AppendLine();
            builder.AppendLine(
                "_Generated by DeverQuest Developer Companion._");

            return builder.ToString();
        }

        private static void AppendCorrections(
            StringBuilder builder,
            IReadOnlyList<DeverQuestCorrection> corrections)
        {
            if (corrections == null || corrections.Count == 0)
            {
                return;
            }
            builder.AppendLine();
            builder.AppendLine("## Chronicle Corrections");
            builder.AppendLine();
            builder.AppendLine(
                "_Corrections are appended; original Quest records remain unchanged._");
            foreach (DeverQuestCorrection correction in corrections)
            {
                builder.AppendLine();
                builder.AppendLine(
                    $"### {Escape(correction.sessionTitle)} — " +
                    $"{Escape(correction.status)}");
                builder.AppendLine();
                builder.AppendLine(
                    $"- **Requested By:** {Escape(correction.requestedBy)}");
                builder.AppendLine(
                    $"- **Requested:** {Escape(correction.requestedUtc)}");
                builder.AppendLine(
                    $"- **Reason:** {Escape(correction.reason)}");
                builder.AppendLine(
                    $"- **Corrected Record:** " +
                    $"{EscapeMultiline(correction.correctedValue)}");
                if (!string.IsNullOrWhiteSpace(correction.reviewedBy))
                {
                    builder.AppendLine(
                        $"- **Reviewed By:** " +
                        $"{Escape(correction.reviewedBy)}");
                }
            }
        }

        private static string BuildBaseFileName(
            string dateKey,
            string safeDeveloperName,
            int chronicleIndex)
        {
            string baseName =
                $"{dateKey}_{safeDeveloperName}_Timecard";
            return chronicleIndex <= 1
                ? baseName
                : $"{baseName}_Chronicle_{chronicleIndex:00}";
        }

        private static bool ShouldRollOver(
            DeverQuestProfile profile,
            string dataPath,
            DeverQuestDailyRecord record,
            string sessionId)
        {
            if (record.sessions.Any(
                    item => item != null &&
                            item.sessionId == sessionId))
            {
                return false;
            }
            if (record.sessions.Count >= profile.chronicleMaxSessions)
            {
                return true;
            }
            return File.Exists(dataPath) &&
                   new FileInfo(dataPath).Length >=
                   profile.chronicleMaxKilobytes * 1024L;
        }

        private static void AppendSession(
            StringBuilder builder,
            DeverQuestSession session,
            int sessionNumber)
        {
            DateTime start =
                DeverQuestSessionStore.GetLocalStartTime(session);

            DateTime completed =
                DeverQuestSessionStore.GetLocalCompletionTime(session);

            builder.AppendLine();
            builder.AppendLine(
                $"## Session {sessionNumber} — {Escape(session.taskName)}");
            builder.AppendLine();
            builder.AppendLine($"- **Project:** {Escape(session.projectName)}");
            builder.AppendLine($"- **Department:** {Escape(session.category)}");
            if (session.usesQuestProfile)
            {
                builder.AppendLine(
                    $"- **Quest Profile:** " +
                    $"{Escape(session.questProfileName)}");
                builder.AppendLine(
                    $"- **Profile Spoils:** " +
                    $"{DeverQuestAdventurerService.FormatCoins(session.questBaseCopper)} " +
                    $"+ {session.questBaseExperience} XP base; " +
                    $"{DeverQuestAdventurerService.FormatCoins(session.questCopperPerWorkBlock)} " +
                    $"+ {session.questExperiencePerWorkBlock} XP per " +
                    $"{session.questWorkBlockMinutes}m block");
            }
            if (session.usesQuestContract)
            {
                builder.AppendLine(
                    $"- **Quest Contract:** " +
                    $"{Escape(session.questContractTitle)}");
                builder.AppendLine(
                    $"- **Contract Creator:** " +
                    $"{Escape(session.questContractCreator)}");
                builder.AppendLine(
                    $"- **Assigned Adventurer:** " +
                    $"{Escape(session.questContractAssignee)}");
                builder.AppendLine(
                    $"- **Priority:** " +
                    $"{Escape(session.questContractPriority)}");
                builder.AppendLine(
                    $"- **Due Date:** " +
                    $"{Escape(string.IsNullOrWhiteSpace(session.questContractDueDate) ? "Unscheduled" : session.questContractDueDate)}");
            }
            builder.AppendLine($"- **Started:** {start:h:mm tt}");
            builder.AppendLine($"- **Ended:** {completed:h:mm tt}");
            builder.AppendLine(
                $"- **Focused Work:** {FormatDuration(session.accumulatedFocusedSeconds)}");
            builder.AppendLine(
                $"- **Paused Time:** {FormatDuration(session.accumulatedPausedSeconds)}");
            double classifiedPaused =
                session.meditationSeconds +
                session.approvedBreakSeconds +
                session.idleUnverifiedSeconds;
            double legacyUnclassified =
                Math.Max(0d,
                    session.accumulatedPausedSeconds -
                    classifiedPaused);
            builder.AppendLine(
                $"- **Time Classification:** Focused " +
                $"{FormatDuration(session.accumulatedFocusedSeconds)} · " +
                $"Meditation {FormatDuration(session.meditationSeconds)} · " +
                $"Approved Break " +
                $"{FormatDuration(session.approvedBreakSeconds)} · " +
                $"Idle/Unverified " +
                $"{FormatDuration(session.idleUnverifiedSeconds)}" +
                (legacyUnclassified > 0d
                    ? $" · Legacy Unclassified " +
                      $"{FormatDuration(legacyUnclassified)}"
                    : string.Empty));

            if (!string.IsNullOrWhiteSpace(session.goal))
            {
                builder.AppendLine();
                builder.AppendLine("### Goal");
                builder.AppendLine();
                builder.AppendLine(EscapeMultiline(session.goal));
            }

            if (session.usesQuestContract &&
                !string.IsNullOrWhiteSpace(
                    session.questContractDeliverables))
            {
                builder.AppendLine();
                builder.AppendLine("### Contract Deliverables");
                builder.AppendLine();
                builder.AppendLine(
                    EscapeMultiline(
                        session.questContractDeliverables));
            }

            if (session.usesQuestContract &&
                (!string.IsNullOrWhiteSpace(
                     session.questEncounterProfileId) ||
                 !string.IsNullOrWhiteSpace(
                     session.questEncounterNotes)))
            {
                builder.AppendLine();
                builder.AppendLine("### Reserved Encounter");
                builder.AppendLine();
                if (!string.IsNullOrWhiteSpace(
                        session.questEncounterProfileId))
                {
                    builder.AppendLine(
                        $"- **Encounter Profile:** " +
                        $"{Escape(session.questEncounterProfileId)}");
                }
                if (!string.IsNullOrWhiteSpace(
                        session.questEncounterNotes))
                {
                    builder.AppendLine(
                        EscapeMultiline(
                            session.questEncounterNotes));
                }
            }

            builder.AppendLine();
            builder.AppendLine("### Commit Journal");
            builder.AppendLine();

            if (session.commitEntries == null ||
                session.commitEntries.Count == 0)
            {
                builder.AppendLine("- No commit entries recorded.");
            }
            else
            {
                foreach (DeverQuestCommitEntry entry in session.commitEntries)
                {
                    string entryType =
                        string.IsNullOrWhiteSpace(entry.entryType)
                            ? "Legacy Entry"
                            : entry.entryType;
                    DateTime created = new DateTime(
                            entry.createdUtcTicks,
                            DateTimeKind.Utc)
                        .ToLocalTime();

                    builder.Append(
                        $"- **{created:h:mm tt}** " +
                        $"[{Escape(entryType)}] " +
                        $"`+{FormatDuration(entry.focusedSecondsAtEntry)}` — " +
                        Escape(entry.comment));

                    if (!string.IsNullOrWhiteSpace(entry.branch))
                    {
                        builder.Append(
                            $" | Branch: `{EscapeCode(entry.branch)}`");
                    }

                    if (!string.IsNullOrWhiteSpace(entry.commitHash))
                    {
                        builder.Append(
                            $" | Commit: `{EscapeCode(entry.commitHash)}`");
                    }

                    builder.AppendLine();
                }
            }

            builder.AppendLine();
            builder.AppendLine("### Closing Notes");
            builder.AppendLine();

            builder.AppendLine(
                string.IsNullOrWhiteSpace(session.closingNotes)
                    ? "No closing notes recorded."
                    : EscapeMultiline(session.closingNotes));

            if (session.wellnessEvents != null &&
                session.wellnessEvents.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("### Wellness Journal");
                builder.AppendLine();

                foreach (DeverQuestWellnessEvent wellnessEvent
                         in session.wellnessEvents)
                {
                    DateTime created = new DateTime(
                            wellnessEvent.createdUtcTicks,
                            DateTimeKind.Utc)
                        .ToLocalTime();

                    builder.AppendLine(
                        $"- **{created:h:mm tt}** — " +
                        $"{wellnessEvent.type}: " +
                        $"{Escape(wellnessEvent.action)}");
                }
            }

            if (session.rewardTransactions != null &&
                session.rewardTransactions.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("### Reward Journal");
                builder.AppendLine();

                foreach (DeverQuestRewardTransaction transaction
                         in session.rewardTransactions)
                {
                    if (transaction.copper != 0L ||
                        transaction.experience != 0L)
                    {
                        string coinSign =
                            transaction.copper >= 0L ? "+" : "-";
                        string coinText =
                            DeverQuestAdventurerService.FormatCoins(
                                Math.Abs(transaction.copper));
                        string levelText =
                            transaction.endingLevel >
                            transaction.startingLevel
                                ? $" · Level {transaction.startingLevel} → " +
                                  $"{transaction.endingLevel}"
                                : string.Empty;
                        builder.AppendLine(
                            $"- **{transaction.transactionType}:** " +
                            $"{coinSign}{coinText} · " +
                            $"+{transaction.experience} XP" +
                            $"{levelText} — {Escape(transaction.note)}");
                    }
                    else
                    {
                        string sign =
                            transaction.minutes >= 0d ? "+" : string.Empty;
                        builder.AppendLine(
                            $"- **{transaction.categoryName}:** " +
                            $"{sign}{transaction.minutes:0.#} minutes " +
                            $"({transaction.transactionType}) — " +
                            Escape(transaction.note));
                    }
                }
            }

            builder.AppendLine();
            builder.AppendLine("---");
        }

        private static string FormatDuration(double totalSeconds)
        {
            TimeSpan duration =
                TimeSpan.FromSeconds(Math.Max(0d, totalSeconds));

            return $"{(int)duration.TotalHours}h " +
                   $"{duration.Minutes}m " +
                   $"{duration.Seconds}s";
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty)
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("|", "\\|");
        }

        private static string EscapeMultiline(string value)
        {
            return (value ?? string.Empty)
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("|", "\\|");
        }

        private static string EscapeCode(string value)
        {
            return (value ?? string.Empty).Replace("`", "'");
        }
    }
}

//----- DeverQuestTimecardWriter.cs END -----
