//----- DeverQuestWindow.cs START -----

using System.IO;
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
            DeverQuestWindow window =
                GetWindow<DeverQuestWindow>("DeverQuest");

            window.ShowNotification(
                new GUIContent(
                    $"Still working? Idle pause in about " +
                    $"{secondsRemaining} seconds."),
                4d);

            EditorApplication.Beep();
            window.Repaint();
        }

        internal static void ShowIdlePaused()
        {
            DeverQuestWindow window =
                GetWindow<DeverQuestWindow>("DeverQuest");

            window.ShowNotification(
                new GUIContent(
                    "Session paused because no input was detected."),
                6d);

            EditorApplication.Beep();
            window.Repaint();
        }

        private void OnEnable()
        {
            minSize = new Vector2(
                MinimumWindowWidth,
                MinimumWindowHeight);

            EditorApplication.update -= RepaintWhileSessionRuns;
            EditorApplication.update += RepaintWhileSessionRuns;
        }

        private void OnDisable()
        {
            EditorApplication.update -= RepaintWhileSessionRuns;
        }

        private void OnGUI()
        {
            BuildStyles();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawHeader();
            EditorGUILayout.Space(12f);

            if (DeverQuestSettingsStore.Profile.setupComplete)
            {
                DrawSessionDashboard();
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
                "Deliberate work sessions, useful records, and earned downtime.",
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
                $"Welcome back, {profile.developerName}.",
                EditorStyles.boldLabel);

            EditorGUILayout.Space(8f);

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
            DrawProfileControls(profile);
        }

        private void DrawNewSessionForm(DeverQuestProfile profile)
        {
            EditorGUILayout.LabelField(
                "Start a Deliberate Session",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Work time begins only when you press Start Focus Session.",
                MessageType.Info);

            newProjectName = EditorGUILayout.TextField(
                "Project",
                newProjectName);

            newTaskName = EditorGUILayout.TextField(
                "Task / Milestone",
                newTaskName);

            newCategory = EditorGUILayout.TextField(
                "Category",
                newCategory);

            EditorGUILayout.LabelField("Session Goal");
            newGoal = EditorGUILayout.TextArea(
                newGoal,
                GUILayout.MinHeight(54f));

            bool canStart =
                !string.IsNullOrWhiteSpace(newProjectName) &&
                !string.IsNullOrWhiteSpace(newTaskName);

            using (new EditorGUI.DisabledScope(!canStart))
            {
                if (GUILayout.Button(
                        "Start Focus Session",
                        GUILayout.Height(36f)))
                {
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
                isRunning ? "FOCUS SESSION ACTIVE" : "SESSION PAUSED",
                EditorStyles.boldLabel);

            EditorGUILayout.LabelField(
                FormatDuration(
                    DeverQuestSessionStore.GetFocusedSeconds()),
                timerStyle);

            EditorGUILayout.LabelField(
                $"Paused: {FormatDuration(DeverQuestSessionStore.GetPausedSeconds())}",
                subtitleStyle);

            if (isRunning &&
                DeverQuestSettingsStore.Profile.idleDetectionEnabled)
            {
                string idleStatus = DeverQuestIdleMonitor.IsSupported
                    ? $"Input idle: {FormatShortDuration(DeverQuestIdleMonitor.CurrentIdleSeconds)}"
                    : "Idle detection is unavailable on this platform.";

                EditorGUILayout.LabelField(
                    idleStatus,
                    subtitleStyle);
            }

            EditorGUILayout.Space(10f);

            DrawReadOnlyValue("Project", session.projectName);
            DrawReadOnlyValue("Task", session.taskName);
            DrawReadOnlyValue("Category", session.category);
            DrawReadOnlyValue(
                "Started",
                DeverQuestSessionStore
                    .GetLocalStartTime(session)
                    .ToString("g"));

            if (!string.IsNullOrWhiteSpace(session.goal))
            {
                EditorGUILayout.Space(6f);
                EditorGUILayout.LabelField(
                    "Goal",
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
                    $"Paused by: {session.pauseReason}",
                    session.pauseReason == "Idle Detection"
                        ? MessageType.Warning
                        : MessageType.Info);
            }

            EditorGUILayout.Space(12f);

            if (showFinalization)
            {
                DrawFinalizationPanel(session);
                return;
            }

            EditorGUILayout.BeginHorizontal();

            if (isRunning)
            {
                if (GUILayout.Button(
                        "Pause",
                        GUILayout.Height(32f)))
                {
                    DeverQuestSessionStore.PauseSession();
                    Repaint();
                }
            }
            else if (GUILayout.Button(
                         "Resume",
                         GUILayout.Height(32f)))
            {
                DeverQuestSessionStore.ResumeSession();
                Repaint();
            }

            if (GUILayout.Button(
                    "End Session",
                    GUILayout.Height(32f)))
            {
                BeginFinalization(session);
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Discard Session"))
            {
                bool discard = EditorUtility.DisplayDialog(
                    "Discard Current Session?",
                    "This removes the current session without keeping its " +
                    "focused time. This cannot be undone.",
                    "Discard",
                    "Keep Session");

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
                "Commit Journal",
                EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Commit Details");
            commitComment = EditorGUILayout.TextArea(
                commitComment,
                GUILayout.MinHeight(46f));

            EditorGUILayout.BeginHorizontal();
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
                if (GUILayout.Button("Add Commit Entry"))
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

        private void BeginFinalization(
            DeverQuestSession session)
        {
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
                "Finalize Session",
                EditorStyles.boldLabel);

            EditorGUILayout.LabelField(
                $"Focused: {FormatDuration(DeverQuestSessionStore.GetFocusedSeconds())}");

            EditorGUILayout.LabelField("Closing Notes");
            closingNotes = EditorGUILayout.TextArea(
                closingNotes,
                GUILayout.MinHeight(70f));

            EditorGUILayout.HelpBox(
                "Finalizing will write or update today's Markdown timecard.",
                MessageType.Info);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(
                    "Finalize and Write Timecard",
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
