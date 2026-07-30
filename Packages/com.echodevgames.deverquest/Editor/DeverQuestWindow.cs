//----- DeverQuestWindow.cs START -----

using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal sealed class DeverQuestWindow : EditorWindow
    {
        private const float MinimumWindowWidth = 430f;
        private const float MinimumWindowHeight = 440f;

        private Vector2 scrollPosition;
        private GUIStyle titleStyle;
        private GUIStyle subtitleStyle;
        private GUIStyle wrappedLabelStyle;
        private GUIStyle timerStyle;

        private string newProjectName = string.Empty;
        private string newTaskName = string.Empty;
        private string newCategory = "Programming";
        private string newGoal = string.Empty;
        private string commitComment = string.Empty;
        private string commitBranch = string.Empty;
        private string commitHash = string.Empty;
        private string closingNotes = string.Empty;
        private bool showFinalization;
        private bool resumeIfFinalizationCancelled;
        private readonly Dictionary<string, double> rewardSpendMinutes =
            new Dictionary<string, double>();
        private string newRewardCategoryName = string.Empty;
        private double newRewardMinutesPerBlock = 5d;
        private double newRewardDailyBonus = 10d;
        private string rewardMessage = string.Empty;
        private bool historyFoldout;
        private DeverQuestHistoryRange historyRange =
            DeverQuestHistoryRange.Last7Days;
        private string historyProjectFilter = string.Empty;
        private string historyCategoryFilter = string.Empty;
        private string historyStartDate =
            DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd");
        private string historyEndDate =
            DateTime.Now.ToString("yyyy-MM-dd");
        private string historyMessage = string.Empty;
        private string focusScheduleText = string.Empty;
        private DeverQuestGitStatus gitStatus;
        private string gitMessage = string.Empty;

        [MenuItem("Tools/DeverQuest/Developer Companion")]
        public static void Open()
        {
            DeverQuestWindow window =
                GetWindow<DeverQuestWindow>("DeverQuest");

            window.minSize =
                new Vector2(MinimumWindowWidth, MinimumWindowHeight);

            window.Show();
        }

        internal static void ShowIdleWarning(int secondsRemaining)
        {
            ShowConfiguredNotification(
                $"Still working? Idle pause in about " +
                $"{secondsRemaining} seconds.",
                4d);
        }

        internal static void ShowIdlePaused()
        {
            ShowConfiguredNotification(
                "Quest entered meditation because no activity was detected.",
                6d);
        }

        internal static void ShowWellnessReminder(string title)
        {
            ShowConfiguredNotification($"DeverQuest: {title}", 6d);
        }

        private static void ShowConfiguredNotification(
            string message,
            double duration)
        {
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;
            DeverQuestWindow[] windows =
                Resources.FindObjectsOfTypeAll<DeverQuestWindow>();
            DeverQuestWindow window =
                windows.Length > 0 ? windows[0] : null;

            if (window == null && profile.autoOpenWindowForReminders)
            {
                window = GetWindow<DeverQuestWindow>("DeverQuest");
            }

            if (profile.showEditorNotifications && window != null)
            {
                window.ShowNotification(
                    new GUIContent(message),
                    duration);
            }

            if (profile.notificationSoundsEnabled)
            {
                EditorApplication.Beep();
            }

            window?.Repaint();
        }

        private void OnEnable()
        {
            minSize = new Vector2(
                MinimumWindowWidth,
                MinimumWindowHeight);

            EditorApplication.update -= RepaintWhileSessionRuns;
            EditorApplication.update += RepaintWhileSessionRuns;

            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;
            if (string.IsNullOrWhiteSpace(newProjectName))
            {
                newProjectName = profile.lockProjectName
                    ? profile.lockedProjectName
                    : profile.lastProjectName;
            }
            if (!string.IsNullOrWhiteSpace(profile.lastDepartmentName))
            {
                newCategory = profile.lastDepartmentName;
            }
            focusScheduleText = string.Join(
                ", ",
                profile.focusCheckInScheduleMinutes);
            RefreshGitStatus();
        }

        private void OnDisable()
        {
            EditorApplication.update -= RepaintWhileSessionRuns;
        }

        private void OnGUI()
        {
            BuildStyles();
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;
            ApplyThemeToStyles(profile);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (!profile.setupComplete || !profile.compactMode)
            {
                DrawHeader();
            }
            EditorGUILayout.Space(12f);

            if (profile.setupComplete)
            {
                if (profile.compactMode)
                {
                    DrawCompactDashboard(profile);
                }
                else
                {
                    DrawSessionDashboard();
                }
            }
            else
            {
                DrawFirstTimeSetup();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField("DEVERQUEST", titleStyle);
            EditorGUILayout.LabelField(
                "Developer Companion",
                subtitleStyle);

            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField(
                "Accept quests, build your legend, and earn your downtime.",
                wrappedLabelStyle);
        }

        private void DrawFirstTimeSetup()
        {
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;

            EditorGUILayout.LabelField(
                "First-Time Setup",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Set up your developer profile and choose where DeverQuest " +
                "will write future timecards. No folder will be created " +
                "until you approve it.",
                MessageType.Info);

            EditorGUILayout.Space(8f);

            profile.developerName = EditorGUILayout.TextField(
                new GUIContent(
                    "Developer Name",
                    "Used on timecards and for the developer subfolder."),
                profile.developerName);

            DrawTimecardRootField(profile);

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField(
                "Session Defaults",
                EditorStyles.boldLabel);

            profile.defaultFocusMinutes = EditorGUILayout.IntField(
                new GUIContent("Focus Minutes"),
                profile.defaultFocusMinutes);

            profile.idleDetectionEnabled = EditorGUILayout.Toggle(
                new GUIContent("Idle Detection"),
                profile.idleDetectionEnabled);

            profile.activityScope =
                (DeverQuestActivityScope)EditorGUILayout.EnumPopup(
                    new GUIContent(
                        "Activity Scope",
                        "Project Focused ignores input made in other apps " +
                        "or Unity projects. System Wide uses all PC input."),
                    profile.activityScope);

            using (new EditorGUI.DisabledScope(
                       !profile.idleDetectionEnabled))
            {
                profile.idleTimeoutMinutes = EditorGUILayout.IntField(
                    new GUIContent("Idle Timeout"),
                    profile.idleTimeoutMinutes);

                profile.idleWarningSeconds = EditorGUILayout.IntField(
                    new GUIContent("Warning Seconds"),
                    profile.idleWarningSeconds);

                EditorGUILayout.LabelField(
                    "Count as Active Work",
                    EditorStyles.boldLabel);

                profile.countPlayModeAsActivity =
                    EditorGUILayout.Toggle(
                        "Play Mode",
                        profile.countPlayModeAsActivity);

                profile.countCompilationAsActivity =
                    EditorGUILayout.Toggle(
                        "Compilation",
                        profile.countCompilationAsActivity);

                profile.countAssetImportAsActivity =
                    EditorGUILayout.Toggle(
                        "Asset Importing",
                        profile.countAssetImportAsActivity);

                profile.countBuildsAsActivity =
                    EditorGUILayout.Toggle(
                        "Player Builds",
                        profile.countBuildsAsActivity);
            }

            profile.defaultFocusMinutes =
                Mathf.Max(1, profile.defaultFocusMinutes);

            profile.idleTimeoutMinutes =
                Mathf.Max(1, profile.idleTimeoutMinutes);

            profile.idleWarningSeconds =
                Mathf.Max(0, profile.idleWarningSeconds);

            DrawWellnessSetup(profile);
            DrawRewardSetup(profile);
            DrawPlaylistSetup(profile);
            DrawPolishSetup(profile);
            DrawProjectDefaultsSetup(profile);

            EditorGUILayout.Space(14f);

            using (new EditorGUI.DisabledScope(
                       string.IsNullOrWhiteSpace(profile.developerName) ||
                       string.IsNullOrWhiteSpace(profile.timecardRootPath)))
            {
                if (GUILayout.Button(
                        "Validate Folders and Finish Setup",
                        GUILayout.Height(34f)))
                {
                    TryFinishSetup(profile);
                }
            }
        }

        private void DrawTimecardRootField(DeverQuestProfile profile)
        {
            EditorGUILayout.BeginHorizontal();

            profile.timecardRootPath = EditorGUILayout.TextField(
                new GUIContent(
                    "Timecard Root",
                    "The root folder containing one folder per developer."),
                profile.timecardRootPath);

            if (GUILayout.Button("Browse", GUILayout.Width(70f)))
            {
                string startingPath =
                    Directory.Exists(profile.timecardRootPath)
                        ? profile.timecardRootPath
                        : DeverQuestPathUtility.GetDefaultTimecardRoot();

                string selectedPath = EditorUtility.OpenFolderPanel(
                    "Choose DeverQuest Timecard Root",
                    startingPath,
                    string.Empty);

                if (!string.IsNullOrWhiteSpace(selectedPath))
                {
                    profile.timecardRootPath = selectedPath;
                }
            }

            EditorGUILayout.EndHorizontal();

            if (string.IsNullOrWhiteSpace(profile.timecardRootPath) &&
                GUILayout.Button("Use Recommended Project Folder"))
            {
                profile.timecardRootPath =
                    DeverQuestPathUtility.GetDefaultTimecardRoot();
            }
        }

        private void TryFinishSetup(DeverQuestProfile profile)
        {
            profile.Sanitize();

            if (!EnsureFolderExists(
                    profile.timecardRootPath,
                    "Timecard root folder"))
            {
                return;
            }

            string developerFolder =
                DeverQuestPathUtility.GetDeveloperFolder(
                    profile.timecardRootPath,
                    profile.developerName);

            if (!EnsureFolderExists(
                    developerFolder,
                    "Developer folder"))
            {
                return;
            }

            profile.setupComplete = true;
            DeverQuestSettingsStore.Save();

            EditorUtility.DisplayDialog(
                "DeverQuest Setup Complete",
                "Your profile is ready.\n\nFuture timecards will be written to:\n" +
                developerFolder,
                "Begin");

            Repaint();
        }

        private bool EnsureFolderExists(
            string path,
            string folderDescription)
        {
            if (Directory.Exists(path))
            {
                return true;
            }

            bool approved = EditorUtility.DisplayDialog(
                $"{folderDescription} Not Found",
                $"{folderDescription} does not exist:\n\n{path}\n\n" +
                "Would you like DeverQuest to create it?",
                "Create Folder",
                "Cancel");

            if (!approved)
            {
                return false;
            }

            if (DeverQuestPathUtility.TryCreateDirectory(
                    path,
                    out string errorMessage))
            {
                return true;
            }

            EditorUtility.DisplayDialog(
                "Folder Creation Failed",
                $"DeverQuest could not create:\n\n{path}\n\n{errorMessage}",
                "Close");

            return false;
        }

        private void DrawSessionDashboard()
        {
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;

            EditorGUILayout.LabelField(
                $"Welcome back, {profile.developerName}. Your quest awaits.",
                EditorStyles.boldLabel);

            if (GUILayout.Button(
                    "Compact View",
                    GUILayout.Width(110f)))
            {
                profile.compactMode = true;
                DeverQuestSettingsStore.Save();
                Repaint();
                return;
            }

            EditorGUILayout.Space(8f);

            DrawGoalsAndStreaks(profile);
            DrawWellnessReminder();
            DrawPlaylistPlayer();
            EditorGUILayout.Space(10f);

            if (DeverQuestSessionStore.HasActiveSession)
            {
                DrawActiveSession();
            }
            else
            {
                DrawNewSessionForm(profile);
                DrawLastCompletedSession();
            }

            EditorGUILayout.Space(18f);
            DrawRewardsPanel(profile);
            EditorGUILayout.Space(18f);
            DrawHistoryPanel(profile);
            EditorGUILayout.Space(18f);
            DrawProfileControls(profile);
        }

        private void DrawCompactDashboard(DeverQuestProfile profile)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                $"DEVERQUEST · {profile.developerName}",
                titleStyle);

            if (GUILayout.Button("Full View", GUILayout.Width(90f)))
            {
                profile.compactMode = false;
                DeverQuestSettingsStore.Save();
                Repaint();
            }
            EditorGUILayout.EndHorizontal();

            DrawWellnessReminder();
            DrawGoalsAndStreaks(profile);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            AudioClip track = DeverQuestPlaylistPlayer.CurrentTrack;
            EditorGUILayout.LabelField(
                "Music",
                track == null ? "No track selected" : track.name);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Previous"))
            {
                DeverQuestPlaylistPlayer.Previous();
            }
            if (GUILayout.Button(
                    DeverQuestPlaylistPlayer.State ==
                    DeverQuestPlaybackState.Playing
                        ? "Pause"
                        : "Play"))
            {
                if (DeverQuestPlaylistPlayer.State ==
                    DeverQuestPlaybackState.Playing)
                {
                    DeverQuestPlaylistPlayer.Pause();
                }
                else
                {
                    DeverQuestPlaylistPlayer.Play();
                }
            }
            if (GUILayout.Button("Next"))
            {
                DeverQuestPlaylistPlayer.Next();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (!DeverQuestSessionStore.HasActiveSession)
            {
                EditorGUILayout.HelpBox(
                    "No active quest. Switch to Full View to accept one.",
                    MessageType.Info);
            }
            else
            {
                DeverQuestSession session =
                    DeverQuestSessionStore.ActiveSession;
                EditorGUILayout.LabelField(
                    FormatDuration(
                        DeverQuestSessionStore.GetFocusedSeconds()),
                    timerStyle);
                EditorGUILayout.LabelField(
                    $"{session.projectName} · {session.taskName}",
                    subtitleStyle);

                if (showFinalization)
                {
                    DrawFinalizationPanel(session);
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    if (session.state ==
                        DeverQuestSessionState.Running)
                    {
                        if (GUILayout.Button("Meditate"))
                        {
                            DeverQuestSessionStore.PauseSession();
                        }
                    }
                    else if (GUILayout.Button("Resume"))
                    {
                        DeverQuestSessionStore.ResumeSession();
                    }

                    if (GUILayout.Button("Complete Quest"))
                    {
                        BeginFinalization(session);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField(
                "Reward Wallet",
                EditorStyles.boldLabel);
            foreach (DeverQuestRewardCategory category
                     in DeverQuestRewardService.Wallet.categories)
            {
                EditorGUILayout.LabelField(
                    category.displayName,
                    $"{category.balanceMinutes:0.#} minutes");
            }
        }

        private static void DrawGoalsAndStreaks(
            DeverQuestProfile profile)
        {
            if (!DeverQuestHistoryService.IsLoaded)
            {
                DeverQuestHistoryService.Refresh(profile);
            }

            DeverQuestGoalStatistics statistics =
                DeverQuestHistoryService.BuildGoalStatistics(
                    profile.dailyWorkGoalMinutes);

            double todaySeconds = statistics.TodayFocusedSeconds;
            if (DeverQuestSessionStore.HasActiveSession)
            {
                DeverQuestSession active =
                    DeverQuestSessionStore.ActiveSession;
                if (DeverQuestSessionStore
                        .GetLocalStartTime(active).Date ==
                    DateTime.Today)
                {
                    todaySeconds +=
                        DeverQuestSessionStore.GetFocusedSeconds();
                }
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Daily Decree and Questing Streaks",
                EditorStyles.boldLabel);

            if (profile.dailyWorkGoalMinutes <= 0)
            {
                EditorGUILayout.LabelField(
                    "Daily goal is disabled. Set it in Profile settings.");
            }
            else
            {
                double goalSeconds =
                    profile.dailyWorkGoalMinutes * 60d;
                float progress =
                    Mathf.Clamp01((float)(todaySeconds / goalSeconds));
                Rect rect = GUILayoutUtility.GetRect(
                    18f,
                    18f,
                    GUILayout.ExpandWidth(true));
                EditorGUI.ProgressBar(
                    rect,
                    progress,
                    $"{FormatDuration(todaySeconds)} / " +
                    $"{FormatDuration(goalSeconds)}");
                EditorGUILayout.LabelField(
                    $"Current streak: {statistics.CurrentStreak} day(s) · " +
                    $"Longest: {statistics.LongestStreak} · " +
                    $"Goal days: {statistics.GoalDays}");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private void DrawHistoryPanel(DeverQuestProfile profile)
        {
            historyFoldout = EditorGUILayout.Foldout(
                historyFoldout,
                "History and Reporting",
                true);

            if (!historyFoldout)
            {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (!DeverQuestHistoryService.IsLoaded)
            {
                DeverQuestHistoryService.Refresh(profile);
            }

            EditorGUILayout.BeginHorizontal();

            historyRange =
                (DeverQuestHistoryRange)EditorGUILayout.EnumPopup(
                    "Date Range",
                    historyRange);

            if (GUILayout.Button(
                    "Refresh",
                    GUILayout.Width(72f)))
            {
                DeverQuestHistoryService.Refresh(profile);
                historyMessage = "History refreshed.";
            }

            EditorGUILayout.EndHorizontal();

            if (historyRange == DeverQuestHistoryRange.Custom)
            {
                historyStartDate =
                    EditorGUILayout.TextField(
                        "Start (yyyy-MM-dd)",
                        historyStartDate);

                historyEndDate =
                    EditorGUILayout.TextField(
                        "End (yyyy-MM-dd)",
                        historyEndDate);
            }

            historyProjectFilter =
                EditorGUILayout.TextField(
                    "Project Contains",
                    historyProjectFilter);

            historyCategoryFilter =
                EditorGUILayout.TextField(
                    "Department Contains",
                    historyCategoryFilter);

            if (!TryGetHistoryDateRange(
                    out DateTime? startDate,
                    out DateTime? endDate,
                    out string dateError))
            {
                EditorGUILayout.HelpBox(
                    dateError,
                    MessageType.Error);

                EditorGUILayout.EndVertical();
                return;
            }

            List<DeverQuestHistoryDay> days =
                DeverQuestHistoryService.GetFilteredDays(
                    startDate,
                    endDate,
                    historyProjectFilter,
                    historyCategoryFilter);

            if (!string.IsNullOrWhiteSpace(
                    DeverQuestHistoryService.LastError))
            {
                EditorGUILayout.HelpBox(
                    DeverQuestHistoryService.LastError,
                    MessageType.Error);
            }

            DrawHistorySummary(days);
            DrawNamedHistorySummary(
                "Weekly Summary",
                DeverQuestHistoryService.BuildWeeklySummaries(days));

            DrawNamedHistorySummary(
                "Project Totals",
                DeverQuestHistoryService.BuildProjectSummaries(days));

            DrawNamedHistorySummary(
                "Department Totals",
                DeverQuestHistoryService.BuildCategorySummaries(days));

            DrawWalletStatistics();
            DrawHistoryDays(days);
            DrawHistoryExport(profile, days);

            if (!string.IsNullOrWhiteSpace(historyMessage))
            {
                EditorGUILayout.HelpBox(
                    historyMessage,
                    MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        private static void DrawHistorySummary(
            IReadOnlyList<DeverQuestHistoryDay> days)
        {
            DeverQuestHistorySummary summary =
                DeverQuestHistoryService.BuildSummary(days);

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField(
                "Filtered Totals",
                EditorStyles.boldLabel);

            DrawReadOnlyValue(
                "Days",
                summary.DayCount.ToString());

            DrawReadOnlyValue(
                "Sessions",
                summary.SessionCount.ToString());

            DrawReadOnlyValue(
                "Focused",
                FormatDuration(summary.FocusedSeconds));

            DrawReadOnlyValue(
                "Paused",
                FormatDuration(summary.PausedSeconds));

            DrawReadOnlyValue(
                "Commits",
                summary.CommitCount.ToString());

            DrawReadOnlyValue(
                "Breaks",
                summary.BreakCount.ToString());

            DrawReadOnlyValue(
                "Rewards Earned",
                $"{summary.RewardMinutesEarned:0.#} minutes");

            DrawReadOnlyValue(
                "Rewards Spent",
                $"{summary.RewardMinutesSpent:0.#} minutes");
        }

        private static void DrawNamedHistorySummary(
            string title,
            IReadOnlyList<DeverQuestNamedSummary> summaries)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                title,
                EditorStyles.boldLabel);

            if (summaries.Count == 0)
            {
                EditorGUILayout.LabelField(
                    "No matching data.",
                    EditorStyles.miniLabel);
                return;
            }

            foreach (DeverQuestNamedSummary summary in summaries)
            {
                EditorGUILayout.LabelField(
                    summary.Name,
                    $"{FormatDuration(summary.FocusedSeconds)} · " +
                    $"{summary.SessionCount} session(s)");
            }
        }

        private static void DrawWalletStatistics()
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Reward Wallet Statistics",
                EditorStyles.boldLabel);

            foreach (DeverQuestRewardCategory category
                     in DeverQuestRewardService.Wallet.categories)
            {
                EditorGUILayout.LabelField(
                    category.displayName,
                    $"Balance {category.balanceMinutes:0.#}m · " +
                    $"Earned {category.totalEarnedMinutes:0.#}m · " +
                    $"Spent {category.totalSpentMinutes:0.#}m");
            }
        }

        private static void DrawHistoryDays(
            IReadOnlyList<DeverQuestHistoryDay> days)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Daily Timecards",
                EditorStyles.boldLabel);

            if (days.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No timecards match the current filters.",
                    MessageType.Info);
                return;
            }

            foreach (DeverQuestHistoryDay day in days)
            {
                DeverQuestHistorySummary summary =
                    DeverQuestHistoryService.BuildSummary(
                        new List<DeverQuestHistoryDay> { day });

                EditorGUILayout.BeginVertical(
                    EditorStyles.helpBox);

                EditorGUILayout.LabelField(
                    day.Date.ToString("dddd, MMMM d, yyyy"),
                    EditorStyles.boldLabel);

                EditorGUILayout.LabelField(
                    $"{summary.SessionCount} session(s) · " +
                    $"{FormatDuration(summary.FocusedSeconds)} focused");

                EditorGUILayout.BeginHorizontal();

                using (new EditorGUI.DisabledScope(
                           !File.Exists(day.MarkdownPath)))
                {
                    if (GUILayout.Button("Open Timecard"))
                    {
                        EditorUtility.OpenWithDefaultApp(
                            day.MarkdownPath);
                    }

                    if (GUILayout.Button("Reveal Timecard"))
                    {
                        EditorUtility.RevealInFinder(
                            day.MarkdownPath);
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawHistoryExport(
            DeverQuestProfile profile,
            IReadOnlyList<DeverQuestHistoryDay> days)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Export Filtered Report",
                EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(days.Count == 0))
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Export CSV"))
                {
                    ExportHistoryCsv(profile, days);
                }

                if (GUILayout.Button("Export JSON"))
                {
                    ExportHistoryJson(profile, days);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void ExportHistoryCsv(
            DeverQuestProfile profile,
            IReadOnlyList<DeverQuestHistoryDay> days)
        {
            string path = EditorUtility.SaveFilePanel(
                "Export DeverQuest CSV",
                GetDeveloperFolder(profile),
                $"DeverQuest_Report_{DateTime.Now:yyyy-MM-dd}",
                "csv");

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            historyMessage =
                DeverQuestHistoryService.TryExportCsv(
                    path,
                    days,
                    out string error)
                    ? $"CSV exported:\n{path}"
                    : $"CSV export failed: {error}";
        }

        private void ExportHistoryJson(
            DeverQuestProfile profile,
            IReadOnlyList<DeverQuestHistoryDay> days)
        {
            string path = EditorUtility.SaveFilePanel(
                "Export DeverQuest JSON",
                GetDeveloperFolder(profile),
                $"DeverQuest_Report_{DateTime.Now:yyyy-MM-dd}",
                "json");

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            historyMessage =
                DeverQuestHistoryService.TryExportJson(
                    path,
                    profile.developerName,
                    days,
                    out string error)
                    ? $"JSON exported:\n{path}"
                    : $"JSON export failed: {error}";
        }

        private bool TryGetHistoryDateRange(
            out DateTime? startDate,
            out DateTime? endDate,
            out string errorMessage)
        {
            DateTime today = DateTime.Today;
            startDate = null;
            endDate = null;
            errorMessage = string.Empty;

            switch (historyRange)
            {
                case DeverQuestHistoryRange.Today:
                    startDate = today;
                    endDate = today;
                    return true;
                case DeverQuestHistoryRange.Last7Days:
                    startDate = today.AddDays(-6);
                    endDate = today;
                    return true;
                case DeverQuestHistoryRange.Last30Days:
                    startDate = today.AddDays(-29);
                    endDate = today;
                    return true;
                case DeverQuestHistoryRange.Custom:
                    if (!DateTime.TryParseExact(
                            historyStartDate,
                            "yyyy-MM-dd",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out DateTime parsedStart) ||
                        !DateTime.TryParseExact(
                            historyEndDate,
                            "yyyy-MM-dd",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out DateTime parsedEnd))
                    {
                        errorMessage =
                            "Custom dates must use yyyy-MM-dd.";
                        return false;
                    }

                    if (parsedEnd < parsedStart)
                    {
                        errorMessage =
                            "The end date cannot be before the start date.";
                        return false;
                    }

                    startDate = parsedStart;
                    endDate = parsedEnd;
                    return true;
                default:
                    return true;
            }
        }

        private static string GetDeveloperFolder(
            DeverQuestProfile profile)
        {
            return DeverQuestPathUtility.GetDeveloperFolder(
                profile.timecardRootPath,
                profile.developerName);
        }

        private void DrawPlaylistSetup(DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Session Music",
                EditorStyles.boldLabel);

            profile.autoPlayMusicOnSessionStart =
                EditorGUILayout.Toggle(
                    "Play on Session Start",
                    profile.autoPlayMusicOnSessionStart);

            profile.pauseMusicWithSession =
                EditorGUILayout.Toggle(
                    "Pause with Session",
                    profile.pauseMusicWithSession);

            profile.resumeMusicWithSession =
                EditorGUILayout.Toggle(
                    "Resume with Session",
                    profile.resumeMusicWithSession);

            profile.stopMusicOnSessionEnd =
                EditorGUILayout.Toggle(
                    "Stop on Session End",
                    profile.stopMusicOnSessionEnd);
        }

        private static void DrawPolishSetup(DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Appearance and Notifications",
                EditorStyles.boldLabel);

            profile.theme =
                (DeverQuestTheme)EditorGUILayout.EnumPopup(
                    "Theme",
                    profile.theme);

            profile.showEditorNotifications =
                EditorGUILayout.Toggle(
                    "Editor Notifications",
                    profile.showEditorNotifications);

            profile.notificationSoundsEnabled =
                EditorGUILayout.Toggle(
                    "Notification Sounds",
                    profile.notificationSoundsEnabled);

            profile.autoOpenWindowForReminders =
                EditorGUILayout.Toggle(
                    new GUIContent(
                        "Open Window for Reminders",
                        "When disabled, reminders never force a closed " +
                        "DeverQuest window to open."),
                    profile.autoOpenWindowForReminders);
        }

        private static void DrawProjectDefaultsSetup(
            DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Quest Project Defaults",
                EditorStyles.boldLabel);

            profile.lockProjectName = EditorGUILayout.Toggle(
                new GUIContent(
                    "Lock Project Name",
                    "Useful when this package belongs to one company project."),
                profile.lockProjectName);

            using (new EditorGUI.DisabledScope(!profile.lockProjectName))
            {
                profile.lockedProjectName = EditorGUILayout.TextField(
                    "Locked Project",
                    profile.lockedProjectName);
            }
        }

        private static void ApplyFocusSchedule(
            DeverQuestProfile profile,
            string scheduleText)
        {
            List<int> parsed = new List<int>();
            string[] values =
                (scheduleText ?? string.Empty).Split(',');

            foreach (string value in values)
            {
                if (int.TryParse(value.Trim(), out int minutes) &&
                    minutes > 0 &&
                    !parsed.Contains(minutes))
                {
                    parsed.Add(minutes);
                }
            }

            parsed.Sort();
            profile.focusCheckInScheduleMinutes = parsed;
        }

        private void DrawPlaylistPlayer()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Playlist Player",
                EditorStyles.boldLabel);

            DeverQuestPlaylist selected =
                (DeverQuestPlaylist)EditorGUILayout.ObjectField(
                    "Playlist",
                    DeverQuestPlaylistPlayer.Playlist,
                    typeof(DeverQuestPlaylist),
                    false);

            if (selected != DeverQuestPlaylistPlayer.Playlist)
            {
                DeverQuestPlaylistPlayer.SetPlaylist(selected);
            }

            if (selected == null)
            {
                if (GUILayout.Button("Create Playlist Asset"))
                {
                    CreatePlaylistAsset();
                    GUIUtility.ExitGUI();
                }

                EditorGUILayout.HelpBox(
                    "Create a playlist, select it, then add AudioClips " +
                    "in its Inspector.",
                    MessageType.Info);

                EditorGUILayout.EndVertical();
                return;
            }

            AudioClip current =
                DeverQuestPlaylistPlayer.CurrentTrack;

            EditorGUILayout.LabelField(
                "Now Playing",
                current == null
                    ? "No track selected"
                    : current.name);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Previous"))
            {
                DeverQuestPlaylistPlayer.Previous();
            }

            string playLabel =
                DeverQuestPlaylistPlayer.State ==
                DeverQuestPlaybackState.Playing
                    ? "Pause"
                    : "Play";

            if (GUILayout.Button(playLabel))
            {
                if (DeverQuestPlaylistPlayer.State ==
                    DeverQuestPlaybackState.Playing)
                {
                    DeverQuestPlaylistPlayer.Pause();
                }
                else
                {
                    DeverQuestPlaylistPlayer.Play();
                }
            }

            if (GUILayout.Button("Next"))
            {
                DeverQuestPlaylistPlayer.Next();
            }

            if (GUILayout.Button("Stop"))
            {
                DeverQuestPlaylistPlayer.Stop();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();

            selected.Shuffle =
                EditorGUILayout.Toggle(
                    "Shuffle",
                    selected.Shuffle);

            selected.RepeatMode =
                (DeverQuestRepeatMode)EditorGUILayout.EnumPopup(
                    "Repeat",
                    selected.RepeatMode);

            selected.Volume =
                EditorGUILayout.Slider(
                    "Volume",
                    selected.Volume,
                    0f,
                    1f);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(selected);
                DeverQuestPlaylistPlayer.ApplyVolume();
            }

            if (!DeverQuestEditorAudioBridge.VolumeSupported)
            {
                EditorGUILayout.HelpBox(
                    "This Unity editor does not expose preview-volume " +
                    "control. Playback still works, but the volume slider " +
                    "may not affect editor preview audio.",
                    MessageType.Warning);
            }

            if (!DeverQuestEditorAudioBridge.PlaybackStatusSupported)
            {
                EditorGUILayout.HelpBox(
                    "This Unity editor does not expose preview playback " +
                    "status. Use Next manually when a track ends.",
                    MessageType.Warning);
            }

            if (!string.IsNullOrWhiteSpace(
                    DeverQuestPlaylistPlayer.LastError))
            {
                EditorGUILayout.HelpBox(
                    DeverQuestPlaylistPlayer.LastError,
                    MessageType.Error);
            }

            if (GUILayout.Button("Select Playlist in Project"))
            {
                Selection.activeObject = selected;
                EditorGUIUtility.PingObject(selected);
            }

            EditorGUILayout.EndVertical();
        }

        private static void CreatePlaylistAsset()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create DeverQuest Playlist",
                "DeverQuestPlaylist",
                "asset",
                "Choose where to save the playlist asset.");

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            DeverQuestPlaylist playlist =
                CreateInstance<DeverQuestPlaylist>();

            AssetDatabase.CreateAsset(playlist, path);
            AssetDatabase.SaveAssets();

            DeverQuestPlaylistPlayer.SetPlaylist(playlist);
            Selection.activeObject = playlist;
            EditorGUIUtility.PingObject(playlist);
        }

        private void DrawRewardSetup(DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Reward Economy",
                EditorStyles.boldLabel);

            profile.rewardsEnabled =
                EditorGUILayout.Toggle(
                    "Enable Rewards",
                    profile.rewardsEnabled);

            using (new EditorGUI.DisabledScope(!profile.rewardsEnabled))
            {
                profile.rewardWorkBlockMinutes =
                    EditorGUILayout.IntField(
                        "Work Block (min)",
                        profile.rewardWorkBlockMinutes);

                profile.dailyWorkGoalMinutes =
                    EditorGUILayout.IntField(
                        "Daily Goal (min)",
                        profile.dailyWorkGoalMinutes);
            }

            profile.Sanitize();
        }

        private void DrawRewardsPanel(DeverQuestProfile profile)
        {
            if (!profile.rewardsEnabled)
            {
                return;
            }

            EditorGUILayout.LabelField(
                "Reward Wallet",
                EditorStyles.boldLabel);

            double todayMinutes =
                DeverQuestRewardService.GetTodayFocusedMinutes();

            EditorGUILayout.LabelField(
                $"Daily Goal: {todayMinutes:0.#} / " +
                $"{profile.dailyWorkGoalMinutes} focused minutes");

            double carryMinutes =
                DeverQuestRewardService.Wallet
                    .unrewardedWorkSeconds / 60d;

            EditorGUILayout.LabelField(
                $"Next work block: {carryMinutes:0.#} / " +
                $"{profile.rewardWorkBlockMinutes} minutes");

            foreach (DeverQuestRewardCategory category
                     in DeverQuestRewardService.Wallet.categories)
            {
                DrawRewardCategory(category);
            }

            if (!string.IsNullOrWhiteSpace(rewardMessage))
            {
                EditorGUILayout.HelpBox(
                    rewardMessage,
                    MessageType.Info);
            }

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField(
                "Add Custom Reward",
                EditorStyles.boldLabel);

            newRewardCategoryName =
                EditorGUILayout.TextField(
                    "Name",
                    newRewardCategoryName);

            newRewardMinutesPerBlock =
                EditorGUILayout.DoubleField(
                    "Minutes per Block",
                    newRewardMinutesPerBlock);

            newRewardDailyBonus =
                EditorGUILayout.DoubleField(
                    "Daily Bonus",
                    newRewardDailyBonus);

            if (GUILayout.Button("Add Reward Category"))
            {
                if (DeverQuestRewardService.AddCategory(
                        newRewardCategoryName,
                        newRewardMinutesPerBlock,
                        newRewardDailyBonus,
                        out string error))
                {
                    rewardMessage =
                        $"Added reward: {newRewardCategoryName}.";
                    newRewardCategoryName = string.Empty;
                }
                else
                {
                    rewardMessage = error;
                }

                Repaint();
            }
        }

        private void DrawRewardCategory(
            DeverQuestRewardCategory category)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField(
                $"{category.displayName}: " +
                $"{category.balanceMinutes:0.#} minutes",
                EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            category.rewardMinutesPerBlock =
                EditorGUILayout.DoubleField(
                    "Per Work Block",
                    category.rewardMinutesPerBlock);

            category.dailyBonusMinutes =
                EditorGUILayout.DoubleField(
                    "Daily Bonus",
                    category.dailyBonusMinutes);

            if (EditorGUI.EndChangeCheck())
            {
                category.Sanitize();
                DeverQuestRewardService.Save();
            }

            if (!rewardSpendMinutes.ContainsKey(category.categoryId))
            {
                rewardSpendMinutes[category.categoryId] = 0d;
            }

            EditorGUILayout.BeginHorizontal();

            rewardSpendMinutes[category.categoryId] =
                EditorGUILayout.DoubleField(
                    "Spend Minutes",
                    rewardSpendMinutes[category.categoryId]);

            if (GUILayout.Button("Spend", GUILayout.Width(64f)))
            {
                if (DeverQuestRewardService.Spend(
                        category.categoryId,
                        rewardSpendMinutes[category.categoryId],
                        out DeverQuestRewardTransaction transaction,
                        out string error))
                {
                    rewardMessage =
                        $"Spent {-transaction.minutes:0.#} minutes of " +
                        $"{category.displayName}.";

                    rewardSpendMinutes[category.categoryId] = 0d;
                    DeverQuestSessionStore.AddRewardTransaction(
                        transaction);
                }
                else
                {
                    rewardMessage = error;
                }

                Repaint();
            }

            EditorGUILayout.EndHorizontal();

            if (!category.isBuiltIn &&
                GUILayout.Button("Remove Category"))
            {
                bool confirmed = EditorUtility.DisplayDialog(
                    "Remove Reward Category?",
                    $"Remove {category.displayName}? Its remaining balance " +
                    "will be lost.",
                    "Remove",
                    "Cancel");

                if (confirmed)
                {
                    DeverQuestRewardService.RemoveCategory(
                        category.categoryId,
                        out rewardMessage);

                    GUIUtility.ExitGUI();
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawWellnessSetup(DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Wellness Reminders",
                EditorStyles.boldLabel);

            profile.wellnessEnabled =
                EditorGUILayout.Toggle(
                    "Enable Wellness",
                    profile.wellnessEnabled);

            using (new EditorGUI.DisabledScope(!profile.wellnessEnabled))
            {
                profile.checkInMinutes =
                    EditorGUILayout.IntField(
                        "Fallback Check-In (min)",
                        profile.checkInMinutes);

                focusScheduleText = EditorGUILayout.TextField(
                    new GUIContent(
                        "Check-In Schedule",
                        "Comma-separated focused minutes, such as " +
                        "15, 30, 45, 60. Empty uses the fallback interval."),
                    focusScheduleText);
                ApplyFocusSchedule(profile, focusScheduleText);

                profile.hydrationMinutes =
                    EditorGUILayout.IntField(
                        "Hydration (min)",
                        profile.hydrationMinutes);

                profile.movementBreakMinutes =
                    EditorGUILayout.IntField(
                        "Movement Break (min)",
                        profile.movementBreakMinutes);

                profile.exerciseMinutes =
                    EditorGUILayout.IntField(
                        "Exercise (min)",
                        profile.exerciseMinutes);

                profile.snoozeMinutes =
                    EditorGUILayout.IntField(
                        "Snooze (min)",
                        profile.snoozeMinutes);

                profile.mealRemindersEnabled =
                    EditorGUILayout.Toggle(
                        "Meal Reminders",
                        profile.mealRemindersEnabled);

                using (new EditorGUI.DisabledScope(
                           !profile.mealRemindersEnabled))
                {
                    profile.lunchHour =
                        EditorGUILayout.IntSlider(
                            "Lunch Hour",
                            profile.lunchHour,
                            0,
                            23);

                    profile.lunchMinute =
                        EditorGUILayout.IntSlider(
                            "Lunch Minute",
                            profile.lunchMinute,
                            0,
                            59);

                    profile.dinnerHour =
                        EditorGUILayout.IntSlider(
                            "Dinner Hour",
                            profile.dinnerHour,
                            0,
                            23);

                    profile.dinnerMinute =
                        EditorGUILayout.IntSlider(
                            "Dinner Minute",
                            profile.dinnerMinute,
                            0,
                            59);
                }

                profile.quietHoursEnabled =
                    EditorGUILayout.Toggle(
                        "Quiet Hours",
                        profile.quietHoursEnabled);

                using (new EditorGUI.DisabledScope(
                           !profile.quietHoursEnabled))
                {
                    profile.quietHoursStartHour =
                        EditorGUILayout.IntSlider(
                            "Quiet Start Hour",
                            profile.quietHoursStartHour,
                            0,
                            23);
                }
            }

            profile.Sanitize();
        }

        private void DrawWellnessReminder()
        {
            if (!DeverQuestWellnessMonitor.HasActiveReminder)
            {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                DeverQuestWellnessMonitor.ActiveTitle,
                EditorStyles.boldLabel);

            EditorGUILayout.LabelField(
                DeverQuestWellnessMonitor.ActiveMessage,
                wrappedLabelStyle);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Pause and Take Break"))
            {
                DeverQuestWellnessMonitor.Acknowledge(true);
                Repaint();
            }

            if (GUILayout.Button("Acknowledge"))
            {
                DeverQuestWellnessMonitor.Acknowledge(false);
                Repaint();
            }

            if (GUILayout.Button(
                    $"Snooze {DeverQuestSettingsStore.Profile.snoozeMinutes}m"))
            {
                DeverQuestWellnessMonitor.Snooze();
                Repaint();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private void DrawNewSessionForm(DeverQuestProfile profile)
        {
            if (profile.lockProjectName)
            {
                newProjectName = profile.lockedProjectName;
            }

            EditorGUILayout.LabelField(
                "Accept a Deliberate Quest",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Focused time begins only when you press Accept Quest.",
                MessageType.Info);

            using (new EditorGUI.DisabledScope(profile.lockProjectName))
            {
                newProjectName = EditorGUILayout.TextField(
                    "Project",
                    profile.lockProjectName
                        ? profile.lockedProjectName
                        : newProjectName);
            }

            newTaskName = EditorGUILayout.TextField(
                "Task / Milestone",
                newTaskName);

            newCategory = EditorGUILayout.TextField(
                "Department",
                newCategory);

            EditorGUILayout.LabelField("Quest Goal");
            newGoal = EditorGUILayout.TextArea(
                newGoal,
                GUILayout.MinHeight(54f));

            bool canStart =
                !string.IsNullOrWhiteSpace(newProjectName) &&
                !string.IsNullOrWhiteSpace(newTaskName);

            using (new EditorGUI.DisabledScope(!canStart))
            {
                if (GUILayout.Button(
                        "Accept Quest",
                        GUILayout.Height(36f)))
                {
                    profile.lastProjectName = newProjectName;
                    profile.lastDepartmentName = newCategory;
                    DeverQuestSettingsStore.Save();
                    DeverQuestSessionStore.StartSession(
                        profile.developerName,
                        newProjectName,
                        newTaskName,
                        newCategory,
                        newGoal);

                    Repaint();
                }
            }
        }

        private void DrawActiveSession()
        {
            DeverQuestSession session =
                DeverQuestSessionStore.ActiveSession;

            bool isRunning =
                session.state == DeverQuestSessionState.Running;

            EditorGUILayout.LabelField(
                isRunning ? "QUEST IN PROGRESS" : "QUEST PAUSED",
                EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                FormatDuration(
                    DeverQuestSessionStore.GetFocusedSeconds()),
                timerStyle,
                GUILayout.Height(48f));

            EditorGUILayout.LabelField(
                $"Meditation: {FormatDuration(DeverQuestSessionStore.GetPausedSeconds())}",
                subtitleStyle,
                GUILayout.Height(22f));

            if (isRunning &&
                DeverQuestSettingsStore.Profile.idleDetectionEnabled)
            {
                string idleStatus = DeverQuestIdleMonitor.IsSupported
                    ? $"Input idle: {FormatShortDuration(DeverQuestIdleMonitor.CurrentIdleSeconds)}"
                    : "Idle detection is unavailable on this platform.";

                EditorGUILayout.LabelField(
                    idleStatus,
                    subtitleStyle,
                    GUILayout.Height(22f));
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10f);

            if (showFinalization)
            {
                DrawFinalizationPanel(session);
                return;
            }

            if (!session.idlePauseAcknowledged)
            {
                EditorGUILayout.HelpBox(
                    $"This quest was automatically paused because: " +
                    $"{session.pauseReason}.\n\nAcknowledge your return " +
                    "before the quest can resume.",
                    MessageType.Warning);
                if (GUILayout.Button(
                        "I Have Returned — Acknowledge Pause",
                        GUILayout.Height(38f)))
                {
                    DeverQuestSessionStore.AcknowledgeIdlePause();
                    Repaint();
                }
                return;
            }

            DrawReadOnlyValue("Project", session.projectName);
            DrawReadOnlyValue("Task", session.taskName);
            DrawReadOnlyValue("Department", session.category);
            DrawReadOnlyValue(
                "Started",
                DeverQuestSessionStore
                    .GetLocalStartTime(session)
                    .ToString("g"));

            if (!string.IsNullOrWhiteSpace(session.goal))
            {
                EditorGUILayout.Space(6f);
                EditorGUILayout.LabelField(
                    "Quest Objective",
                    EditorStyles.boldLabel);

                EditorGUILayout.LabelField(
                    session.goal,
                    wrappedLabelStyle);
            }

            DrawCommitJournal(session);

            if (!isRunning &&
                !string.IsNullOrWhiteSpace(session.pauseReason))
            {
                EditorGUILayout.Space(4f);
                EditorGUILayout.HelpBox(
                    $"Meditating because: {session.pauseReason}",
                    session.pauseReason == "Idle Detection" ||
                    session.pauseReason == "Unity Project Lost Focus"
                        ? MessageType.Warning
                        : MessageType.Info);
            }

            EditorGUILayout.Space(12f);

            EditorGUILayout.BeginHorizontal();

            if (isRunning)
            {
                if (GUILayout.Button(
                        "Meditate",
                        GUILayout.Height(32f)))
                {
                    DeverQuestSessionStore.PauseSession();
                    Repaint();
                }
            }
            else if (GUILayout.Button(
                         "Resume Quest",
                         GUILayout.Height(32f)))
            {
                DeverQuestSessionStore.ResumeSession();
                Repaint();
            }

            if (GUILayout.Button(
                    "Complete Quest",
                    GUILayout.Height(32f)))
            {
                BeginFinalization(session);
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Abandon Quest"))
            {
                bool discard = EditorUtility.DisplayDialog(
                    "Abandon Current Quest?",
                    "This removes the current quest without keeping its " +
                    "focused time. This cannot be undone.",
                    "Abandon",
                    "Keep Quest");

                if (discard)
                {
                    DeverQuestSessionStore.DiscardSession();
                    Repaint();
                }
            }
        }

        private void DrawCommitJournal(DeverQuestSession session)
        {
            EditorGUILayout.Space(14f);
            EditorGUILayout.LabelField(
                "Quest Log and Git",
                EditorStyles.boldLabel);

            DrawGitPanel();

            EditorGUILayout.LabelField("Commit Details");
            commitComment = EditorGUILayout.TextArea(
                commitComment,
                GUILayout.MinHeight(46f));

            EditorGUILayout.BeginHorizontal();
            if (gitStatus != null && gitStatus.IsRepository)
            {
                if (string.IsNullOrWhiteSpace(commitBranch))
                {
                    commitBranch = gitStatus.Branch;
                }
                if (string.IsNullOrWhiteSpace(commitHash))
                {
                    commitHash = gitStatus.ShortHash;
                }
            }
            commitBranch = EditorGUILayout.TextField(
                new GUIContent("Branch"),
                commitBranch);
            commitHash = EditorGUILayout.TextField(
                new GUIContent("Hash"),
                commitHash);
            EditorGUILayout.EndHorizontal();

            using (new EditorGUI.DisabledScope(
                       string.IsNullOrWhiteSpace(commitComment)))
            {
                if (GUILayout.Button(
                        "Add Quest Log Note (No Git Commit)"))
                {
                    DeverQuestSessionStore.AddCommitEntry(
                        commitComment,
                        commitBranch,
                        commitHash);

                    commitComment = string.Empty;
                    commitHash = string.Empty;
                    Repaint();
                }
            }

            if (session.commitEntries == null ||
                session.commitEntries.Count == 0)
            {
                EditorGUILayout.LabelField(
                    "No commit entries yet.",
                    EditorStyles.miniLabel);
                return;
            }

            for (int index = 0;
                 index < session.commitEntries.Count;
                 index++)
            {
                DeverQuestCommitEntry entry =
                    session.commitEntries[index];

                EditorGUILayout.BeginHorizontal(
                    EditorStyles.helpBox);

                string entryLabel =
                    $"{index + 1}. {entry.comment} " +
                    $"(+{FormatDuration(entry.focusedSecondsAtEntry)})";

                EditorGUILayout.LabelField(
                    entryLabel,
                    wrappedLabelStyle);

                bool removeEntry = GUILayout.Button(
                        "Remove",
                        GUILayout.Width(64f));

                EditorGUILayout.EndHorizontal();

                if (removeEntry)
                {
                    DeverQuestSessionStore.RemoveCommitEntry(
                        entry.entryId);

                    GUIUtility.ExitGUI();
                }
            }
        }

        private void DrawGitPanel()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                "Git Repository",
                EditorStyles.boldLabel);
            if (GUILayout.Button("Refresh", GUILayout.Width(72f)))
            {
                RefreshGitStatus();
            }
            EditorGUILayout.EndHorizontal();

            if (gitStatus == null || !gitStatus.GitAvailable)
            {
                EditorGUILayout.HelpBox(
                    "Git was not found. Install Git and ensure the git " +
                    "command is available to Unity.",
                    MessageType.Warning);
                EditorGUILayout.EndVertical();
                return;
            }

            if (!gitStatus.IsRepository)
            {
                EditorGUILayout.HelpBox(
                    "This Unity project is not inside a Git repository. " +
                    "Create or clone a repository before using Git actions.",
                    MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            DrawReadOnlyValue(
                "Branch",
                string.IsNullOrWhiteSpace(gitStatus.Branch)
                    ? "Detached HEAD"
                    : gitStatus.Branch);
            DrawReadOnlyValue(
                "Current Commit",
                string.IsNullOrWhiteSpace(gitStatus.ShortHash)
                    ? "No commits yet"
                    : gitStatus.ShortHash);
            EditorGUILayout.LabelField(
                $"Staged: {gitStatus.StagedCount} · " +
                $"Modified: {gitStatus.UnstagedCount} · " +
                $"Untracked: {gitStatus.UntrackedCount}");

            EditorGUILayout.HelpBox(
                "Branch = your current development path. Staged files = " +
                "the changes selected for the next Git commit. Hash = the " +
                "unique ID Git assigns after committing.",
                MessageType.Info);

            using (new EditorGUI.DisabledScope(
                       !gitStatus.HasStagedChanges ||
                       string.IsNullOrWhiteSpace(commitComment)))
            {
                if (GUILayout.Button("Commit Staged Changes"))
                {
                    CommitWithGit(false);
                }
            }

            using (new EditorGUI.DisabledScope(
                       gitStatus.IsClean ||
                       string.IsNullOrWhiteSpace(commitComment)))
            {
                if (GUILayout.Button("Stage All and Commit…"))
                {
                    CommitWithGit(true);
                }
            }

            if (!string.IsNullOrWhiteSpace(gitMessage))
            {
                EditorGUILayout.HelpBox(
                    gitMessage,
                    gitMessage.StartsWith("Git commit created")
                        ? MessageType.Info
                        : MessageType.Warning);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private void CommitWithGit(bool stageAll)
        {
            if (gitStatus == null || !gitStatus.IsRepository)
            {
                gitMessage = "No Git repository is available.";
                return;
            }

            if (stageAll)
            {
                bool confirmed = EditorUtility.DisplayDialog(
                    "Stage Every Project Change?",
                    "This stages every modified, deleted, and untracked " +
                    "file in the repository for one commit.\n\nReview your " +
                    "changes first if you are unsure.",
                    "Stage All and Commit",
                    "Cancel");

                if (!confirmed)
                {
                    return;
                }

                DeverQuestGitResult stageResult =
                    DeverQuestGitService.StageAll(
                        gitStatus.RepositoryRoot);
                if (!stageResult.Succeeded)
                {
                    gitMessage =
                        $"Git could not stage the changes: " +
                        $"{stageResult.Error}";
                    RefreshGitStatus(false);
                    return;
                }
            }

            DeverQuestGitResult commitResult =
                DeverQuestGitService.CommitStaged(
                    gitStatus.RepositoryRoot,
                    commitComment);
            if (!commitResult.Succeeded)
            {
                gitMessage =
                    $"Git commit failed: {commitResult.Error}";
                RefreshGitStatus(false);
                return;
            }

            string committedMessage = commitComment.Trim();
            RefreshGitStatus(false);
            DeverQuestGitMonitor.MarkObserved(gitStatus);
            DeverQuestSessionStore.AddCommitEntry(
                committedMessage,
                gitStatus.Branch,
                gitStatus.ShortHash);
            commitBranch = gitStatus.Branch;
            commitHash = gitStatus.ShortHash;
            commitComment = string.Empty;
            gitMessage =
                $"Git commit created: {gitStatus.ShortHash}";
            Repaint();
        }

        private void RefreshGitStatus(bool clearMessage = true)
        {
            gitStatus = DeverQuestGitService.Refresh();
            if (clearMessage)
            {
                gitMessage = string.Empty;
            }
        }

        private void BeginFinalization(
            DeverQuestSession session)
        {
            if (!session.idlePauseAcknowledged)
            {
                DeverQuestSessionStore.AcknowledgeIdlePause();
            }

            resumeIfFinalizationCancelled =
                session.state == DeverQuestSessionState.Running;

            if (resumeIfFinalizationCancelled)
            {
                DeverQuestSessionStore.PauseSession("Finalizing");
            }

            showFinalization = true;
            closingNotes = string.Empty;
            Repaint();
        }

        private void DrawFinalizationPanel(
            DeverQuestSession session)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Complete Quest",
                EditorStyles.boldLabel);

            EditorGUILayout.LabelField(
                $"Focused: {FormatDuration(DeverQuestSessionStore.GetFocusedSeconds())}");

            EditorGUILayout.HelpBox(
                "Focused time is intentionally paused while you write your " +
                "closing notes. Continue Working will resume it.",
                MessageType.Info);

            EditorGUILayout.LabelField("Closing Notes");
            closingNotes = EditorGUILayout.TextArea(
                closingNotes,
                GUILayout.MinHeight(70f));

            EditorGUILayout.HelpBox(
                "Completing this quest will write or update today's Quest Ledger.",
                MessageType.Info);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(
                    "Complete Quest and Write Ledger",
                    GUILayout.Height(34f)))
            {
                FinalizeSession();
                GUIUtility.ExitGUI();
            }

            if (GUILayout.Button(
                    "Continue Working",
                    GUILayout.Height(34f)))
            {
                showFinalization = false;

                if (resumeIfFinalizationCancelled &&
                    DeverQuestSessionStore.HasActiveSession)
                {
                    DeverQuestSessionStore.ResumeSession();
                }

                resumeIfFinalizationCancelled = false;
                Repaint();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void FinalizeSession()
        {
            DeverQuestSession session =
                DeverQuestSessionStore.CompleteSession(
                    closingNotes);

            if (session == null)
            {
                return;
            }

            DeverQuestRewardService.ProcessCompletedSession(
                DeverQuestSettingsStore.Profile,
                session);

            WriteTimecard(session);

            newProjectName = session.projectName;
            newCategory = session.category;
            newTaskName = string.Empty;
            newGoal = string.Empty;
            commitComment = string.Empty;
            commitHash = string.Empty;
            closingNotes = string.Empty;
            showFinalization = false;
            resumeIfFinalizationCancelled = false;

            Repaint();
        }

        private static void WriteTimecard(
            DeverQuestSession session)
        {
            bool succeeded =
                DeverQuestTimecardWriter.TryWriteSession(
                    DeverQuestSettingsStore.Profile,
                    session,
                    out string timecardPath,
                    out string errorMessage);

            session.timecardWriteSucceeded = succeeded;
            session.timecardWriteAttempted = true;
            session.timecardPath = timecardPath;
            session.timecardWriteError = errorMessage;

            DeverQuestSessionStore.SaveCompletedSession(session);
            DeverQuestHistoryService.Refresh(
                DeverQuestSettingsStore.Profile);
        }

        private void DrawLastCompletedSession()
        {
            DeverQuestSession session =
                DeverQuestSessionStore.LastCompletedSession;

            if (session == null ||
                session.state != DeverQuestSessionState.Completed)
            {
                return;
            }

            EditorGUILayout.Space(18f);
            EditorGUILayout.LabelField(
                "Last Completed Session",
                EditorStyles.boldLabel);

            DrawReadOnlyValue("Project", session.projectName);
            DrawReadOnlyValue("Task", session.taskName);
            DrawReadOnlyValue(
                "Focused",
                FormatDuration(
                    session.accumulatedFocusedSeconds));

            DrawReadOnlyValue(
                "Completed",
                DeverQuestSessionStore
                    .GetLocalCompletionTime(session)
                    .ToString("g"));

            DrawReadOnlyValue(
                "Commits",
                (session.commitEntries?.Count ?? 0).ToString());

            if (!session.timecardWriteAttempted)
            {
                EditorGUILayout.HelpBox(
                    "This session was completed before timecard writing " +
                    "was installed.",
                    MessageType.Info);

                if (GUILayout.Button("Write Session to Timecard"))
                {
                    WriteTimecard(session);
                    Repaint();
                }
            }
            else if (session.timecardWriteSucceeded)
            {
                EditorGUILayout.HelpBox(
                    $"Timecard written:\n{session.timecardPath}",
                    MessageType.Info);

                if (GUILayout.Button("Reveal Timecard"))
                {
                    EditorUtility.RevealInFinder(
                        session.timecardPath);
                }
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Timecard write failed:\n" +
                    session.timecardWriteError,
                    MessageType.Error);

                if (GUILayout.Button("Retry Timecard Write"))
                {
                    WriteTimecard(session);
                    Repaint();
                }
            }
        }

        private void DrawProfileControls(DeverQuestProfile profile)
        {
            EditorGUILayout.LabelField(
                "Profile",
                EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            DrawPolishSetup(profile);
            if (EditorGUI.EndChangeCheck())
            {
                profile.Sanitize();
                DeverQuestSettingsStore.Save();
                Repaint();
            }

            string developerFolder =
                DeverQuestPathUtility.GetDeveloperFolder(
                    profile.timecardRootPath,
                    profile.developerName);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Reveal Timecard Folder"))
            {
                if (Directory.Exists(developerFolder))
                {
                    EditorUtility.RevealInFinder(developerFolder);
                }
                else
                {
                    EditorUtility.DisplayDialog(
                        "Folder Missing",
                        "The developer folder no longer exists. " +
                        "Reconfigure the profile to validate it again.",
                        "Close");
                }
            }

            using (new EditorGUI.DisabledScope(
                       DeverQuestSessionStore.HasActiveSession))
            {
                if (GUILayout.Button("Reconfigure Profile"))
                {
                    profile.setupComplete = false;
                    DeverQuestSettingsStore.Save();
                    Repaint();
                }
            }

            EditorGUILayout.EndHorizontal();

            using (new EditorGUI.DisabledScope(
                       DeverQuestSessionStore.HasActiveSession))
            {
                if (GUILayout.Button("Reset DeverQuest Profile"))
                {
                    bool confirmed = EditorUtility.DisplayDialog(
                        "Reset DeverQuest Profile?",
                        "This clears DeverQuest's saved profile settings. " +
                        "It will not delete folders or timecards.",
                        "Reset Profile",
                        "Cancel");

                    if (confirmed)
                    {
                        DeverQuestSettingsStore.ResetProfile();
                        Repaint();
                    }
                }
            }
        }

        private static void DrawReadOnlyValue(
            string label,
            string value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            EditorGUILayout.SelectableLabel(
                value,
                EditorStyles.textField,
                GUILayout.Height(
                    EditorGUIUtility.singleLineHeight));
            EditorGUILayout.EndHorizontal();
        }

        private void BuildStyles()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 24,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            if (subtitleStyle == null)
            {
                subtitleStyle = new GUIStyle(EditorStyles.label)
                {
                    fontSize = 14,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            if (wrappedLabelStyle == null)
            {
                wrappedLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    wordWrap = true,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            if (timerStyle == null)
            {
                timerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 32,
                    alignment = TextAnchor.MiddleCenter,
                    fixedHeight = 44f
                };
            }
        }

        private void ApplyThemeToStyles(DeverQuestProfile profile)
        {
            Color titleColor = EditorStyles.label.normal.textColor;
            Color timerColor = EditorStyles.label.normal.textColor;

            switch (profile.theme)
            {
                case DeverQuestTheme.Dark:
                    titleColor = new Color(0.78f, 0.86f, 0.94f);
                    timerColor = new Color(0.55f, 0.82f, 1f);
                    break;
                case DeverQuestTheme.Light:
                    titleColor = new Color(0.12f, 0.20f, 0.28f);
                    timerColor = new Color(0.05f, 0.42f, 0.56f);
                    break;
                case DeverQuestTheme.EchoNeon:
                    titleColor = new Color(0.20f, 0.94f, 0.86f);
                    timerColor = new Color(1f, 0.30f, 0.70f);
                    break;
            }

            titleStyle.normal.textColor = titleColor;
            subtitleStyle.normal.textColor = titleColor;
            timerStyle.normal.textColor = timerColor;
        }

        private void RepaintWhileSessionRuns()
        {
            if (DeverQuestSessionStore.HasActiveSession)
            {
                Repaint();
            }
        }

        private static string FormatDuration(double totalSeconds)
        {
            System.TimeSpan duration =
                System.TimeSpan.FromSeconds(
                    System.Math.Max(0d, totalSeconds));

            if (duration.TotalHours >= 100d)
            {
                return $"{(int)duration.TotalHours:000}:" +
                       $"{duration.Minutes:00}:" +
                       $"{duration.Seconds:00}";
            }

            return $"{(int)duration.TotalHours:00}:" +
                   $"{duration.Minutes:00}:" +
                   $"{duration.Seconds:00}";
        }

        private static string FormatShortDuration(double totalSeconds)
        {
            System.TimeSpan duration =
                System.TimeSpan.FromSeconds(
                    System.Math.Max(0d, totalSeconds));

            if (duration.TotalMinutes >= 1d)
            {
                return $"{(int)duration.TotalMinutes}m " +
                       $"{duration.Seconds}s";
            }

            return $"{duration.Seconds}s";
        }
    }
}

//----- DeverQuestWindow.cs END -----
