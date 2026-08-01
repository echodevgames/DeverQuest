//----- DeverQuestWindow.cs START -----

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal sealed class DeverQuestWindow : EditorWindow
    {
        private enum DeverQuestWorkspace
        {
            Quest = 0,
            QuestLog = 1,
            Character = 2,
            GuildHall = 3,
            RewardsHistory = 4,
            AudioWellness = 5,
            Settings = 6
        }

        private enum QuestTurnInStep
        {
            Chronicle = 0,
            Rewards = 1
        }

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
        private DeverQuestQuestProfile selectedQuestProfile;
        private string appliedQuestProfileId = string.Empty;
        private DeverQuestQuestContract selectedQuestContract;
        private string appliedQuestContractId = string.Empty;
        private string commitComment = string.Empty;
        private string commitBranch = string.Empty;
        private string commitHash = string.Empty;
        private string closingNotes = string.Empty;
        private bool showFinalization;
        private string rewardMessage = string.Empty;
        private bool historyFoldout;
        private bool hallOfHeroesFoldout = true;
        private bool contractBoardFoldout = true;
        private bool guildAdministrationFoldout = true;
        private bool guildCollapsedForActiveQuest;
        private DeverQuestHistoryRange historyRange =
            DeverQuestHistoryRange.Last7Days;
        private string historyProjectFilter = string.Empty;
        private string historyCategoryFilter = string.Empty;
        private string historyStartDate =
            DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd");
        private string historyEndDate =
            DateTime.Now.ToString("yyyy-MM-dd");
        private string historyMessage = string.Empty;
        private string correctionDataPath = string.Empty;
        private string correctionSessionId = string.Empty;
        private string correctionSessionTitle = string.Empty;
        private string correctionReason = string.Empty;
        private string correctionValue = string.Empty;
        private string focusScheduleText = string.Empty;
        private string guildLoginName = string.Empty;
        private string guildPasscode = string.Empty;
        private string newGuildDeveloper = string.Empty;
        private string newGuildCharacter = string.Empty;
        private string newGuildClass = "Warrior";
        private DeverQuestClassDefinition newGuildClassDefinition;
        private string newGuildRank = "Member";
        private string newGuildProjects = string.Empty;
        private string newGuildPasscode = string.Empty;
        private string guildMessage = string.Empty;
        private bool compensationPreviewFoldout = true;
        private bool compensationAdministrationFoldout;
        private int compensationAccountIndex;
        private string compensationLoadedAccountId = string.Empty;
        private bool compensationEnabled;
        private DeverQuestCompensationBasis compensationBasis =
            DeverQuestCompensationBasis.Hourly;
        private string compensationCurrencyCode = "USD";
        private double compensationHourlyRate;
        private double compensationAnnualSalary;
        private double compensationWeeklyHours = 40d;
        private bool compensationIncludeApprovedBreaks;
        private DeverQuestCompensationIntegrityPolicy
            compensationIntegrityPolicy =
                DeverQuestCompensationIntegrityPolicy
                    .VerifiedChroniclesOnly;
        private DeverQuestAbility rulesAbility =
            DeverQuestAbility.Intelligence;
        private int rulesDifficultyClass = 12;
        private bool rulesProficient = true;
        private string rulesSeed = "Ajnaag-Test-1";
        private string rulesResult = string.Empty;
        private DeverQuestEquipment selectedRulesEquipment;
        private DeverQuestSpell selectedRulesSpell;
        private DeverQuestCompanionProfile selectedCompanionProfile;
        private bool companionStableFoldout = true;
        private string companionMessage = string.Empty;
        private DeverQuestShopProfile selectedShopProfile;
        private bool guildShopFoldout = true;
        private bool purchaseHistoryFoldout;
        private bool tradeLedgerFoldout;
        private int tradeTargetIndex;
        private string fulfillmentReference = string.Empty;
        private string shopMessage = string.Empty;
        private bool contentScaffoldFoldout = true;
        private string contentScaffoldMessage = string.Empty;
        private string creationCharacterName = string.Empty;
        private DeverQuestAncestry creationAncestry;
        private DeverQuestClassDefinition creationClassDefinition;
        private DeverQuestDeity creationFaith;
        private DeverQuestAlignment creationAlignment =
            DeverQuestAlignment.TrueNeutral;
        private bool identityCatalogGenerationQueued;
        private DeverQuestGitStatus gitStatus;
        private string gitMessage = string.Empty;
        private bool gitOperationInProgress;
        private string voiceMemoName = "Quest Memo";
        private int selectedMicrophoneIndex;
        private string mediaMessage = string.Empty;
        private QuestTurnInStep turnInStep =
            QuestTurnInStep.Chronicle;
        private DeverQuestWorkspace activeWorkspace =
            DeverQuestWorkspace.Quest;
        private double nextSessionRepaintTime;

        [MenuItem("Tools/DeverQuest/Developer Companion")]
        public static void Open()
        {
            DeverQuestWindow window =
                GetWindow<DeverQuestWindow>("DeverQuest");

            window.minSize =
                new Vector2(MinimumWindowWidth, MinimumWindowHeight);

            window.Show();
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Current Quest")]
        private static void OpenQuestWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.Quest);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Quest Log and Git")]
        private static void OpenQuestLogWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.QuestLog);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Character Sheet")]
        private static void OpenCharacterWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.Character);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Guild Hall")]
        private static void OpenGuildWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.GuildHall);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Rewards and History")]
        private static void OpenHistoryWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.RewardsHistory);
        }

        private static void OpenWorkspace(
            DeverQuestWorkspace workspace)
        {
            DeverQuestWindow window =
                GetWindow<DeverQuestWindow>("DeverQuest");
            window.activeWorkspace = workspace;
            window.minSize =
                new Vector2(
                    MinimumWindowWidth,
                    MinimumWindowHeight);
            window.Show();
            window.Focus();
            window.Repaint();
        }

        internal static void ShowIdleWarning(int secondsRemaining)
        {
            ShowConfiguredNotification(
                $"Still working? Idle pause in about " +
                $"{secondsRemaining} seconds.",
                4d,
                DeverQuestAudioCue.IdleWarning);
        }

        internal static void ShowIdlePaused()
        {
            ShowConfiguredNotification(
                "Quest entered meditation because no activity was detected.",
                6d,
                DeverQuestAudioCue.IdlePaused);
        }

        internal static void ShowWellnessReminder(
            string title,
            DeverQuestWellnessType type)
        {
            DeverQuestAudioCue cue;
            switch (type)
            {
                case DeverQuestWellnessType.Hydration:
                    cue = DeverQuestAudioCue.Hydration;
                    break;
                case DeverQuestWellnessType.MovementBreak:
                case DeverQuestWellnessType.Exercise:
                    cue = DeverQuestAudioCue.MovementBreak;
                    break;
                case DeverQuestWellnessType.Lunch:
                case DeverQuestWellnessType.Dinner:
                    cue = DeverQuestAudioCue.MealReminder;
                    break;
                default:
                    cue = DeverQuestAudioCue.FocusCheckIn;
                    break;
            }
            ShowConfiguredNotification(
                $"DeverQuest: {title}", 6d, cue);
        }

        private static void ShowConfiguredNotification(
            string message,
            double duration,
            DeverQuestAudioCue cue)
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
                if (!DeverQuestAudioDirector.PlayCue(cue))
                {
                    EditorApplication.Beep();
                }
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
            if (profile.sharedGuildEnabled)
            {
                DeverQuestSharedGuildService.Refresh();
            }
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
                if (!DeverQuestGuildAccountService.IsAuthenticated)
                {
                    DrawGuildLogin();
                }
                else if (!DeverQuestGuildAccountService.CurrentAccount
                             .characterCreationComplete)
                {
                    DrawCharacterCreation();
                }
                else if (profile.compactMode)
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

        private void DrawGuildLogin()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Enter the Guild Hall",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Choose your administrator-created Guild account and enter " +
                "its local passcode.",
                MessageType.Info);
            string[] names = DeverQuestGuildAccountService.Accounts
                .Where(account => !account.disabled)
                .Select(account => account.developerName)
                .ToArray();
            int index = Mathf.Max(0,
                Array.IndexOf(names, guildLoginName));
            if (names.Length > 0)
            {
                guildLoginName = names[
                    EditorGUILayout.Popup("Developer", index, names)];
            }
            guildPasscode = EditorGUILayout.PasswordField(
                "Passcode", guildPasscode);
            if (GUILayout.Button("Enter Guild Hall"))
            {
                if (DeverQuestGuildAccountService.Login(
                        guildLoginName,
                        guildPasscode,
                        out string error))
                {
                    guildPasscode = string.Empty;
                    guildMessage = "Guild identity verified.";
                }
                else
                {
                    guildMessage = error;
                }
            }
            if (!string.IsNullOrWhiteSpace(guildMessage))
            {
                EditorGUILayout.HelpBox(
                    guildMessage,
                    MessageType.Warning);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCharacterCreation()
        {
            EnsureCharacterCreationDefaults();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Create Your Adventurer",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Choose carefully. Your name, Ancestry, Class, Alignment, " +
                "Faith, and home Department lock when you enter the Guild.",
                MessageType.Info);
            creationCharacterName = EditorGUILayout.TextField(
                "Adventurer Name", creationCharacterName);
            creationAncestry =
                (DeverQuestAncestry)EditorGUILayout.ObjectField(
                    "Ancestry",
                    creationAncestry,
                    typeof(DeverQuestAncestry),
                    false);
            creationClassDefinition =
                (DeverQuestClassDefinition)
                EditorGUILayout.ObjectField(
                    "Class",
                    creationClassDefinition,
                    typeof(DeverQuestClassDefinition),
                    false);
            creationAlignment =
                (DeverQuestAlignment)EditorGUILayout.EnumPopup(
                    "Alignment", creationAlignment);
            creationFaith =
                (DeverQuestDeity)EditorGUILayout.ObjectField(
                    "Faith",
                    creationFaith,
                    typeof(DeverQuestDeity),
                    false);
            string creationDepartment =
                creationClassDefinition?.department ?? "Unassigned";
            EditorGUILayout.LabelField(
                "Home Department", creationDepartment);
            EditorGUILayout.HelpBox(
                "Your Class asset determines your starting Department. " +
                "Dungeon Masters can use Contract eligibility rules " +
                "to build Class-, Ancestry-, and Department-specific Quests.",
                MessageType.Info);
            bool eligible =
                DeverQuestIdentityCatalogService.IsEligible(
                    creationAncestry,
                    creationClassDefinition,
                    out string eligibilityReason);
            if (eligible)
            {
                eligible =
                    DeverQuestIdentityCatalogService.IsFaithEligible(
                        creationFaith,
                        creationAncestry,
                        creationClassDefinition,
                        creationAlignment,
                        out eligibilityReason);
            }
            if (!eligible)
            {
                EditorGUILayout.HelpBox(
                    eligibilityReason,
                    MessageType.Warning);
            }
            DeverQuestAdventurer preview =
                new DeverQuestAdventurer();
            DeverQuestIdentityCatalogService.ApplyIdentityFoundation(
                preview,
                creationAncestry,
                creationClassDefinition,
                creationFaith,
                creationAlignment,
                true);
            EditorGUILayout.LabelField(
                "Identity Foundation",
                $"STR {preview.strength} · DEX {preview.dexterity} · " +
                $"CON {preview.constitution} · INT {preview.intelligence} · " +
                $"WIS {preview.wisdom} · CHA {preview.charisma}");
            EditorGUILayout.LabelField(
                "Extended Attributes",
                $"AGI {preview.agility} · STA {preview.stamina} · " +
                $"Luck {preview.luck} · HP {preview.maximumHitPoints} · " +
                $"Mana {preview.maximumMana}");
            if (creationClassDefinition != null &&
                creationClassDefinition.supportsCompanion)
            {
                EditorGUILayout.LabelField(
                    "Companion Tradition",
                    string.IsNullOrWhiteSpace(
                        creationClassDefinition.companionTradition)
                        ? "Supported"
                        : creationClassDefinition.companionTradition);
                if (creationClassDefinition.starterCompanion != null)
                {
                    EditorGUILayout.LabelField(
                        "Starting Companion",
                        creationClassDefinition
                            .starterCompanion.displayName);
                }
            }
            using (new EditorGUI.DisabledScope(
                       !eligible ||
                       string.IsNullOrWhiteSpace(
                           creationCharacterName)))
            {
                if (GUILayout.Button(
                        "Enter the Guild",
                        GUILayout.Height(34f)))
                {
                    if (!DeverQuestGuildAccountService
                            .CompleteCharacterCreation(
                                creationCharacterName,
                                creationAncestry,
                                creationClassDefinition,
                                creationFaith,
                                creationAlignment,
                                out string error))
                    {
                        guildMessage = error;
                    }
                }
            }
            DeverQuestIdentityCatalog activeIdentityCatalog =
                DeverQuestIdentityCatalogService.ActiveCatalog;
            bool identityCatalogReady =
                activeIdentityCatalog != null &&
                activeIdentityCatalog.defaultAncestry != null &&
                activeIdentityCatalog.defaultClass != null &&
                (activeIdentityCatalog.ancestries?.Count ?? 0) > 0 &&
                (activeIdentityCatalog.classes?.Count ?? 0) > 0;
            if (!identityCatalogReady)
            {
                EditorGUILayout.HelpBox(
                    "No complete playable Identity Catalog is installed. " +
                    "Generate or repair the original starter catalog before " +
                    "finishing character creation.",
                    MessageType.Warning);
                bool canGenerate =
                    DeverQuestGuildAccountService.HasPermission(
                        DeverQuestGuildPermission.ManageGuild);
                using (new EditorGUI.DisabledScope(
                           !canGenerate ||
                           identityCatalogGenerationQueued))
                {
                    if (GUILayout.Button(
                            identityCatalogGenerationQueued
                                ? "Generating Starter Identity Catalog…"
                                : "Generate Original Starter Identity Catalog"))
                    {
                        QueueOriginalStarterIdentityCatalogGeneration();
                    }
                }
                if (!canGenerate)
                {
                    EditorGUILayout.HelpBox(
                        "Ask a CEO or Boss to generate or assign the Guild's " +
                        "Identity Catalog.",
                        MessageType.Info);
                }
            }
            if (!string.IsNullOrWhiteSpace(guildMessage))
            {
                EditorGUILayout.HelpBox(
                    guildMessage, MessageType.Warning);
            }
            EditorGUILayout.EndVertical();
        }

        private void QueueOriginalStarterIdentityCatalogGeneration(
            bool reportToContentScaffolding = false)
        {
            if (identityCatalogGenerationQueued)
            {
                return;
            }

            identityCatalogGenerationQueued = true;
            string preparingMessage =
                "Preparing the original starter Identity Catalog…";
            if (reportToContentScaffolding)
            {
                contentScaffoldMessage = preparingMessage;
            }
            else
            {
                guildMessage = preparingMessage;
            }
            Repaint();

            EditorApplication.delayCall += () =>
            {
                try
                {
                    DeverQuestIdentityGenerationReport report =
                        DeverQuestIdentityCatalogGenerator
                            .GenerateOriginalStarterCatalog();
                    string resultMessage = report.Summary;
                    if (reportToContentScaffolding)
                    {
                        contentScaffoldMessage = resultMessage;
                    }
                    else
                    {
                        guildMessage = resultMessage;
                    }

                    if (!report.Succeeded || report.Catalog == null)
                    {
                        return;
                    }

                    creationAncestry =
                        report.Catalog.defaultAncestry;
                    creationClassDefinition =
                        report.Catalog.defaultClass;
                    creationFaith =
                        report.Catalog.defaultFaith;
                    Selection.activeObject = report.Catalog;
                    EditorGUIUtility.PingObject(report.Catalog);
                    SelectGeneratedFolder(report.RootPath);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                    string failureMessage =
                        "Starter Identity Catalog generation failed: " +
                        exception.Message;
                    if (reportToContentScaffolding)
                    {
                        contentScaffoldMessage = failureMessage;
                    }
                    else
                    {
                        guildMessage = failureMessage;
                    }
                }
                finally
                {
                    identityCatalogGenerationQueued = false;
                    Repaint();
                }
            };
        }

        private void EnsureCharacterCreationDefaults()
        {
            DeverQuestIdentityCatalog catalog =
                DeverQuestIdentityCatalogService.ActiveCatalog;
            if ((creationAncestry == null ||
                 (catalog != null &&
                  !(catalog.ancestries ??
                    new List<DeverQuestAncestry>())
                      .Contains(creationAncestry))) &&
                DeverQuestIdentityCatalogService.Ancestries.Count > 0)
            {
                creationAncestry =
                    catalog?.defaultAncestry ??
                    DeverQuestIdentityCatalogService.Ancestries[0];
            }
            if ((creationClassDefinition == null ||
                 (catalog != null &&
                  !(catalog.classes ??
                    new List<DeverQuestClassDefinition>())
                      .Contains(creationClassDefinition))) &&
                DeverQuestIdentityCatalogService.Classes.Count > 0)
            {
                creationClassDefinition =
                    catalog?.defaultClass ??
                    DeverQuestIdentityCatalogService.Classes
                        .FirstOrDefault(value =>
                            value.displayName == "Warrior") ??
                    DeverQuestIdentityCatalogService.Classes[0];
            }
            if ((creationFaith == null ||
                 (catalog != null &&
                  !(catalog.faiths ??
                    new List<DeverQuestDeity>())
                      .Contains(creationFaith))) &&
                DeverQuestIdentityCatalogService.Faiths.Count > 0)
            {
                creationFaith =
                    catalog?.defaultFaith ??
                    DeverQuestIdentityCatalogService.Faiths
                        .FirstOrDefault(value =>
                            value.displayName == "Agnostic") ??
                    DeverQuestIdentityCatalogService.Faiths[0];
            }
        }

        private void DrawGuildAdministration()
        {
            DeverQuestGuildAccount account =
                DeverQuestGuildAccountService.CurrentAccount;
            if (account == null)
            {
                return;
            }
            guildAdministrationFoldout = EditorGUILayout.Foldout(
                guildAdministrationFoldout,
                "Guild Accounts and Authority",
                true);
            if (!guildAdministrationFoldout)
            {
                return;
            }
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                $"{account.developerName} · {account.guildRank}",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Assigned Projects",
                account.assignedProjects.Count == 0
                    ? "All Guild projects (rank permitting)"
                    : string.Join(", ", account.assignedProjects));

            if (DeverQuestGuildAccountService.RequiresPasscodeSetup)
            {
                EditorGUILayout.HelpBox(
                    "Your existing Adventurer was migrated as the founding " +
                    "CEO. Set a local passcode now to lock the identity and " +
                    "permissions.",
                    MessageType.Warning);
                guildPasscode = EditorGUILayout.PasswordField(
                    "New Passcode", guildPasscode);
                if (GUILayout.Button("Secure Founding Account"))
                {
                    if (DeverQuestGuildAccountService
                        .SecureCurrentAccount(
                            guildPasscode, out string error))
                    {
                        guildPasscode = string.Empty;
                        guildMessage =
                            "Founding CEO account secured.";
                    }
                    else
                    {
                        guildMessage = error;
                    }
                }
            }

            if (DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild))
            {
                EditorGUILayout.Space(6f);
                EditorGUILayout.LabelField(
                    "Create Adventurer Account",
                    EditorStyles.boldLabel);
                newGuildDeveloper = EditorGUILayout.TextField(
                    "Developer", newGuildDeveloper);
                newGuildCharacter = EditorGUILayout.TextField(
                    "Adventurer", newGuildCharacter);
                if (DeverQuestIdentityCatalogService.Classes.Count > 0)
                {
                    if (newGuildClassDefinition == null)
                    {
                        newGuildClassDefinition =
                            DeverQuestIdentityCatalogService.Classes
                                .FirstOrDefault(value =>
                                    value.displayName ==
                                    newGuildClass) ??
                            DeverQuestIdentityCatalogService.Classes[0];
                    }
                    newGuildClassDefinition =
                        (DeverQuestClassDefinition)
                        EditorGUILayout.ObjectField(
                            "Class",
                            newGuildClassDefinition,
                            typeof(DeverQuestClassDefinition),
                            false);
                    if (newGuildClassDefinition != null)
                    {
                        newGuildClass =
                            newGuildClassDefinition.displayName;
                    }
                }
                else
                {
                    newGuildClass = DrawStringPopup(
                        "Legacy Class", newGuildClass,
                        DeverQuestAdventurerService.Classes);
                }
                newGuildRank = DrawStringPopup(
                    "Guild Rank", newGuildRank,
                    DeverQuestAdventurerService.GuildRanks);
                newGuildProjects = EditorGUILayout.TextField(
                    new GUIContent(
                        "Projects",
                        "Comma-separated Project assignments. Required for " +
                        "Project Leaders."),
                    newGuildProjects);
                newGuildPasscode = EditorGUILayout.PasswordField(
                    "Temporary Passcode", newGuildPasscode);
                if (GUILayout.Button("Create Guild Account"))
                {
                    if (DeverQuestGuildAccountService.CreateAccount(
                            newGuildDeveloper,
                            newGuildCharacter,
                            newGuildClass,
                            newGuildRank,
                            newGuildProjects.Split(','),
                            newGuildPasscode,
                            out string error))
                    {
                        guildMessage =
                            $"Created account for {newGuildDeveloper}.";
                        newGuildDeveloper = string.Empty;
                        newGuildCharacter = string.Empty;
                        newGuildProjects = string.Empty;
                        newGuildPasscode = string.Empty;
                    }
                    else
                    {
                        guildMessage = error;
                    }
                }

                DrawCompensationAdministration();
            }

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(
                "Recent Authority Audit",
                EditorStyles.boldLabel);
            foreach (DeverQuestGuildAuditEntry entry
                     in DeverQuestGuildAccountService.AuditEntries.Take(8))
            {
                EditorGUILayout.LabelField(
                    $"{entry.actorName}: {entry.action} → {entry.target}",
                    EditorStyles.wordWrappedMiniLabel);
            }

            if (!string.IsNullOrWhiteSpace(guildMessage))
            {
                EditorGUILayout.HelpBox(
                    guildMessage, MessageType.Info);
            }
            if (GUILayout.Button("Leave Guild Hall"))
            {
                DeverQuestGuildAccountService.Logout();
                guildMessage = string.Empty;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private void DrawCompensationAdministration()
        {
            compensationAdministrationFoldout =
                EditorGUILayout.Foldout(
                    compensationAdministrationFoldout,
                    "Compensation Preview Policies",
                    true);
            if (!compensationAdministrationFoldout)
            {
                return;
            }

            List<DeverQuestGuildAccount> accounts =
                DeverQuestGuildAccountService.Accounts
                    .Where(value => value != null)
                    .ToList();
            if (accounts.Count == 0)
            {
                return;
            }

            compensationAccountIndex = Mathf.Clamp(
                compensationAccountIndex,
                0,
                accounts.Count - 1);
            compensationAccountIndex = EditorGUILayout.Popup(
                "Adventurer Account",
                compensationAccountIndex,
                accounts
                    .Select(value => value.developerName)
                    .ToArray());
            DeverQuestGuildAccount target =
                accounts[compensationAccountIndex];
            LoadCompensationPolicy(target);

            compensationEnabled = EditorGUILayout.Toggle(
                "Enable Preview",
                compensationEnabled);
            compensationBasis =
                (DeverQuestCompensationBasis)
                EditorGUILayout.EnumPopup(
                    "Basis",
                    compensationBasis);
            compensationCurrencyCode = EditorGUILayout.TextField(
                new GUIContent(
                    "Currency Code",
                    "Three-letter display code, such as USD, CAD, EUR, or " +
                    "GBP."),
                compensationCurrencyCode);
            if (compensationBasis ==
                DeverQuestCompensationBasis.Hourly)
            {
                compensationHourlyRate = EditorGUILayout.DoubleField(
                    "Hourly Rate",
                    compensationHourlyRate);
            }
            else
            {
                compensationAnnualSalary = EditorGUILayout.DoubleField(
                    "Annual Salary",
                    compensationAnnualSalary);
                compensationWeeklyHours = EditorGUILayout.DoubleField(
                    "Scheduled Hours/Week",
                    compensationWeeklyHours);
            }
            compensationIncludeApprovedBreaks =
                EditorGUILayout.Toggle(
                    new GUIContent(
                        "Include Approved Breaks",
                        "When enabled, completed approved-break seconds are " +
                        "included. Meditation and Idle/Unverified time are " +
                        "always excluded."),
                    compensationIncludeApprovedBreaks);
            compensationIntegrityPolicy =
                (DeverQuestCompensationIntegrityPolicy)
                EditorGUILayout.EnumPopup(
                    new GUIContent(
                        "Chronicle Eligibility",
                        "Modified or unavailable Chronicles are always " +
                        "excluded."),
                    compensationIntegrityPolicy);

            EditorGUILayout.HelpBox(
                "Compensation settings are a local planning convenience. " +
                "They are not encrypted payroll records and are never " +
                "published to shared Guild snapshots or written into daily " +
                "Chronicles.",
                MessageType.Warning);

            if (GUILayout.Button("Save Preview Policy"))
            {
                if (DeverQuestGuildAccountService
                    .UpdateCompensationPolicy(
                        target.accountId,
                        compensationEnabled,
                        compensationBasis,
                        compensationCurrencyCode,
                        compensationHourlyRate,
                        compensationAnnualSalary,
                        compensationWeeklyHours,
                        compensationIncludeApprovedBreaks,
                        compensationIntegrityPolicy,
                        out string error))
                {
                    compensationLoadedAccountId = string.Empty;
                    guildMessage =
                        $"Saved compensation preview policy for " +
                        $"{target.developerName}.";
                }
                else
                {
                    guildMessage = error;
                }
            }
        }

        private void LoadCompensationPolicy(
            DeverQuestGuildAccount account)
        {
            if (account == null ||
                compensationLoadedAccountId == account.accountId)
            {
                return;
            }
            compensationLoadedAccountId = account.accountId;
            compensationEnabled =
                account.compensationPreviewEnabled;
            compensationBasis = account.compensationBasis;
            compensationCurrencyCode =
                account.compensationCurrencyCode;
            compensationHourlyRate =
                account.compensationHourlyRate;
            compensationAnnualSalary =
                account.compensationAnnualSalary;
            compensationWeeklyHours =
                account.compensationWeeklyHours;
            compensationIncludeApprovedBreaks =
                account.compensationIncludeApprovedBreaks;
            compensationIntegrityPolicy =
                account.compensationIntegrityPolicy;
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
            DrawExternalActivitySetup();
            DrawRewardSetup(profile);
            DrawCampaignRulesSetup(profile);
            DrawPlaylistSetup(profile);
            DrawPolishSetup(profile);
            DrawChronicleIntegritySetup(profile);
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
            DeverQuestGuildAccountService
                .RefreshUnsecuredFounderIdentity(
                    profile.developerName);

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

            EditorGUILayout.Space(6f);
            DrawWorkspaceTabs();
            EditorGUILayout.Space(8f);

            if (DeverQuestSessionStore.HasActiveSession &&
                !guildCollapsedForActiveQuest)
            {
                guildAdministrationFoldout = false;
                guildCollapsedForActiveQuest = true;
            }
            else if (!DeverQuestSessionStore.HasActiveSession)
            {
                guildCollapsedForActiveQuest = false;
            }
            switch (activeWorkspace)
            {
                case DeverQuestWorkspace.Quest:
                    DrawGoalsAndStreaks(profile);
                    DrawWellnessReminder();
                    if (DeverQuestSessionStore.HasActiveSession)
                    {
                        DrawActiveSession(false);
                    }
                    else
                    {
                        DrawNewSessionForm(profile);
                        DrawLastCompletedSession();
                    }
                    break;
                case DeverQuestWorkspace.QuestLog:
                    DrawQuestLogWorkspace();
                    break;
                case DeverQuestWorkspace.Character:
                    DrawAdventurerSheet();
                    DrawCompanionStable();
                    DrawRulesLaboratory(profile);
                    break;
                case DeverQuestWorkspace.GuildHall:
                    DrawContentScaffolding();
                    DrawGuildShop();
                    DrawGuildAdministration();
                    DrawHallOfHeroes(profile);
                    break;
                case DeverQuestWorkspace.RewardsHistory:
                    DrawRewardsPanel(profile);
                    DrawHistoryPanel(profile);
                    break;
                case DeverQuestWorkspace.AudioWellness:
                    DrawWellnessReminder();
                    DrawPlaylistPlayer();
                    break;
                case DeverQuestWorkspace.Settings:
                    DrawProfileControls(profile);
                    break;
            }
        }

        private void DrawContentScaffolding()
        {
            contentScaffoldFoldout = EditorGUILayout.Foldout(
                contentScaffoldFoldout,
                "Campaign Content Scaffolding",
                true);
            if (!contentScaffoldFoldout)
            {
                return;
            }
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Studio Content Generator",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Create an organized production tree with blank template " +
                "assets, or generate an isolated, interconnected tutorial " +
                "campaign. Existing folders and assets are always preserved.",
                EditorStyles.wordWrappedLabel);

            bool canManage =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);
            using (new EditorGUI.DisabledScope(!canManage))
            {
                DeverQuestIdentityCatalog activeCatalog =
                    (DeverQuestIdentityCatalog)
                    EditorGUILayout.ObjectField(
                        "Active Identity Catalog",
                        DeverQuestIdentityCatalogService.ActiveCatalog,
                        typeof(DeverQuestIdentityCatalog),
                        false);
                if (activeCatalog !=
                    DeverQuestIdentityCatalogService.ActiveCatalog)
                {
                    try
                    {
                        DeverQuestIdentityCatalogService.SetActiveCatalog(
                            activeCatalog);
                        DeverQuestIdentityCatalog selectedCatalog =
                            DeverQuestIdentityCatalogService.ActiveCatalog;
                        contentScaffoldMessage = selectedCatalog == null
                            ? "No Identity Catalog assets are available."
                            : $"Active Guild Identity Catalog: " +
                              $"{selectedCatalog.displayName}.";
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                        contentScaffoldMessage =
                            "The active Identity Catalog could not be saved: " +
                            exception.Message;
                    }
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(
                        "Create Empty Studio Structure"))
                {
                    DeverQuestScaffoldReport report =
                        DeverQuestCampaignScaffolder
                            .CreateProductionStructure();
                    contentScaffoldMessage = report.Summary;
                    SelectGeneratedFolder(
                        report.SelectedFolder);
                }
                if (GUILayout.Button(
                        "Create Tutorial Campaign"))
                {
                    bool create = EditorUtility.DisplayDialog(
                        "Create DeverQuest Tutorial Campaign?",
                        "This creates connected tutorial assets under " +
                        "Assets/DeverQuest/DemoCampaign. Existing assets " +
                        "will not be replaced.",
                        "Create Tutorial",
                        "Cancel");
                    if (create)
                    {
                        DeverQuestScaffoldReport report =
                            DeverQuestCampaignScaffolder
                                .CreateTutorialCampaign();
                        selectedQuestContract =
                            report.TutorialContract;
                        selectedShopProfile =
                            report.TutorialShop;
                        contentScaffoldMessage =
                            report.Summary +
                            " The tutorial Contract and Quartermaster " +
                            "are now selected.";
                        SelectGeneratedFolder(
                            report.SelectedFolder);
                    }
                }
                EditorGUILayout.EndHorizontal();
                using (new EditorGUI.DisabledScope(
                           identityCatalogGenerationQueued))
                {
                    if (GUILayout.Button(
                            identityCatalogGenerationQueued
                                ? "Generating Starter Identity Catalog…"
                                : "Generate Original Starter Identity Catalog"))
                    {
                        QueueOriginalStarterIdentityCatalogGeneration(
                            true);
                    }
                }
                if (GUILayout.Button(
                        "Generate Original Companion Stable"))
                {
                    DeverQuestCompanionGenerationReport report =
                        DeverQuestCompanionCatalogGenerator
                            .GenerateOriginalStarterCatalog();
                    contentScaffoldMessage = report.Summary;
                    SelectGeneratedFolder(report.RootPath);
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Ancestry Asset…"))
                {
                    CreateCharacterAsset<DeverQuestAncestry>(
                        "NewAncestry");
                }
                if (GUILayout.Button("Create Class Definition…"))
                {
                    CreateCharacterAsset<
                        DeverQuestClassDefinition>(
                        "NewClassDefinition");
                }
                if (GUILayout.Button("Create Faith Asset…"))
                {
                    CreateCharacterAsset<DeverQuestDeity>(
                        "NewFaith");
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Companion Profile…"))
                {
                    CreateCharacterAsset<
                        DeverQuestCompanionProfile>(
                        "NewCompanionProfile");
                }
                if (GUILayout.Button("Create Companion Catalog…"))
                {
                    CreateCharacterAsset<
                        DeverQuestCompanionCatalog>(
                        "NewCompanionCatalog");
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Create Identity Catalog…"))
                {
                    CreateCharacterAsset<
                        DeverQuestIdentityCatalog>(
                        "NewGuildIdentityCatalog");
                }
            }
            if (!canManage)
            {
                EditorGUILayout.HelpBox(
                    "CEO or Boss permission is required to generate " +
                    "studio content.",
                    MessageType.Info);
            }
            if (!string.IsNullOrWhiteSpace(
                    contentScaffoldMessage))
            {
                EditorGUILayout.HelpBox(
                    contentScaffoldMessage,
                    MessageType.Info);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private static void SelectGeneratedFolder(
            string path)
        {
            UnityEngine.Object folder =
                AssetDatabase.LoadAssetAtPath<
                    UnityEngine.Object>(path);
            if (folder == null)
            {
                return;
            }
            Selection.activeObject = folder;
            EditorGUIUtility.PingObject(folder);
        }

        private void DrawWorkspaceTabs()
        {
            string[] labels =
            {
                "Quest",
                "Quest Log & Git",
                "Character",
                "Guild Hall",
                "Rewards & History",
                "Audio & Wellness",
                "Settings"
            };
            activeWorkspace = (DeverQuestWorkspace)
                GUILayout.SelectionGrid(
                    (int)activeWorkspace,
                    labels,
                    4,
                    EditorStyles.toolbarButton);
            EditorGUILayout.LabelField(
                "Only the selected workspace is rendered. This keeps " +
                "inactive AssetDatabase, Git, history, and shared-record " +
                "panels off Unity's repaint path.",
                EditorStyles.wordWrappedMiniLabel);
        }

        private void DrawQuestLogWorkspace()
        {
            if (!DeverQuestSessionStore.HasActiveSession)
            {
                EditorGUILayout.HelpBox(
                    "Accept a Quest to open its live Quest Log and Git " +
                    "workspace.",
                    MessageType.Info);
                return;
            }
            DeverQuestSession session =
                DeverQuestSessionStore.ActiveSession;
            EditorGUILayout.LabelField(
                session.taskName,
                EditorStyles.boldLabel);
            DrawReadOnlyValue(
                "Focused",
                FormatDuration(
                    DeverQuestSessionStore.GetFocusedSeconds()));
            DeverQuestSessionStage activeStage =
                DeverQuestSessionStore.CurrentQuestStage();
            if (activeStage != null)
            {
                EditorGUILayout.Space(8f);
                EditorGUILayout.LabelField(
                    "Current Focus Stage",
                    EditorStyles.boldLabel);
                double stageElapsed = Math.Max(
                    0d,
                    DeverQuestSessionStore.GetFocusedSeconds() -
                    activeStage.startedFocusedSeconds);
                EditorGUILayout.LabelField(
                    activeStage.stageTitle,
                    $"{stageElapsed / 60d:0.0} / " +
                    $"{activeStage.focusedMinutesRequired} minutes");
                if (activeStage.survivalMode ||
                    DeverQuestEncounterService.IsSurvival(activeStage))
                {
                    DrawSurvivalControls(activeStage);
                }
                else
                {
                    using (new EditorGUI.DisabledScope(
                               !activeStage.allowEarlyTurnIn))
                    {
                        if (GUILayout.Button(
                                "Report Development Objective Complete"))
                        {
                            DeverQuestSessionStore
                                .CompleteCurrentStageEarly(
                                    out string message);
                            EditorUtility.DisplayDialog(
                                "Quest Stage Pace",
                                message,
                                "Continue");
                        }
                    }
                }
            }
            DrawBattleResults(session);
            DrawCommitJournal(session);
        }

        private void DrawHallOfHeroes(
            DeverQuestProfile profile)
        {
            hallOfHeroesFoldout = EditorGUILayout.Foldout(
                hallOfHeroesFoldout,
                "Shared Guild Records and Hall of Heroes",
                true);
            if (!hallOfHeroesFoldout)
            {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            bool canManage =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);
            using (new EditorGUI.DisabledScope(!canManage))
            {
                EditorGUI.BeginChangeCheck();
                profile.sharedGuildEnabled =
                    EditorGUILayout.Toggle(
                        "Enable Shared Guild",
                        profile.sharedGuildEnabled);
                profile.publishCompletedQuests =
                    EditorGUILayout.Toggle(
                        "Publish Completed Quests",
                        profile.publishCompletedQuests);
                profile.healthyDailyFocusMinutes =
                    EditorGUILayout.IntField(
                        new GUIContent(
                            "Ranking Daily Cap",
                            "Ranked focus is capped per day to avoid " +
                            "rewarding unhealthy hours."),
                        profile.healthyDailyFocusMinutes);
                if (EditorGUI.EndChangeCheck())
                {
                    profile.Sanitize();
                    DeverQuestSettingsStore.Save();
                }
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Repository");
            EditorGUILayout.SelectableLabel(
                string.IsNullOrWhiteSpace(
                    profile.sharedGuildRepositoryPath)
                    ? "Not configured"
                    : profile.sharedGuildRepositoryPath,
                EditorStyles.textField,
                GUILayout.Height(
                    EditorGUIUtility.singleLineHeight));
            using (new EditorGUI.DisabledScope(!canManage))
            {
                if (GUILayout.Button(
                        "Choose…",
                        GUILayout.Width(72f)))
                {
                    string selected = EditorUtility.OpenFolderPanel(
                        "Choose Shared DeverQuest Guild Repository",
                        string.IsNullOrWhiteSpace(
                            profile.sharedGuildRepositoryPath)
                            ? DeverQuestPathUtility
                                .GetDefaultTimecardRoot()
                            : profile.sharedGuildRepositoryPath,
                        string.Empty);
                    if (!string.IsNullOrWhiteSpace(selected))
                    {
                        profile.sharedGuildRepositoryPath =
                            selected;
                        profile.sharedGuildEnabled = true;
                        DeverQuestSettingsStore.Save();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(
                "This repository coordinates tamper-evident shared records. " +
                "It becomes authoritative only when the studio controls its " +
                "server, network-share, or cloud-folder permissions. Local " +
                "users who can rewrite every file are not cryptographically " +
                "prevented from replacing history.",
                MessageType.Warning);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Validate Repository"))
            {
                DeverQuestSharedGuildService.ValidateRepository(
                    out string message);
                guildMessage = message;
            }
            using (new EditorGUI.DisabledScope(
                       DeverQuestSessionStore
                           .LastCompletedSession == null))
            {
                if (GUILayout.Button("Publish Last Quest"))
                {
                    DeverQuestSharedGuildService
                        .PublishLastCompleted(
                            out string message);
                    guildMessage = message;
                }
            }
            if (GUILayout.Button("Refresh Hall"))
            {
                DeverQuestSharedGuildService.Refresh();
                guildMessage =
                    DeverQuestSharedGuildService.LastMessage;
            }
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrWhiteSpace(guildMessage))
            {
                EditorGUILayout.HelpBox(
                    guildMessage,
                    guildMessage.Contains("failed") ||
                    guildMessage.Contains("unavailable")
                        ? MessageType.Warning
                        : MessageType.Info);
            }

            EditorGUILayout.LabelField(
                $"Published records: " +
                $"{DeverQuestSharedGuildService.PublishedRecordCount} · " +
                $"Invalid/quarantined: " +
                $"{DeverQuestSharedGuildService.InvalidRecordCount}",
                EditorStyles.miniLabel);

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(
                "Hall of Heroes",
                EditorStyles.boldLabel);
            IReadOnlyList<DeverQuestHallEntry> hall =
                DeverQuestSharedGuildService.Hall;
            if (hall.Count == 0)
            {
                EditorGUILayout.LabelField(
                    "No shared Adventurer records loaded.",
                    EditorStyles.miniLabel);
            }
            for (int index = 0; index < hall.Count; index++)
            {
                DeverQuestHallEntry entry = hall[index];
                EditorGUILayout.BeginVertical(
                    EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    $"#{index + 1} {entry.AdventurerName} · " +
                    $"{(string.IsNullOrWhiteSpace(entry.AncestryName) ? string.Empty : entry.AncestryName + " · ")}" +
                    $"{entry.CharacterClass} Level {entry.Level}",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    $"{entry.DeveloperName} · {entry.GuildRank} · " +
                    $"{entry.QuestCount} Quest(s) · " +
                    $"{entry.ContractCount} Contract(s)");
                EditorGUILayout.LabelField(
                    $"Ranked {FormatDuration(entry.RankedFocusedSeconds)} · " +
                    $"Raw {FormatDuration(entry.RawFocusedSeconds)} · " +
                    $"{entry.ExperienceEarned} XP · " +
                    $"{DeverQuestAdventurerService.FormatCoins(entry.CopperEarned)}");
                EditorGUILayout.LabelField(
                    $"Current streak: {entry.CurrentStreak} day(s) · " +
                    $"Review flags: {entry.ReviewFlagCount}",
                    entry.ReviewFlagCount > 0
                        ? EditorStyles.boldLabel
                        : EditorStyles.miniLabel);
                EditorGUILayout.EndVertical();
            }

            DrawGuildNamedReports(
                "Project Standings",
                DeverQuestSharedGuildService.Projects);
            DrawGuildNamedReports(
                "Department Standings",
                DeverQuestSharedGuildService.Departments);
            EditorGUILayout.EndVertical();
        }

        private static void DrawGuildNamedReports(
            string title,
            IReadOnlyList<DeverQuestGuildNamedReport> reports)
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(
                title,
                EditorStyles.boldLabel);
            if (reports.Count == 0)
            {
                EditorGUILayout.LabelField(
                    "No shared data.",
                    EditorStyles.miniLabel);
                return;
            }
            foreach (DeverQuestGuildNamedReport report in reports)
            {
                EditorGUILayout.LabelField(
                    report.Name,
                    $"{FormatDuration(report.FocusedSeconds)} · " +
                    $"{report.QuestCount} Quest(s) · " +
                    $"{report.AdventurerCount} Adventurer(s)");
            }
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
            EditorGUILayout.LabelField(
                "Ambience",
                DeverQuestAudioDirector.CurrentAmbience == null
                    ? "None"
                    : DeverQuestAudioDirector
                        .CurrentAmbience.name);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(
                    DeverQuestAudioDirector.AmbiencePlaying
                        ? "Stop Ambience"
                        : "Play Ambience"))
            {
                if (DeverQuestAudioDirector.AmbiencePlaying)
                {
                    DeverQuestAudioDirector.StopAmbience();
                }
                else
                {
                    DeverQuestAudioDirector.PlayAmbience();
                }
            }
            if (GUILayout.Button("Next Ambience"))
            {
                DeverQuestAudioDirector.NextAmbience();
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
                AnnounceCompletedStages();
                EditorGUILayout.LabelField(
                    FormatDuration(
                        DeverQuestSessionStore.GetFocusedSeconds()),
                    timerStyle);
                EditorGUILayout.LabelField(
                    $"{session.projectName} · {session.taskName}",
                    subtitleStyle);
                DrawCompactQuestLog(session);

                if (showFinalization)
                {
                    DrawFinalizationPanel(session);
                }
                else if (!session.idlePauseAcknowledged)
                {
                    EditorGUILayout.HelpBox(
                        $"This quest entered meditation because: " +
                        $"{session.pauseReason}. Acknowledge your return " +
                        "before resuming.",
                        MessageType.Warning);
                    if (GUILayout.Button(
                            "I Have Returned — Acknowledge"))
                    {
                        DeverQuestSessionStore.AcknowledgeIdlePause();
                        Repaint();
                    }
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
                        DeverQuestSessionStage currentStage =
                            DeverQuestSessionStore.CurrentQuestStage();
                        if (currentStage?.survivalFightPaused == true)
                        {
                            if (DeverQuestSessionStore
                                .ContinueSurvival(
                                    out string continueMessage))
                            {
                                DeverQuestSessionStore.ResumeSession();
                            }
                            else
                            {
                                EditorUtility.DisplayDialog(
                                    "Survival Expedition",
                                    continueMessage,
                                    "Close");
                            }
                        }
                        else
                        {
                            DeverQuestSessionStore.ResumeSession();
                        }
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
                "Coin Purse",
                EditorStyles.boldLabel);
            DeverQuestAdventurer compactAdventurer =
                DeverQuestAdventurerService.Adventurer;
            string compactAncestryName =
                string.IsNullOrWhiteSpace(
                    compactAdventurer.ancestryName)
                    ? "Legacy Ancestry"
                    : compactAdventurer.ancestryName;
            EditorGUILayout.LabelField(
                $"{compactAncestryName} · " +
                $"{compactAdventurer.characterClass} · " +
                $"Level {compactAdventurer.level}",
                DeverQuestAdventurerService.FormatCoins(
                    compactAdventurer.copperBalance));
            EditorGUILayout.LabelField(
                $"HP {compactAdventurer.currentHitPoints}/" +
                $"{compactAdventurer.maximumHitPoints} · " +
                $"Mana {compactAdventurer.currentMana}/" +
                $"{compactAdventurer.maximumMana} · " +
                $"AC {DeverQuestRulesService.ArmorClass(compactAdventurer)}");
            DeverQuestCompanionState compactCompanion =
                DeverQuestCompanionService.ActiveCompanion(
                    compactAdventurer);
            if (compactCompanion != null)
            {
                DeverQuestCompanionProfile compactProfile =
                    DeverQuestCompanionService.FindProfile(
                        compactCompanion.profileId);
                EditorGUILayout.LabelField(
                    "Active Companion",
                    $"{DeverQuestCompanionService.DisplayName(compactCompanion)} · " +
                    $"Level {compactCompanion.level} · " +
                    $"HP {compactCompanion.currentHitPoints}/" +
                    $"{DeverQuestCompanionService.MaximumHitPoints(compactCompanion, compactProfile)}");
            }
            EditorGUILayout.LabelField(
                $"STR {compactAdventurer.strength} · " +
                $"DEX {compactAdventurer.dexterity} · " +
                $"AGI {compactAdventurer.agility} · " +
                $"STA {compactAdventurer.stamina} · " +
                $"INT {compactAdventurer.intelligence} · " +
                $"WIS {compactAdventurer.wisdom} · " +
                $"Luck {compactAdventurer.luck}",
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField(
                $"Hunger {compactAdventurer.hunger} · " +
                $"Rest {compactAdventurer.rest} · " +
                $"Happiness {compactAdventurer.happiness}");
        }

        private void AnnounceCompletedStages()
        {
            foreach (string title in
                     DeverQuestSessionStore.UpdateQuestStages())
            {
                ShowNotification(
                    new GUIContent(
                        $"Focus Stage Complete: {title}"),
                    5d);
                if (DeverQuestSettingsStore.Profile
                    .notificationSoundsEnabled)
                {
                    if (!DeverQuestAudioDirector.PlayCue(
                            DeverQuestAudioCue.StageComplete))
                    {
                        EditorApplication.Beep();
                    }
                }
            }
        }

        private static void DrawCompactQuestLog(
            DeverQuestSession session)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Quest Log",
                EditorStyles.boldLabel);
            if (!string.IsNullOrWhiteSpace(session.questStory))
            {
                EditorGUILayout.LabelField(
                    session.questStory,
                    EditorStyles.wordWrappedLabel);
            }
            EditorGUILayout.LabelField(
                "Objective",
                string.IsNullOrWhiteSpace(session.goal)
                    ? "No objective recorded."
                    : session.goal,
                EditorStyles.wordWrappedLabel);
            if (session.questIsGroupQuest)
            {
                EditorGUILayout.LabelField(
                    $"Party ({session.questMaximumParticipants} max)",
                    session.questPartyMembers);
            }
            if (session.questStages != null)
            {
                foreach (DeverQuestSessionStage stage
                         in session.questStages)
                {
                    EditorGUILayout.LabelField(
                        stage.completed ? "✓ " + stage.stageTitle
                            : "○ " + stage.stageTitle,
                        $"{stage.focusedMinutesRequired}m" +
                        (stage.completedEarly
                            ? " · Early"
                            : string.Empty) +
                        (stage.survivalMode
                            ? $" · Wave {stage.survivalWave}"
                            : string.Empty) +
                        (string.IsNullOrWhiteSpace(
                             stage.assignedPartyRole)
                            ? string.Empty
                            : " · " + stage.assignedPartyRole));
                }
            }
            DeverQuestSessionStage current =
                DeverQuestSessionStore.CurrentQuestStage();
            if (current != null)
            {
                double elapsed = Math.Max(
                    0d,
                    DeverQuestSessionStore.GetFocusedSeconds() -
                    current.startedFocusedSeconds);
                EditorGUILayout.LabelField(
                    "Current Stage Pace",
                    $"{elapsed / 60d:0.0} / " +
                    $"{current.focusedMinutesRequired} focused minutes");
                if (!current.survivalMode &&
                    !DeverQuestEncounterService.IsSurvival(current))
                {
                    using (new EditorGUI.DisabledScope(
                               !current.allowEarlyTurnIn))
                    {
                        if (GUILayout.Button(
                                "Report Development Objective Complete"))
                        {
                            DeverQuestSessionStore
                                .CompleteCurrentStageEarly(
                                    out string paceMessage);
                            EditorUtility.DisplayDialog(
                                "Quest Stage Pace",
                                paceMessage,
                                "Continue");
                        }
                    }
                }
                else
                {
                    DrawSurvivalControls(current);
                }
            }
            DrawBattleResults(session);
            EditorGUILayout.EndVertical();
        }

        private static void DrawSurvivalControls(
            DeverQuestSessionStage stage)
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            float weight =
                DeverQuestEncumbranceService.CarriedWeight(adventurer);
            float capacity =
                DeverQuestEncumbranceService.CarryCapacity(adventurer);
            EditorGUILayout.LabelField(
                "Survival Expedition",
                $"Wave {stage.survivalWave} · Carry " +
                $"{weight:0.0}/{capacity:0.0}");
            if (stage.survivalFightPaused)
            {
                EditorGUILayout.HelpBox(
                    "Fight paused for safety: " +
                    stage.survivalPauseReason,
                    MessageType.Warning);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Attempt Flee"))
            {
                DeverQuestSessionStore.TryExitSurvival(
                    "Flee", out string message);
                EditorUtility.DisplayDialog(
                    "Survival Escape", message, "Close");
            }
            using (new EditorGUI.DisabledScope(
                       !DeverQuestTacticalCombatService
                           .HasReturnAbility(adventurer)))
            {
                if (GUILayout.Button("Use Homeward Passage"))
                {
                    DeverQuestSessionStore.TryExitSurvival(
                        "Return", out string message);
                    EditorUtility.DisplayDialog(
                        "Homeward Passage", message, "Close");
                }
            }
            using (new EditorGUI.DisabledScope(
                       !stage.survivalExitOffered))
            {
                if (GUILayout.Button("Take Guild Wagon"))
                {
                    DeverQuestSessionStore.TryExitSurvival(
                        "Wagon", out string message);
                    EditorUtility.DisplayDialog(
                        "Guild Wagon", message, "Close");
                }
            }
            EditorGUILayout.EndHorizontal();
            if (DeverQuestEncumbranceService.IsEncumbered(adventurer))
            {
                EditorGUILayout.HelpBox(
                    "Encumbered. Drop something before continuing. " +
                    "Coin may only be exchanged at the Guild Hall.",
                    MessageType.Warning);
                foreach (DeverQuestInventoryEntry item in
                         adventurer.inventory.ToArray())
                {
                    if (GUILayout.Button(
                            $"Drop 1 × {item.displayName} " +
                            $"({item.unitWeight:0.##} wt)"))
                    {
                        DeverQuestEncumbranceService.DropInventory(
                            item.ownershipId, 1, out _);
                        break;
                    }
                }
            }
        }

        private static void DrawBattleResults(
            DeverQuestSession session)
        {
            if (session.battleResults == null ||
                session.battleResults.Count == 0)
            {
                return;
            }
            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField(
                "Battle Chronicle",
                EditorStyles.boldLabel);
            foreach (DeverQuestBattleResult battle
                     in session.battleResults)
            {
                string battleStatus =
                    battle.safetyPaused
                        ? "Safety Pause"
                        : battle.victory
                            ? battle.earlyVictory
                                ? "Early Victory"
                                : "Victory"
                            : "Defeat";
                EditorGUILayout.LabelField(
                    $"{battleStatus} · " +
                    battle.encounterName,
                    $"{battle.rounds} rounds · HP " +
                    $"{battle.startingHitPoints}→" +
                    $"{battle.endingHitPoints}");
                EditorGUILayout.LabelField(
                    "Typed Damage",
                    string.IsNullOrWhiteSpace(
                        battle.typedDamageSummary)
                        ? "Legacy battle — no typed damage record."
                        : battle.typedDamageSummary,
                    EditorStyles.wordWrappedLabel);
                if (!string.IsNullOrWhiteSpace(
                        battle.companionName))
                {
                    EditorGUILayout.LabelField(
                        "Companion",
                        $"{battle.companionName} · HP " +
                        $"{battle.companionStartingHitPoints}→" +
                        $"{battle.companionEndingHitPoints} · " +
                        $"Level {battle.companionLevelBefore}→" +
                        $"{battle.companionLevelAfter}" +
                        (battle.companionFell
                            ? " · Fell"
                            : string.Empty));
                }
                if (battle.loot.Count > 0)
                {
                    EditorGUILayout.LabelField(
                        "Loot",
                        string.Join(", ", battle.loot));
                }
                if (!string.IsNullOrWhiteSpace(battle.injury))
                {
                    EditorGUILayout.LabelField(
                        "Consequence", battle.injury);
                }
                if (!string.IsNullOrWhiteSpace(
                        battle.safetyPauseReason))
                {
                    EditorGUILayout.HelpBox(
                        battle.safetyPauseReason,
                        MessageType.Warning);
                }
                if (battle.actionEvents.Count > 0)
                {
                    DeverQuestCombatActionEvent lastAction =
                        battle.actionEvents[
                            battle.actionEvents.Count - 1];
                    EditorGUILayout.LabelField(
                        "Last Tactical Action",
                        $"{lastAction.actor}: " +
                        $"{lastAction.actionName} → " +
                        $"{lastAction.target}");
                }
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
            EditorGUILayout.LabelField(
                $"{profile.campaignDifficulty} campaign · " +
                $"Recommended Level " +
                $"{profile.dailyDecreeRecommendedLevel} · " +
                $"Checks {(profile.dailyDecreeCheckModifier >= 0 ? "+" : string.Empty)}" +
                $"{profile.dailyDecreeCheckModifier}");

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
            DrawCompensationPreview(
                profile,
                days,
                startDate,
                endDate);
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
            DrawNewChronicleControl(profile);
            DrawHistoryExport(profile, days);

            if (!string.IsNullOrWhiteSpace(historyMessage))
            {
                EditorGUILayout.HelpBox(
                    historyMessage,
                    MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawCompensationPreview(
            DeverQuestProfile profile,
            IReadOnlyList<DeverQuestHistoryDay> filteredDays,
            DateTime? startDate,
            DateTime? endDate)
        {
            compensationPreviewFoldout = EditorGUILayout.Foldout(
                compensationPreviewFoldout,
                "Compensation Preview",
                true);
            if (!compensationPreviewFoldout)
            {
                return;
            }

            DeverQuestGuildAccount account =
                DeverQuestGuildAccountService.CurrentAccount;
            if (account == null)
            {
                return;
            }
            if (!account.compensationPreviewEnabled)
            {
                EditorGUILayout.HelpBox(
                    "No optional compensation preview policy is enabled for " +
                    "this Adventurer. A Boss or CEO can configure one in " +
                    "Guild Hall > Guild Accounts and Authority.",
                    MessageType.Info);
                return;
            }

            DeverQuestCompensationPreview preview =
                DeverQuestCompensationService.BuildPreview(
                    account,
                    profile,
                    filteredDays);
            DateTime today = DateTime.Today;
            int daysSinceMonday =
                ((int)today.DayOfWeek + 6) % 7;
            DateTime weekStart =
                today.AddDays(-daysSinceMonday);
            List<DeverQuestHistoryDay> currentWeekDays =
                DeverQuestHistoryService.GetFilteredDays(
                    weekStart,
                    today,
                    historyProjectFilter,
                    historyCategoryFilter);
            DeverQuestCompensationPreview weekPreview =
                DeverQuestCompensationService.BuildPreview(
                    account,
                    profile,
                    currentWeekDays);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawReadOnlyValue(
                "Policy",
                DeverQuestCompensationService.DescribeBasis(account));
            DrawReadOnlyValue(
                "Current Week Eligible",
                FormatDuration(weekPreview.EligibleSeconds));
            DrawReadOnlyValue(
                "Current Week Estimate",
                DeverQuestCompensationService.FormatMoney(
                    account,
                    weekPreview.EstimatedGross));
            DrawReadOnlyValue(
                "Filtered Eligible",
                FormatDuration(preview.EligibleSeconds));
            DrawReadOnlyValue(
                "Filtered Estimate",
                DeverQuestCompensationService.FormatMoney(
                    account,
                    preview.EstimatedGross));
            DrawReadOnlyValue(
                "Included",
                $"{preview.IncludedChronicles} Chronicle(s) · " +
                $"{preview.IncludedSessions} Quest(s)");
            DrawReadOnlyValue(
                "Eligible Time",
                account.compensationIncludeApprovedBreaks
                    ? "Focused + Approved Breaks"
                    : "Focused only");

            if (preview.ExcludedModifiedSeconds > 0d)
            {
                EditorGUILayout.HelpBox(
                    $"{FormatDuration(preview.ExcludedModifiedSeconds)} " +
                    "was excluded because its Chronicle was modified or " +
                    "could not be verified.",
                    MessageType.Warning);
            }
            if (preview.ExcludedLegacySeconds > 0d)
            {
                EditorGUILayout.HelpBox(
                    $"{FormatDuration(preview.ExcludedLegacySeconds)} of " +
                    "legacy/unsealed time was excluded by this policy.",
                    MessageType.Info);
            }
            if (preview.FlaggedSeconds > 0d)
            {
                EditorGUILayout.HelpBox(
                    $"{FormatDuration(preview.FlaggedSeconds)} is included " +
                    "but matches a configured long/frequent Quest flag and " +
                    "should be reviewed manually.",
                    MessageType.Warning);
            }
            EditorGUILayout.HelpBox(
                DeverQuestCompensationService.Disclaimer,
                MessageType.Warning);

            if (filteredDays.Count > 0 &&
                GUILayout.Button("Export Filtered Compensation Preview…"))
            {
                string path = EditorUtility.SaveFilePanel(
                    "Export Compensation Preview",
                    GetDeveloperFolder(profile),
                    $"DeverQuest_Compensation_Preview_" +
                    $"{DateTime.Now:yyyy-MM-dd}",
                    "csv");
                if (!string.IsNullOrWhiteSpace(path))
                {
                    string rangeDescription =
                        startDate.HasValue || endDate.HasValue
                            ? $"{startDate?.ToString("yyyy-MM-dd") ?? "All"} " +
                              $"through " +
                              $"{endDate?.ToString("yyyy-MM-dd") ?? "All"}"
                            : "All filtered Chronicle history";
                    historyMessage =
                        DeverQuestCompensationService.TryExportPreview(
                            path,
                            account,
                            preview,
                            rangeDescription,
                            out string error)
                            ? $"Compensation Preview exported:\n{path}"
                            : $"Compensation Preview export failed: {error}";
                }
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
                "Coin Earned",
                DeverQuestAdventurerService.FormatCoins(
                    summary.CopperEarned));

            DrawReadOnlyValue(
                "Coin Spent",
                DeverQuestAdventurerService.FormatCoins(
                    summary.CopperSpent));

            DrawReadOnlyValue(
                "Experience Earned",
                $"{summary.ExperienceEarned} XP");
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
                "Adventurer Treasury",
                EditorStyles.boldLabel);
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            EditorGUILayout.LabelField(
                "Coin Purse",
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.copperBalance));
            EditorGUILayout.LabelField(
                "Physical Coin",
                $"{adventurer.platinumCoins}p · " +
                $"{adventurer.goldCoins}g · " +
                $"{adventurer.silverCoins}s · " +
                $"{adventurer.copperCoins}c · " +
                $"{DeverQuestAdventurerService.CoinPieceCount(adventurer)} pieces");
            EditorGUILayout.LabelField(
                "Carry Weight",
                $"{DeverQuestEncumbranceService.CarriedWeight(adventurer):0.0} / " +
                $"{DeverQuestEncumbranceService.CarryCapacity(adventurer):0.0}" +
                (DeverQuestEncumbranceService.IsEncumbered(adventurer)
                    ? " · ENCUMBERED"
                    : string.Empty));
            EditorGUILayout.LabelField(
                "Lifetime Earned",
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.totalCopperEarned));
            EditorGUILayout.LabelField(
                "Lifetime Spent",
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.totalCopperSpent));
        }

        private void DrawHistoryDays(
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
                    $"Chronicle {Math.Max(1, day.Record.chronicleIndex)} · " +
                    $"Integrity: {day.IntegrityStatus}");
                EditorGUILayout.HelpBox(
                    day.IntegrityMessage,
                    day.IntegrityStatus ==
                    DeverQuestIntegrityStatus.Modified
                        ? MessageType.Warning
                        : MessageType.None);

                EditorGUILayout.LabelField(
                    $"{summary.SessionCount} session(s) · " +
                    $"{FormatDuration(summary.FocusedSeconds)} focused");
                int dailyLimit = DeverQuestSettingsStore.Profile
                    .suspiciousDailyQuestCount;
                if (day.SuspiciousSessionCount > 0 ||
                    day.SuspiciousFrequency)
                {
                    EditorGUILayout.HelpBox(
                        $"{day.SuspiciousSessionCount} unusually long " +
                        $"Quest(s); daily frequency threshold " +
                        $"{dailyLimit}. Flagged for leadership review, not " +
                        "automatically rejected.",
                        MessageType.Warning);
                }

                EditorGUILayout.BeginHorizontal();

                using (new EditorGUI.DisabledScope(
                           !File.Exists(day.MarkdownPath)))
                {
                    if (GUILayout.Button("Open Timecard"))
                    {
                        DeverQuestIdleMonitor
                            .BeginIntentionalExternalAction();
                        EditorUtility.OpenWithDefaultApp(
                            day.MarkdownPath);
                    }

                    if (GUILayout.Button("Reveal Timecard"))
                    {
                        DeverQuestIdleMonitor
                            .BeginIntentionalExternalAction();
                        EditorUtility.RevealInFinder(
                            day.MarkdownPath);
                    }
                }

                EditorGUILayout.EndHorizontal();

                foreach (DeverQuestSession session
                         in day.Record.sessions)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(
                        session.taskName,
                        $"{FormatDuration(session.accumulatedFocusedSeconds)} " +
                        "focused");
                    if (GUILayout.Button(
                            "Request Correction",
                            GUILayout.Width(132f)))
                    {
                        correctionDataPath = day.DataPath;
                        correctionSessionId = session.sessionId;
                        correctionSessionTitle = session.taskName;
                        correctionReason = string.Empty;
                        correctionValue = string.Empty;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                DrawCorrectionEditor(day);
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawCorrectionEditor(DeverQuestHistoryDay day)
        {
            if (correctionDataPath == day.DataPath &&
                !string.IsNullOrWhiteSpace(correctionSessionId))
            {
                EditorGUILayout.Space(5f);
                EditorGUILayout.LabelField(
                    $"Correction: {correctionSessionTitle}",
                    EditorStyles.boldLabel);
                correctionReason = EditorGUILayout.TextField(
                    "Reason", correctionReason);
                correctionValue = EditorGUILayout.TextArea(
                    correctionValue, GUILayout.MinHeight(48f));
                using (new EditorGUI.DisabledScope(
                           string.IsNullOrWhiteSpace(correctionReason) ||
                           string.IsNullOrWhiteSpace(correctionValue)))
                {
                    if (GUILayout.Button(
                            "Append Correction Request"))
                    {
                        DeverQuestChronicleIntegrityService.AddCorrection(
                            day.DataPath,
                            correctionSessionId,
                            correctionSessionTitle,
                            DeverQuestSettingsStore.Profile.developerName,
                            correctionReason,
                            correctionValue);
                        DeverQuestTimecardWriter.TryRegenerate(
                            day.DataPath, out string regenerateError);
                        historyMessage =
                            string.IsNullOrWhiteSpace(regenerateError)
                                ? "Correction request appended."
                                : regenerateError;
                        correctionSessionId = string.Empty;
                        DeverQuestHistoryService.Refresh(
                            DeverQuestSettingsStore.Profile);
                    }
                }
            }

            List<DeverQuestCorrection> corrections =
                DeverQuestChronicleIntegrityService.LoadCorrections(
                    day.DataPath);
            foreach (DeverQuestCorrection correction in corrections)
            {
                DeverQuestSession correctedSession =
                    day.Record.sessions.FirstOrDefault(
                        item => item.sessionId ==
                                correction.sessionId);
                string correctionProject =
                    correctedSession?.projectName ?? string.Empty;
                bool leadership =
                    DeverQuestGuildAccountService.HasPermission(
                        DeverQuestGuildPermission.ReviewCorrections,
                        correctionProject);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    $"{correction.sessionTitle} · {correction.status}",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    correction.reason,
                    EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField(
                    correction.correctedValue,
                    EditorStyles.wordWrappedLabel);
                if (leadership && correction.status == "Pending")
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Approve"))
                    {
                        ReviewCorrection(
                            day, correction, "Approved");
                    }
                    if (GUILayout.Button("Return"))
                    {
                        ReviewCorrection(
                            day, correction, "Returned");
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void ReviewCorrection(
            DeverQuestHistoryDay day,
            DeverQuestCorrection correction,
            string status)
        {
            DeverQuestSession correctedSession =
                day.Record.sessions.FirstOrDefault(
                    item => item.sessionId == correction.sessionId);
            DeverQuestChronicleIntegrityService.ReviewCorrection(
                day.DataPath,
                correction.correctionId,
                status,
                DeverQuestSettingsStore.Profile.developerName,
                correctedSession?.projectName ?? string.Empty);
            DeverQuestTimecardWriter.TryRegenerate(
                day.DataPath, out string error);
            historyMessage = string.IsNullOrWhiteSpace(error)
                ? $"Correction {status.ToLowerInvariant()}."
                : error;
            DeverQuestHistoryService.Refresh(
                DeverQuestSettingsStore.Profile);
        }

        private void DrawNewChronicleControl(DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Chronicle Rollover",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Start a fresh numbered Chronicle for today. Existing " +
                "Chronicles are never overwritten.",
                MessageType.Info);
            if (GUILayout.Button("Start New Chronicle"))
            {
                string developerFolder =
                    DeverQuestPathUtility.GetDeveloperFolder(
                        profile.timecardRootPath,
                        profile.developerName);
                int index =
                    DeverQuestChronicleIntegrityService
                        .StartNewChronicle(
                            developerFolder,
                            DateTime.Now.ToString("yyyy-MM-dd"));
                historyMessage =
                    $"Chronicle {index} will receive the next completed Quest.";
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

        private static void DrawExternalActivitySetup()
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "External Activity",
                EditorStyles.boldLabel);

            DeverQuestExternalActivityProfile profile =
                (DeverQuestExternalActivityProfile)
                EditorGUILayout.ObjectField(
                    new GUIContent(
                        "Activity Profile",
                        "Foreground creative applications that may keep the " +
                        "Quest active while recent input is detected."),
                    DeverQuestExternalActivityMonitor.Profile,
                    typeof(DeverQuestExternalActivityProfile),
                    false);
            if (profile !=
                DeverQuestExternalActivityMonitor.Profile)
            {
                DeverQuestExternalActivityMonitor.SetProfile(
                    profile);
            }

            EditorGUILayout.HelpBox(
                "External activity never counts a permanently idle app as " +
                "work. The configured tool must be foreground and must have " +
                "recent keyboard or pointer input. Windows foreground " +
                "detection is supported in this milestone.",
                MessageType.Info);

            if (DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild) &&
                GUILayout.Button(
                    "Create Aseprite Activity Profile…"))
            {
                CreateAsepriteActivityProfile();
            }
        }

        private static void CreateAsepriteActivityProfile()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create Aseprite Activity Profile",
                "AsepriteActivityProfile",
                "asset",
                "Choose where to save the external activity profile.");
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            DeverQuestExternalActivityProfile profile =
                CreateInstance<DeverQuestExternalActivityProfile>();
            profile.displayName = "Pixel Art Workshop";
            profile.description =
                "Aseprite foreground input is accepted as deliberate " +
                "external Quest activity.";
            profile.providers.Add(
                new DeverQuestExternalActivityProvider
                {
                    displayName = "Aseprite",
                    processName = "aseprite",
                    inputFreshnessSeconds = 30,
                    enabled = true
                });
            AssetDatabase.CreateAsset(profile, path);
            AssetDatabase.SaveAssets();
            DeverQuestExternalActivityMonitor.SetProfile(
                profile);
            Selection.activeObject = profile;
            EditorGUIUtility.PingObject(profile);
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

        private static void DrawChronicleIntegritySetup(
            DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Chronicle Integrity and Review",
                EditorStyles.boldLabel);
            profile.chronicleIntegrityEnabled =
                EditorGUILayout.Toggle(
                    new GUIContent(
                        "Integrity Seals",
                        "Creates a SHA-256 audit chain. This reveals casual " +
                        "edits; it is not an authoritative server signature."),
                    profile.chronicleIntegrityEnabled);
            profile.chronicleMaxSessions =
                EditorGUILayout.IntField(
                    "Quests per Chronicle",
                    profile.chronicleMaxSessions);
            profile.chronicleMaxKilobytes =
                EditorGUILayout.IntField(
                    "Chronicle Size (KB)",
                    profile.chronicleMaxKilobytes);
            profile.suspiciousQuestMinutes =
                EditorGUILayout.IntField(
                    "Flag Quest at (min)",
                    profile.suspiciousQuestMinutes);
            profile.suspiciousDailyQuestCount =
                EditorGUILayout.IntField(
                    "Flag Daily Quest Count",
                    profile.suspiciousDailyQuestCount);
            EditorGUILayout.HelpBox(
                "Integrity hashes reveal changes but cannot stop a person " +
                "with local file access from replacing files. Authoritative " +
                "records require a future shared Guild service.",
                MessageType.Info);
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

                DrawAudioProfiles();
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

            if (!DeverQuestEditorAudioBridge.IndependentVolumeSupported)
            {
                EditorGUILayout.HelpBox(
                    "Music and Ambience now use independent logical " +
                    "playback channels. This Unity editor exposes only " +
                    "global preview gain, so both channels can play and " +
                    "stop independently even when their volume sliders " +
                    "cannot be mixed independently.",
                    MessageType.Info);
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

            DrawAudioProfiles();
            EditorGUILayout.EndVertical();
        }

        private void DrawAudioProfiles()
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField(
                "Warnings and Ambience",
                EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            DeverQuestWarningAudioProfile warning =
                (DeverQuestWarningAudioProfile)
                EditorGUILayout.ObjectField(
                    "Warning Profile",
                    DeverQuestAudioDirector.WarningProfile,
                    typeof(DeverQuestWarningAudioProfile),
                    false);
            if (EditorGUI.EndChangeCheck())
            {
                DeverQuestAudioDirector.SetWarningProfile(
                    warning);
            }

            EditorGUI.BeginChangeCheck();
            DeverQuestAmbienceProfile ambience =
                (DeverQuestAmbienceProfile)
                EditorGUILayout.ObjectField(
                    "Ambience Profile",
                    DeverQuestAudioDirector.AmbienceProfile,
                    typeof(DeverQuestAmbienceProfile),
                    false);
            if (EditorGUI.EndChangeCheck())
            {
                DeverQuestAudioDirector.SetAmbienceProfile(
                    ambience);
                ambience =
                    DeverQuestAudioDirector.AmbienceProfile;
                GUI.FocusControl(null);
            }

            if (DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Warning Profile…"))
                {
                    CreateCharacterAsset<
                        DeverQuestWarningAudioProfile>(
                        "NewWarningAudioProfile");
                }
                if (GUILayout.Button("Create Ambience Profile…"))
                {
                    DeverQuestAmbienceProfile created =
                        CreateCharacterAsset<
                            DeverQuestAmbienceProfile>(
                            "NewAmbienceProfile");
                    if (created != null)
                    {
                        DeverQuestAudioDirector.SetAmbienceProfile(
                            created);
                        ambience = created;
                    }
                }
                EditorGUILayout.EndHorizontal();

                DeverQuestAmbienceProfile selectedAmbience =
                    Selection.activeObject as
                        DeverQuestAmbienceProfile;
                using (new EditorGUI.DisabledScope(
                           selectedAmbience == null ||
                           selectedAmbience == ambience))
                {
                    if (GUILayout.Button(
                            "Use Selected Ambience Profile"))
                    {
                        DeverQuestAudioDirector.SetAmbienceProfile(
                            selectedAmbience);
                        ambience = selectedAmbience;
                        Repaint();
                    }
                }
            }

            if (warning != null)
            {
                EditorGUI.BeginChangeCheck();
                warning.volume = EditorGUILayout.Slider(
                    "Warning Volume",
                    warning.volume,
                    0f,
                    1f);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(warning);
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Test Warning"))
                {
                    DeverQuestAudioDirector.PlayCue(
                        DeverQuestAudioCue.IdleWarning);
                }
                if (GUILayout.Button("Test Victory"))
                {
                    DeverQuestAudioDirector.PlayCue(
                        DeverQuestAudioCue.EncounterVictory);
                }
                if (GUILayout.Button("Test Level Up"))
                {
                    DeverQuestAudioDirector.PlayCue(
                        DeverQuestAudioCue.LevelUp);
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button(
                        "Select Warning Profile in Project"))
                {
                    Selection.activeObject = warning;
                    EditorGUIUtility.PingObject(warning);
                }
            }

            if (ambience != null)
            {
                int ambienceClipCount =
                    ambience.ambienceClips == null
                        ? 0
                        : ambience.ambienceClips.Count(
                            clip => clip != null);
                if (ambienceClipCount == 0)
                {
                    EditorGUILayout.HelpBox(
                        "The selected Ambience Profile is assigned, but it " +
                        "does not contain a playable AudioClip. Select the " +
                        "profile in the Project window and add at least one " +
                        "clip to Ambience Clips.",
                        MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.LabelField(
                        "Playable Ambience Clips",
                        ambienceClipCount.ToString());
                }

                EditorGUI.BeginChangeCheck();
                ambience.volume = EditorGUILayout.Slider(
                    "Ambience Volume",
                    ambience.volume,
                    0f,
                    1f);
                ambience.playDuringActiveQuest =
                    EditorGUILayout.Toggle(
                        "Quest-Aware Ambience",
                        ambience.playDuringActiveQuest);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(ambience);
                    DeverQuestAudioDirector.ApplyVolumes();
                }
                EditorGUILayout.LabelField(
                    "Now Playing",
                    DeverQuestAudioDirector.CurrentAmbience == null
                        ? "No ambience playing"
                        : DeverQuestAudioDirector
                            .CurrentAmbience.name);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(
                        DeverQuestAudioDirector.AmbiencePlaying
                            ? "Stop Ambience"
                            : "Play Ambience"))
                {
                    if (DeverQuestAudioDirector.AmbiencePlaying)
                    {
                        DeverQuestAudioDirector.StopAmbience();
                    }
                    else
                    {
                        DeverQuestAudioDirector.PlayAmbience();
                    }
                }
                if (GUILayout.Button("Next Ambience"))
                {
                    DeverQuestAudioDirector.NextAmbience();
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button(
                        "Select Ambience Profile in Project"))
                {
                    Selection.activeObject = ambience;
                    EditorGUIUtility.PingObject(ambience);
                }
            }
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
                        "Daily Decree (min)",
                        profile.dailyWorkGoalMinutes);

                profile.copperPerWorkBlock =
                    EditorGUILayout.IntField(
                        "Copper per Block",
                        profile.copperPerWorkBlock);

                profile.experiencePerWorkBlock =
                    EditorGUILayout.IntField(
                        "XP per Block",
                        profile.experiencePerWorkBlock);

                profile.dailyCopperBonus =
                    EditorGUILayout.IntField(
                        "Decree Copper",
                        profile.dailyCopperBonus);

                profile.dailyExperienceBonus =
                    EditorGUILayout.IntField(
                        "Decree XP",
                        profile.dailyExperienceBonus);

                profile.baseQuestCopper =
                    EditorGUILayout.IntField(
                        "Base Quest Copper",
                        profile.baseQuestCopper);

                profile.baseQuestExperience =
                    EditorGUILayout.IntField(
                        "Base Quest XP",
                        profile.baseQuestExperience);
            }

            profile.Sanitize();

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField(
                "Adventurer Identity",
                EditorStyles.boldLabel);

            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            EditorGUILayout.LabelField(
                "Character Name", adventurer.characterName);
            EditorGUILayout.LabelField("Guild", adventurer.guildName);
            EditorGUILayout.LabelField(
                "Class", adventurer.characterClass);
            EditorGUILayout.LabelField(
                "Ancestry",
                string.IsNullOrWhiteSpace(adventurer.ancestryName)
                    ? "Legacy / Not Assigned"
                    : adventurer.ancestryName);
            EditorGUILayout.LabelField(
                "Alignment",
                ObjectNames.NicifyVariableName(
                    adventurer.alignment.ToString()));
            EditorGUILayout.LabelField(
                "Faith",
                string.IsNullOrWhiteSpace(adventurer.deityName)
                    ? "Agnostic"
                    : adventurer.deityName);
            EditorGUILayout.LabelField(
                "Guild Rank", adventurer.guildRank);
            EditorGUILayout.HelpBox(
                "Identity, Ancestry, Class, Alignment, Faith, and Guild " +
                "Rank are locked to the active Guild account.",
                MessageType.Info);
        }

        private void DrawRewardsPanel(DeverQuestProfile profile)
        {
            if (!profile.rewardsEnabled)
            {
                return;
            }

            EditorGUILayout.LabelField(
                "Adventurer Spoils",
                EditorStyles.boldLabel);

            double todayMinutes =
                DeverQuestRewardService.GetTodayFocusedMinutes();

            EditorGUILayout.LabelField(
                $"Daily Decree: {todayMinutes:0.#} / " +
                $"{profile.dailyWorkGoalMinutes} focused minutes");

            double carryMinutes =
                DeverQuestRewardService.Wallet
                    .unrewardedWorkSeconds / 60d;

            EditorGUILayout.LabelField(
                $"Next work block: {carryMinutes:0.#} / " +
                $"{profile.rewardWorkBlockMinutes} minutes");

            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            EditorGUILayout.LabelField(
                "Coin Purse",
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.copperBalance),
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Lifetime Spoils",
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.totalCopperEarned));

            if (!string.IsNullOrWhiteSpace(rewardMessage))
            {
                EditorGUILayout.HelpBox(
                    rewardMessage,
                    MessageType.Info);
            }

            EditorGUILayout.HelpBox(
                "Spend earned coin through the Guild Shop. Purchases and " +
                "redemptions remain recorded in the Guild ledger.",
                MessageType.Info);
        }

        private static void DrawAdventurerSheet()
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Adventurer Character Sheet",
                EditorStyles.boldLabel);
            string characterName = string.IsNullOrWhiteSpace(
                adventurer.characterName)
                ? "Unnamed Adventurer"
                : adventurer.characterName;
            EditorGUILayout.LabelField(
                characterName,
                $"{adventurer.characterClass} · Level {adventurer.level}");
            EditorGUILayout.LabelField(
                "Ancestry",
                string.IsNullOrWhiteSpace(adventurer.ancestryName)
                    ? "Legacy / Not Assigned"
                    : adventurer.ancestryName);
            EditorGUILayout.LabelField(
                "Alignment",
                ObjectNames.NicifyVariableName(
                    adventurer.alignment.ToString()));
            EditorGUILayout.LabelField(
                "Faith",
                string.IsNullOrWhiteSpace(adventurer.deityName)
                    ? "Agnostic"
                    : adventurer.deityName);
            EditorGUILayout.LabelField(
                adventurer.guildName,
                adventurer.guildRank);
            EditorGUILayout.LabelField(
                "Experience",
                $"{adventurer.currentExperience} / " +
                $"{DeverQuestAdventurerService.ExperienceForNextLevel(adventurer.level)} XP");
            Rect experienceRect =
                EditorGUILayout.GetControlRect(false, 18f);
            float experienceProgress =
                adventurer.currentExperience /
                (float)DeverQuestAdventurerService.ExperienceForNextLevel(
                    adventurer.level);
            EditorGUI.ProgressBar(
                experienceRect,
                Mathf.Clamp01(experienceProgress),
                $"Lifetime XP: {adventurer.lifetimeExperience}");
            EditorGUILayout.LabelField(
                "Coin Purse",
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.copperBalance));
            EditorGUILayout.LabelField(
                "Hit Points",
                $"{adventurer.currentHitPoints} / " +
                $"{adventurer.maximumHitPoints} · d{adventurer.hitDie}");
            EditorGUILayout.LabelField(
                "Armor Class",
                DeverQuestRulesService.ArmorClass(adventurer).ToString());
            EditorGUILayout.LabelField(
                "Damage Affinities",
                DeverQuestDamageService
                    .DescribeAdventurerAffinities(adventurer),
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField(
                "Proficiency",
                $"+{DeverQuestRulesService.ProficiencyBonus(adventurer.level)}");
            EditorGUILayout.Space(4f);
            DrawAbility(
                "STR", adventurer,
                DeverQuestAbility.Strength);
            DrawAbility(
                "DEX", adventurer,
                DeverQuestAbility.Dexterity);
            DrawAbility(
                "CON", adventurer,
                DeverQuestAbility.Constitution);
            DrawAbility(
                "INT", adventurer,
                DeverQuestAbility.Intelligence);
            DrawAbility(
                "WIS", adventurer,
                DeverQuestAbility.Wisdom);
            DrawAbility(
                "CHA", adventurer,
                DeverQuestAbility.Charisma);
            EditorGUILayout.LabelField(
                "AGI", adventurer.agility.ToString());
            EditorGUILayout.LabelField(
                "STA", adventurer.stamina.ToString());
            EditorGUILayout.LabelField(
                "Luck", adventurer.luck.ToString());
            EditorGUILayout.LabelField(
                "Mana",
                $"{adventurer.currentMana} / " +
                $"{adventurer.maximumMana}");
            EditorGUILayout.LabelField(
                "Home Department",
                adventurer.homeDepartment);
            EditorGUILayout.LabelField(
                "Wellness",
                $"Hunger {adventurer.hunger} · " +
                $"Rest {adventurer.rest} · " +
                $"Happiness {adventurer.happiness}");
            EditorGUILayout.LabelField(
                "Saving Throw Proficiencies",
                string.Join(", ", adventurer.proficientSaves));
            EditorGUILayout.LabelField(
                "Class Features",
                string.Join(", ",
                    DeverQuestRulesService.ClassFeatures(adventurer)));
            DeverQuestClassDefinition classDefinition =
                DeverQuestIdentityCatalogService.FindClass(
                    adventurer.classId,
                    adventurer.characterClass);
            DeverQuestAncestry ancestry =
                DeverQuestIdentityCatalogService.FindAncestry(
                    adventurer.ancestryId,
                    adventurer.ancestryName);
            DeverQuestDeity faith =
                DeverQuestIdentityCatalogService.FindFaith(
                    adventurer.deityId,
                    adventurer.deityName);
            if (ancestry != null)
            {
                EditorGUILayout.LabelField(
                    "Ancestry Traits",
                    ancestry.innateTraits.Count == 0
                        ? "None"
                        : string.Join(", ", ancestry.innateTraits));
                EditorGUILayout.LabelField(
                    "Languages",
                    ancestry.languages.Count == 0
                        ? "None"
                        : string.Join(", ", ancestry.languages));
            }
            if (faith != null &&
                !string.IsNullOrWhiteSpace(faith.grantedTrait))
            {
                EditorGUILayout.LabelField(
                    "Faith Boon", faith.grantedTrait);
            }
            if (classDefinition != null &&
                classDefinition.supportsCompanion)
            {
                EditorGUILayout.LabelField(
                    "Companion Tradition",
                    string.IsNullOrWhiteSpace(
                        classDefinition.companionTradition)
                        ? "Supported"
                        : classDefinition.companionTradition);
            }
            if (classDefinition?.abilityProfile != null)
            {
                EditorGUILayout.LabelField(
                    "Tactical Ability Profile",
                    $"{classDefinition.abilityProfile.displayName} · " +
                    $"{classDefinition.abilityProfile.tacticalStyle} · " +
                    $"{classDefinition.abilityProfile.abilities.Count} actions");
            }
            EditorGUILayout.LabelField(
                "Status Effects",
                adventurer.statusEffects.Count == 0
                    ? "None"
                    : string.Join(", ", adventurer.statusEffects));
            if (adventurer.isFallen)
            {
                EditorGUILayout.HelpBox(
                    "This Adventurer has fallen. Resurrection costs 50 " +
                    "copper and restores half Hit Points.",
                    MessageType.Warning);
                if (GUILayout.Button("Resurrect at the Guild Shrine"))
                {
                    DeverQuestEncounterService.Resurrect(
                        out string resurrectionMessage);
                    EditorUtility.DisplayDialog(
                        "Guild Shrine",
                        resurrectionMessage,
                        "Close");
                }
            }
            List<string> equipment =
                DeverQuestRulesService.EquippedNames(adventurer);
            List<string> spells =
                DeverQuestRulesService.KnownSpellNames(adventurer);
            EditorGUILayout.LabelField(
                "Equipment",
                equipment.Count == 0
                    ? "None"
                    : string.Join(", ", equipment));
            EditorGUILayout.LabelField(
                "Known Spells",
                spells.Count == 0
                    ? "None"
                    : string.Join(", ", spells));
            foreach (DeverQuestSpell spell in
                     DeverQuestRulesService.KnownSpellAssets(adventurer))
            {
                string effects =
                    spell.effects == null ||
                    spell.effects.Count == 0
                        ? string.IsNullOrWhiteSpace(spell.damageDice)
                            ? "Legacy utility"
                            : "Legacy direct damage"
                        : string.Join(
                            ", ",
                            spell.effects
                                .Where(effect => effect != null)
                                .Select(effect =>
                                    effect.effectType.ToString()));
                EditorGUILayout.LabelField(
                    "  " + spell.displayName,
                    $"{effects} · {spell.manaCost} mana");
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCompanionStable()
        {
            companionStableFoldout = EditorGUILayout.Foldout(
                companionStableFoldout,
                "Companion Stable",
                true);
            if (!companionStableFoldout)
            {
                return;
            }
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Pets, Familiars, Minions, and Companions",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "One active Companion may join deterministic encounters. " +
                "Companions affect RPG battles only—they never create " +
                "focused-work time or productivity rewards.",
                EditorStyles.wordWrappedLabel);

            selectedCompanionProfile =
                (DeverQuestCompanionProfile)
                EditorGUILayout.ObjectField(
                    "Companion Profile",
                    selectedCompanionProfile,
                    typeof(DeverQuestCompanionProfile),
                    false);
            bool canRecruit =
                DeverQuestCompanionService.CanRecruit(
                    adventurer,
                    selectedCompanionProfile,
                    out string recruitReason);
            using (new EditorGUI.DisabledScope(!canRecruit))
            {
                if (GUILayout.Button("Recruit Companion"))
                {
                    DeverQuestCompanionService.Recruit(
                        selectedCompanionProfile,
                        out companionMessage);
                }
            }
            if (!canRecruit &&
                selectedCompanionProfile != null &&
                !string.IsNullOrWhiteSpace(recruitReason))
            {
                EditorGUILayout.HelpBox(
                    recruitReason,
                    MessageType.Info);
            }

            bool canManage =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);
            using (new EditorGUI.DisabledScope(!canManage))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(
                        "Generate Original Starter Stable"))
                {
                    DeverQuestCompanionGenerationReport report =
                        DeverQuestCompanionCatalogGenerator
                            .GenerateOriginalStarterCatalog();
                    companionMessage = report.Summary;
                }
                if (GUILayout.Button("Create Profile…"))
                {
                    CreateCharacterAsset<
                        DeverQuestCompanionProfile>(
                        "NewCompanionProfile");
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(5f);
            if ((adventurer.companions?.Count ?? 0) == 0)
            {
                EditorGUILayout.LabelField(
                    "No Companions have joined this Adventurer.");
            }
            else
            {
                foreach (DeverQuestCompanionState companion in
                         adventurer.companions.ToList())
                {
                    if (companion == null)
                    {
                        continue;
                    }
                    DeverQuestCompanionProfile profile =
                        DeverQuestCompanionService.FindProfile(
                            companion.profileId);
                    EditorGUILayout.BeginVertical(
                        EditorStyles.helpBox);
                    EditorGUI.BeginChangeCheck();
                    companion.customName =
                        EditorGUILayout.TextField(
                            "Name",
                            companion.customName);
                    if (EditorGUI.EndChangeCheck())
                    {
                        companion.customName =
                            companion.customName?.Trim() ??
                            string.Empty;
                        DeverQuestAdventurerService.Save();
                    }
                    int maximumHitPoints =
                        DeverQuestCompanionService
                            .MaximumHitPoints(
                                companion,
                                profile);
                    EditorGUILayout.LabelField(
                        profile == null
                            ? "Missing Companion Profile"
                            : $"{profile.kind} · {profile.role} · " +
                              $"{profile.creatureType}",
                        companion.isActive
                            ? "ACTIVE"
                            : companion.isFallen
                                ? "FALLEN"
                                : "Resting");
                    EditorGUILayout.LabelField(
                        "Level and Bond",
                        $"Level {companion.level} · " +
                        $"{companion.currentExperience}/" +
                        $"{DeverQuestCompanionService.ExperienceForNextLevel(companion.level)} XP · " +
                        $"Loyalty {companion.loyalty}/100");
                    EditorGUILayout.LabelField(
                        "Vitals",
                        $"HP {companion.currentHitPoints}/" +
                        $"{maximumHitPoints} · " +
                        $"Battles {companion.battles} · " +
                        $"Victories {companion.victories}");
                    EditorGUILayout.BeginHorizontal();
                    if (companion.isActive)
                    {
                        if (GUILayout.Button("Dismiss"))
                        {
                            DeverQuestCompanionService.Dismiss(
                                companion);
                            companionMessage =
                                $"{DeverQuestCompanionService.DisplayName(companion)} is resting.";
                        }
                    }
                    else
                    {
                        using (new EditorGUI.DisabledScope(
                                   companion.isFallen ||
                                   profile == null))
                        {
                            if (GUILayout.Button("Set Active"))
                            {
                                DeverQuestCompanionService.Activate(
                                    companion,
                                    out companionMessage);
                            }
                        }
                    }
                    using (new EditorGUI.DisabledScope(
                               profile == null ||
                               (!companion.isFallen &&
                                companion.currentHitPoints >=
                                maximumHitPoints)))
                    {
                        if (GUILayout.Button("Recover"))
                        {
                            DeverQuestCompanionService.Recover(
                                companion,
                                out companionMessage);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
            }
            if (!string.IsNullOrWhiteSpace(companionMessage))
            {
                EditorGUILayout.HelpBox(
                    companionMessage,
                    MessageType.Info);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawGuildShop()
        {
            guildShopFoldout = EditorGUILayout.Foldout(
                guildShopFoldout,
                "Guild Shop and Inventory",
                true);
            if (!guildShopFoldout)
            {
                return;
            }

            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Guild Quartermaster",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Coin Purse",
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.copperBalance));
            EditorGUILayout.LabelField(
                "Carry Weight",
                $"{DeverQuestEncumbranceService.CarriedWeight(adventurer):0.0} / " +
                $"{DeverQuestEncumbranceService.CarryCapacity(adventurer):0.0}");
            if (GUILayout.Button(
                    "Exchange Coin Denominations at Guild Hall"))
            {
                DeverQuestAdventurerService.ExchangeCoinAtGuildHall();
                shopMessage =
                    "Loose coin exchanged at 100c = 1s, " +
                    "100s = 1g, and 100g = 1p.";
            }
            selectedShopProfile =
                (DeverQuestShopProfile)EditorGUILayout.ObjectField(
                    "Shop Profile",
                    selectedShopProfile,
                    typeof(DeverQuestShopProfile),
                    false);

            if (DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Shop Profile…"))
                {
                    CreateCharacterAsset<DeverQuestShopProfile>(
                        "NewGuildShop");
                }
                if (GUILayout.Button("Create Shop Item…"))
                {
                    CreateCharacterAsset<DeverQuestShopItem>(
                        "NewGuildShopItem");
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button(
                        "Generate Starter Quartermaster"))
                {
                    selectedShopProfile =
                        DeverQuestStarterContentGenerator
                            .GenerateBasicShop();
                    shopMessage =
                        "Starter provisions and break permits generated " +
                        "under Assets/DeverQuest/GuildShop.";
                }
            }

            if (selectedShopProfile == null)
            {
                EditorGUILayout.HelpBox(
                    "Select a Shop Profile to browse the " +
                    "Quartermaster's stock.",
                    MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    selectedShopProfile.welcomeMessage,
                    MessageType.None);
                foreach (DeverQuestShopItem item
                         in selectedShopProfile.items)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    EditorGUILayout.BeginVertical(
                        EditorStyles.helpBox);
                    EditorGUILayout.LabelField(
                        item.displayName,
                        $"{item.rarity} {item.itemType} · " +
                        DeverQuestAdventurerService.FormatCoins(
                            item.copperCost));
                    if (!string.IsNullOrWhiteSpace(
                            item.description))
                    {
                        EditorGUILayout.LabelField(
                            item.description,
                            EditorStyles.wordWrappedLabel);
                    }
                    string approval =
                        item.requiresLeadershipApproval
                            ? " · Leadership approval required"
                            : string.Empty;
                    EditorGUILayout.LabelField(
                        $"Level {item.minimumLevel}{approval}",
                        EditorStyles.miniLabel);
                    using (new EditorGUI.DisabledScope(
                               adventurer.level <
                               item.minimumLevel ||
                               adventurer.copperBalance <
                               item.copperCost))
                    {
                        if (GUILayout.Button("Purchase"))
                        {
                            DeverQuestShopService.Purchase(
                                item, out shopMessage);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField(
                "Inventory",
                EditorStyles.boldLabel);
            if (adventurer.inventory.Count == 0)
            {
                EditorGUILayout.LabelField("The pack is empty.");
            }
            foreach (DeverQuestInventoryEntry entry
                     in adventurer.inventory.ToArray())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(
                    $"{entry.displayName} ×{entry.quantity}",
                    $"{entry.rarity} · {entry.binding} · " +
                    $"{entry.unitWeight * entry.quantity:0.##} wt");
                DeverQuestShopItem item =
                    DeverQuestShopService.FindItem(
                        entry.shopItemId);
                bool usable =
                    item != null &&
                    (item.itemType ==
                     DeverQuestShopItemType.Consumable ||
                     item.itemType == DeverQuestShopItemType.Food ||
                     item.itemType == DeverQuestShopItemType.Drink ||
                     item.itemType ==
                     DeverQuestShopItemType.InnRest ||
                     item.itemType ==
                     DeverQuestShopItemType.BreakPermit ||
                     item.itemType ==
                     DeverQuestShopItemType.BreakPermit);
                using (new EditorGUI.DisabledScope(!usable))
                {
                    if (GUILayout.Button(
                            "Use",
                            GUILayout.Width(60f)))
                    {
                        DeverQuestShopService.Use(
                            item, out shopMessage);
                    }
                }
                if (GUILayout.Button(
                        "Drop",
                        GUILayout.Width(60f)))
                {
                    DeverQuestEncumbranceService.DropInventory(
                        entry.ownershipId, 1, out shopMessage);
                }
                EditorGUILayout.EndHorizontal();
            }

            DrawTradingPost(adventurer);
            DrawPurchaseApprovals();
            if (!string.IsNullOrWhiteSpace(shopMessage))
            {
                EditorGUILayout.HelpBox(
                    shopMessage,
                    MessageType.Info);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private void DrawTradingPost(DeverQuestAdventurer adventurer)
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(
                "Trading Post", EditorStyles.boldLabel);
            List<DeverQuestGuildAccount> targets =
                DeverQuestGuildAccountService.Accounts
                    .Where(account =>
                        !account.disabled &&
                        account.accountId !=
                        DeverQuestGuildAccountService
                            .CurrentAccount?.accountId)
                    .ToList();
            if (targets.Count > 0)
            {
                tradeTargetIndex = Mathf.Clamp(
                    tradeTargetIndex, 0, targets.Count - 1);
                tradeTargetIndex = EditorGUILayout.Popup(
                    "Trade With",
                    tradeTargetIndex,
                    targets.Select(account =>
                        $"{account.characterName} " +
                        $"({account.developerName})").ToArray());
                foreach (DeverQuestInventoryEntry entry
                         in adventurer.inventory.ToArray())
                {
                    bool tradable = entry.tradable &&
                        entry.itemType !=
                        DeverQuestShopItemType.Redemption &&
                        entry.binding !=
                        DeverQuestItemBinding.BindOnPickup &&
                        entry.binding !=
                        DeverQuestItemBinding.AccountBound &&
                        string.IsNullOrWhiteSpace(
                            entry.boundAccountId);
                    using (new EditorGUI.DisabledScope(!tradable))
                    {
                        if (GUILayout.Button(
                                $"Offer {entry.displayName}"))
                        {
                            DeverQuestTradeService.Offer(
                                entry,
                                targets[tradeTargetIndex].accountId,
                                out shopMessage);
                        }
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField(
                    "Create another enabled Guild account to trade.");
            }

            string accountId =
                DeverQuestGuildAccountService.CurrentAccount
                    ?.accountId ?? string.Empty;
            foreach (DeverQuestTradeRecord trade
                     in DeverQuestTradeService.Records.Where(
                         value =>
                             value.status ==
                             DeverQuestTradeStatus.Offered &&
                             value.toAccountId == accountId).ToArray())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(
                    $"{trade.fromName} offers {trade.itemName}",
                    trade.rarity.ToString());
                if (GUILayout.Button("Accept", GUILayout.Width(65f)))
                {
                    DeverQuestTradeService.Accept(
                        trade, out shopMessage);
                }
                if (GUILayout.Button("Reject", GUILayout.Width(60f)))
                {
                    DeverQuestTradeService.Reject(
                        trade, out shopMessage);
                }
                EditorGUILayout.EndHorizontal();
            }
            foreach (DeverQuestTradeRecord trade
                     in DeverQuestTradeService.Records.Where(
                         value =>
                             value.fromAccountId == accountId &&
                             (value.status ==
                              DeverQuestTradeStatus.Offered ||
                              value.status ==
                              DeverQuestTradeStatus.Rejected)).ToArray())
            {
                if (GUILayout.Button(
                        $"{(trade.status == DeverQuestTradeStatus.Offered ? "Cancel" : "Reclaim")} " +
                        $"{trade.itemName} ({trade.toName})"))
                {
                    DeverQuestTradeService.CancelOrReclaim(
                        trade, out shopMessage);
                }
            }
            tradeLedgerFoldout = EditorGUILayout.Foldout(
                tradeLedgerFoldout, "Permanent Trade Ledger", true);
            if (tradeLedgerFoldout)
            {
                foreach (DeverQuestTradeRecord trade
                         in DeverQuestTradeService.Records.Take(20))
                {
                    EditorGUILayout.LabelField(
                        $"[{trade.status}] {trade.itemName}",
                        $"{trade.fromName} → {trade.toName}");
                }
            }
        }

        private void DrawPurchaseApprovals()
        {
            bool canApprove =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);
            if (canApprove)
            {
                List<DeverQuestPurchaseRecord> pending =
                    DeverQuestShopService.Records
                        .Where(record =>
                            record.status ==
                            DeverQuestPurchaseStatus.Requested)
                        .ToList();
                if (pending.Count > 0)
                {
                    EditorGUILayout.Space(6f);
                    EditorGUILayout.LabelField(
                        "Leadership Approval Queue",
                        EditorStyles.boldLabel);
                    foreach (DeverQuestPurchaseRecord record
                             in pending)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(
                            $"{record.adventurerName}: " +
                            $"{record.itemName}",
                            DeverQuestAdventurerService.FormatCoins(
                                record.copperCost));
                        if (GUILayout.Button(
                                "Approve",
                                GUILayout.Width(70f)))
                        {
                            DeverQuestShopService.Resolve(
                                record, true, out shopMessage);
                        }
                        if (GUILayout.Button(
                                "Deny",
                                GUILayout.Width(50f)))
                        {
                            DeverQuestShopService.Resolve(
                                record, false, out shopMessage);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                List<DeverQuestPurchaseRecord> fulfillment =
                    DeverQuestShopService.Records.Where(record =>
                        record.itemType ==
                        DeverQuestShopItemType.Redemption &&
                        record.status ==
                        DeverQuestPurchaseStatus.Approved).ToList();
                if (fulfillment.Count > 0)
                {
                    EditorGUILayout.LabelField(
                        "Real Reward Fulfillment",
                        EditorStyles.boldLabel);
                    fulfillmentReference =
                        EditorGUILayout.TextField(
                            "Delivery Reference",
                            fulfillmentReference);
                    foreach (DeverQuestPurchaseRecord record
                             in fulfillment)
                    {
                        if (GUILayout.Button(
                                $"Mark Delivered: " +
                                $"{record.itemName} → " +
                                $"{record.adventurerName}"))
                        {
                            DeverQuestShopService.MarkFulfilled(
                                record,
                                fulfillmentReference,
                                out shopMessage);
                        }
                    }
                }
            }

            purchaseHistoryFoldout = EditorGUILayout.Foldout(
                purchaseHistoryFoldout,
                "Purchase and Redemption History",
                true);
            if (!purchaseHistoryFoldout)
            {
                return;
            }
            string accountId =
                DeverQuestGuildAccountService.CurrentAccount
                    ?.accountId ?? string.Empty;
            IEnumerable<DeverQuestPurchaseRecord> visible =
                canApprove
                    ? DeverQuestShopService.Records
                    : DeverQuestShopService.Records.Where(
                        record => record.accountId == accountId);
            foreach (DeverQuestPurchaseRecord record
                     in visible.Take(20))
            {
                EditorGUILayout.LabelField(
                    $"[{record.status}] {record.itemName}",
                    $"{record.adventurerName} · " +
                    DeverQuestAdventurerService.FormatCoins(
                        record.copperCost));
                if (!string.IsNullOrWhiteSpace(
                        record.fulfillmentReference))
                {
                    EditorGUILayout.LabelField(
                        "Delivery",
                        record.fulfillmentReference);
                }
            }
        }

        private static void DrawAbility(
            string label,
            DeverQuestAdventurer adventurer,
            DeverQuestAbility ability)
        {
            int score = DeverQuestRulesService.GetAbilityScore(
                adventurer, ability);
            int modifier =
                DeverQuestRulesService.AbilityModifier(score);
            EditorGUILayout.LabelField(
                label,
                $"{score} ({(modifier >= 0 ? "+" : string.Empty)}" +
                $"{modifier})");
        }

        private void DrawRulesLaboratory(
            DeverQuestProfile profile)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Rules Laboratory",
                EditorStyles.boldLabel);
            rulesAbility =
                (DeverQuestAbility)EditorGUILayout.EnumPopup(
                    "Ability", rulesAbility);
            rulesDifficultyClass = EditorGUILayout.IntField(
                "Difficulty Class", rulesDifficultyClass);
            rulesProficient = EditorGUILayout.Toggle(
                "Proficient", rulesProficient);
            rulesSeed = EditorGUILayout.TextField(
                new GUIContent(
                    "Recorded Seed",
                    "The same character, rules, and seed produce the same " +
                    "roll so future encounters can be audited."),
                rulesSeed);
            if (GUILayout.Button("Resolve Deterministic Check"))
            {
                DeverQuestRuleResult result =
                    DeverQuestRulesService.ResolveCheck(
                        DeverQuestAdventurerService.Adventurer,
                        rulesAbility,
                        rulesProficient,
                        rulesDifficultyClass,
                        rulesSeed,
                        profile.dailyDecreeCheckModifier);
                rulesResult =
                    $"{(result.Success ? "SUCCESS" : "FAILURE")} · " +
                    $"{result.Formula} vs DC {result.DifficultyClass}";
            }
            if (!string.IsNullOrWhiteSpace(rulesResult))
            {
                EditorGUILayout.HelpBox(
                    rulesResult,
                    MessageType.Info);
            }
            if (DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Equipment Asset…"))
                {
                    CreateCharacterAsset<DeverQuestEquipment>(
                        "NewDeverQuestEquipment");
                }
                if (GUILayout.Button("Create Spell Asset…"))
                {
                    CreateCharacterAsset<DeverQuestSpell>(
                        "NewDeverQuestSpell");
                }
                if (GUILayout.Button("Create Attack Technique…"))
                {
                    CreateCharacterAsset<DeverQuestAttackTechnique>(
                        "NewCombatTechnique");
                }
                if (GUILayout.Button("Create Ability Profile…"))
                {
                    CreateCharacterAsset<DeverQuestAbilityProfile>(
                        "NewAbilityProfile");
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Monster Profile…"))
                {
                    CreateCharacterAsset<DeverQuestMonsterProfile>(
                        "NewMonsterProfile");
                }
                if (GUILayout.Button("Create Encounter Profile…"))
                {
                    CreateCharacterAsset<DeverQuestEncounterProfile>(
                        "NewEncounterProfile");
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button(
                        "Generate Guildhall Training Encounter"))
                {
                    DeverQuestEncounterProfile encounter =
                        DeverQuestStarterContentGenerator
                            .GenerateTrainingEncounter();
                    Selection.activeObject = encounter;
                    EditorGUIUtility.PingObject(encounter);
                    rulesResult =
                        "Generated a safe two-wave training Encounter " +
                        "under Assets/DeverQuest/Encounters.";
                }
                if (GUILayout.Button(
                        "Generate Tactical Starter Kit + Quest Templates"))
                {
                    DeverQuestTacticalContentReport report =
                        DeverQuestTacticalContentGenerator
                            .GenerateStarterKit();
                    UnityEngine.Object folder =
                        AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(
                            report.RootPath);
                    Selection.activeObject = folder;
                    EditorGUIUtility.PingObject(folder);
                    rulesResult =
                        $"Tactical kit ready: {report.Created} created, " +
                        $"{report.Updated} updated. Includes original " +
                        "abilities, class profiles, a 15-minute skirmish, " +
                        "and an endless survival Quest.";
                }
                if (GUILayout.Button("Create Starter Loadout…"))
                {
                    CreateCharacterAsset<DeverQuestStarterLoadout>(
                        "NewStarterLoadout");
                }
                if (GUILayout.Button(
                        "Generate Copper–Steel Starter Gear"))
                {
                    int created =
                        DeverQuestStarterContentGenerator
                            .GenerateBasicGear();
                    rulesResult =
                        $"Generated {created} new starter gear asset(s) " +
                        "under Assets/DeverQuest/StarterGear.";
                }
                if (GUILayout.Button(
                        "Generate Guild Combat Codex"))
                {
                    DeverQuestCombatTypeCatalog catalog =
                        DeverQuestStarterContentGenerator
                            .GenerateCombatCodex();
                    Selection.activeObject = catalog;
                    EditorGUIUtility.PingObject(catalog);
                    rulesResult =
                        "Generated the complete creature and damage-type " +
                        "codex under Assets/DeverQuest/Combat.";
                }
                selectedRulesEquipment =
                    (DeverQuestEquipment)EditorGUILayout.ObjectField(
                        "Equipment",
                        selectedRulesEquipment,
                        typeof(DeverQuestEquipment),
                        false);
                using (new EditorGUI.DisabledScope(
                           selectedRulesEquipment == null ||
                           (selectedRulesEquipment != null &&
                            DeverQuestAdventurerService.Adventurer.level <
                            selectedRulesEquipment.minimumLevel)))
                {
                    if (GUILayout.Button("Grant and Equip"))
                    {
                        DeverQuestAdventurer character =
                            DeverQuestAdventurerService.Adventurer;
                        DeverQuestRulesService.Equip(
                            character, selectedRulesEquipment);
                        DeverQuestAdventurerService.Save();
                        DeverQuestGuildAccountService.AddAudit(
                            "Equipment Granted",
                            character.characterName,
                            selectedRulesEquipment.displayName);
                    }
                }
                selectedRulesSpell =
                    (DeverQuestSpell)EditorGUILayout.ObjectField(
                        "Spell",
                        selectedRulesSpell,
                        typeof(DeverQuestSpell),
                        false);
                using (new EditorGUI.DisabledScope(
                           selectedRulesSpell == null ||
                           (selectedRulesSpell != null &&
                            DeverQuestAdventurerService.Adventurer.level <
                            selectedRulesSpell.minimumCharacterLevel)))
                {
                    if (GUILayout.Button("Teach Spell"))
                    {
                        DeverQuestAdventurer character =
                            DeverQuestAdventurerService.Adventurer;
                        if (!character.knownSpellIds.Contains(
                                selectedRulesSpell.SpellId))
                        {
                            character.knownSpellIds.Add(
                                selectedRulesSpell.SpellId);
                            DeverQuestAdventurerService.Save();
                        }
                        DeverQuestGuildAccountService.AddAudit(
                            "Spell Taught",
                            character.characterName,
                            selectedRulesSpell.displayName);
                    }
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private static T CreateCharacterAsset<T>(
            string defaultName)
            where T : ScriptableObject
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create DeverQuest Character Asset",
                defaultName,
                "asset",
                "Choose where to save the character rules asset.");
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            T asset = CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
            return asset;
        }

        private static void DrawCampaignRulesSetup(
            DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Daily Decree Campaign Rules",
                EditorStyles.boldLabel);
            profile.dailyDecreeRecommendedLevel =
                EditorGUILayout.IntField(
                    "Recommended Level",
                    profile.dailyDecreeRecommendedLevel);
            profile.campaignDifficulty =
                (DeverQuestCampaignDifficulty)
                EditorGUILayout.EnumPopup(
                    "Campaign Difficulty",
                    profile.campaignDifficulty);
            profile.dailyDecreeCheckModifier =
                EditorGUILayout.IntSlider(
                    new GUIContent(
                        "Check Modifier",
                        "Applied to deterministic ability and saving-throw " +
                        "checks for the active Daily Decree."),
                    profile.dailyDecreeCheckModifier,
                    -10,
                    10);
        }

        private static string DrawStringPopup(
            string label,
            string current,
            string[] options)
        {
            int index = Array.IndexOf(options, current);
            index = Mathf.Max(0, index);
            return options[EditorGUILayout.Popup(label, index, options)];
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

                EditorGUILayout.Space(4f);
                EditorGUILayout.LabelField(
                    "Completed Break Benefits",
                    EditorStyles.miniBoldLabel);
                profile.wellnessShortBreakMinutes =
                    EditorGUILayout.IntField(
                        "Short Break (min)",
                        profile.wellnessShortBreakMinutes);
                profile.wellnessMealBreakMinutes =
                    EditorGUILayout.IntField(
                        "Meal Break (min)",
                        profile.wellnessMealBreakMinutes);
                profile.wellnessQuietBreakMinutes =
                    EditorGUILayout.IntField(
                        "Quiet Break (min)",
                        profile.wellnessQuietBreakMinutes);
                profile.wellnessBreakExperience =
                    EditorGUILayout.IntField(
                        "Completed Break XP",
                        profile.wellnessBreakExperience);

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

            EditorGUILayout.HelpBox(
                "Acknowledge Only dismisses the reminder without recording " +
                "a break benefit. Take Approved Break pauses the Quest, " +
                "classifies the permitted time separately, and grants the " +
                "configured wellness benefit only after at least 80% of the " +
                "break is completed.",
                MessageType.Info);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Take Approved Break"))
            {
                DeverQuestWellnessMonitor.Acknowledge(true);
                Repaint();
            }

            if (GUILayout.Button("Acknowledge Only"))
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

            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            bool canCreateCustomQuest =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageContracts,
                    newProjectName) ||
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);

            EditorGUI.BeginChangeCheck();
            selectedQuestContract =
                (DeverQuestQuestContract)EditorGUILayout.ObjectField(
                    "Quest Contract",
                    selectedQuestContract,
                    typeof(DeverQuestQuestContract),
                    false);
            if (EditorGUI.EndChangeCheck())
            {
                ApplySelectedQuestContract();
            }

            string contractJoinReason = string.Empty;
            bool contractAssignedToAdventurer =
                selectedQuestContract != null &&
                DeverQuestContractService.CanJoin(
                    selectedQuestContract,
                    adventurer,
                    out contractJoinReason);
            bool contractStatusAvailable =
                selectedQuestContract != null &&
                (selectedQuestContract.status ==
                 DeverQuestContractStatus.Offered ||
                 selectedQuestContract.status ==
                 DeverQuestContractStatus.Accepted ||
                 (selectedQuestContract.groupQuest &&
                  selectedQuestContract.status ==
                  DeverQuestContractStatus.Active) ||
                 (canCreateCustomQuest &&
                  selectedQuestContract.status ==
                  DeverQuestContractStatus.Draft));
            bool contractUnavailable =
                selectedQuestContract != null &&
                (!contractStatusAvailable ||
                 (!canCreateCustomQuest &&
                  (!contractAssignedToAdventurer ||
                   adventurer.level <
                   selectedQuestContract.minimumAdventurerLevel)));

            if (!canCreateCustomQuest && selectedQuestContract == null)
            {
                EditorGUILayout.HelpBox(
                    "Members must select an assigned or open Quest Contract.",
                    MessageType.Warning);
            }
            else if (contractUnavailable)
            {
                EditorGUILayout.HelpBox(
                    !contractStatusAvailable
                        ? $"This Contract is {selectedQuestContract.status} " +
                          "and cannot currently be accepted."
                        : contractJoinReason,
                    MessageType.Warning);
            }

            using (new EditorGUI.DisabledScope(!canCreateCustomQuest))
            {
            EditorGUI.BeginChangeCheck();
            selectedQuestProfile =
                (DeverQuestQuestProfile)EditorGUILayout.ObjectField(
                    "Quest Profile",
                    selectedQuestProfile,
                    typeof(DeverQuestQuestProfile),
                    false);
            if (EditorGUI.EndChangeCheck())
            {
                ApplySelectedQuestProfile();
            }
            }

            if (canCreateCustomQuest)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Quest Profile…"))
                {
                    CreateQuestProfileAsset();
                }
                if (GUILayout.Button("Create Quest Contract…"))
                {
                    CreateQuestContractAsset(adventurer);
                }
                if (selectedQuestProfile != null &&
                    GUILayout.Button("Inspect Selected Profile"))
                {
                    Selection.activeObject = selectedQuestProfile;
                    EditorGUIUtility.PingObject(selectedQuestProfile);
                }
                EditorGUILayout.EndHorizontal();
                if (selectedQuestContract != null &&
                    GUILayout.Button("Inspect Selected Contract"))
                {
                    Selection.activeObject = selectedQuestContract;
                    EditorGUIUtility.PingObject(selectedQuestContract);
                }
            }

            DrawContractBoard(adventurer, canCreateCustomQuest);

            bool profileUnavailable =
                !canCreateCustomQuest &&
                selectedQuestContract == null &&
                selectedQuestProfile != null &&
                (!selectedQuestProfile.availableToMembers ||
                 adventurer.level <
                 selectedQuestProfile.minimumAdventurerLevel);
            if (profileUnavailable)
            {
                EditorGUILayout.HelpBox(
                    !selectedQuestProfile.availableToMembers
                        ? "This Quest Profile is reserved for Guild leadership."
                        : $"Requires Adventurer Level " +
                          $"{selectedQuestProfile.minimumAdventurerLevel}.",
                    MessageType.Warning);
            }

            bool lockQuestFields =
                !canCreateCustomQuest &&
                selectedQuestContract != null;

            using (new EditorGUI.DisabledScope(
                       profile.lockProjectName || lockQuestFields))
            {
                newProjectName = EditorGUILayout.TextField(
                    "Project",
                    profile.lockProjectName
                        ? profile.lockedProjectName
                        : newProjectName);
            }

            using (new EditorGUI.DisabledScope(lockQuestFields))
            {
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
            }

            DrawSelectedQuestSpoils(canCreateCustomQuest);
            if (selectedQuestContract != null)
            {
                EditorGUILayout.LabelField(
                    "Contract",
                    $"{selectedQuestContract.status} · " +
                    $"{selectedQuestContract.priority} · Due " +
                    $"{(string.IsNullOrWhiteSpace(selectedQuestContract.dueDate) ? "Unscheduled" : selectedQuestContract.dueDate)}");
                EditorGUILayout.LabelField(
                    "Assigned To",
                    selectedQuestContract.openToAnyMember
                        ? "Any eligible Member"
                        : selectedQuestContract.assignedAdventurer);
            }

            bool canStart =
                !string.IsNullOrWhiteSpace(newProjectName) &&
                !string.IsNullOrWhiteSpace(newTaskName) &&
                !profileUnavailable &&
                !contractUnavailable &&
                (canCreateCustomQuest ||
                 selectedQuestContract != null);

            using (new EditorGUI.DisabledScope(!canStart))
            {
                if (GUILayout.Button(
                        "Accept Quest",
                        GUILayout.Height(36f)))
                {
                    if (selectedQuestContract != null &&
                        !DeverQuestContractService.Join(
                            selectedQuestContract,
                            adventurer,
                            profile.developerName,
                            out string joinError))
                    {
                        EditorUtility.DisplayDialog(
                            "Cannot Join Quest",
                            joinError,
                            "Close");
                        return;
                    }
                    if (selectedQuestContract != null &&
                        selectedQuestContract.groupQuest &&
                        selectedQuestContract.HasOpenPartySlot)
                    {
                        EditorUtility.DisplayDialog(
                            "Party Joined",
                            "Your place is reserved. This Quest can begin " +
                            "after the required party has assembled.",
                            "Return to the Guild Hall");
                        Repaint();
                        return;
                    }
                    profile.lastProjectName = newProjectName;
                    profile.lastDepartmentName = newCategory;
                    DeverQuestSettingsStore.Save();
                    DeverQuestSessionStore.StartSession(
                        profile.developerName,
                        newProjectName,
                        newTaskName,
                        newCategory,
                        newGoal,
                        selectedQuestProfile,
                        selectedQuestContract);

                    if (selectedQuestContract != null)
                    {
                        DeverQuestContractService.SetStatus(
                            selectedQuestContract,
                            DeverQuestContractStatus.Active);
                    }

                    Repaint();
                }
            }
        }

        private void DrawSelectedQuestSpoils(
            bool canManageQuest)
        {
            if (selectedQuestContract != null)
            {
                EditorGUILayout.LabelField(
                    "Effective Contract Spoils",
                    FormatSpoils(
                        selectedQuestContract.baseCopper,
                        selectedQuestContract.baseExperience,
                        selectedQuestContract.copperPerWorkBlock,
                        selectedQuestContract.experiencePerWorkBlock,
                        selectedQuestContract.workBlockMinutes));

                if (selectedQuestContract.questProfile != null &&
                    !selectedQuestContract.SpoilsMatchLinkedProfile())
                {
                    EditorGUILayout.HelpBox(
                        "This Contract's snapshotted Spoils differ from its " +
                        "linked Quest Profile. The Contract values shown " +
                        "above are the values that will actually be awarded.",
                        MessageType.Warning);
                    EditorGUILayout.LabelField(
                        "Linked Profile Spoils",
                        FormatSpoils(
                            selectedQuestContract.questProfile.baseCopper,
                            selectedQuestContract.questProfile.baseExperience,
                            selectedQuestContract.questProfile
                                .copperPerWorkBlock,
                            selectedQuestContract.questProfile
                                .experiencePerWorkBlock,
                            selectedQuestContract.questProfile
                                .workBlockMinutes));

                    using (new EditorGUI.DisabledScope(
                               !canManageQuest ||
                               !selectedQuestContract
                                   .CanRefreshSpoilsFromProfile()))
                    {
                        if (GUILayout.Button(
                                "Refresh Contract Spoils from Linked Profile"))
                        {
                            selectedQuestContract
                                .RefreshSpoilsFromProfile();
                            EditorUtility.SetDirty(
                                selectedQuestContract);
                            AssetDatabase.SaveAssets();
                            Repaint();
                        }
                    }
                }
                return;
            }

            if (selectedQuestProfile != null)
            {
                EditorGUILayout.LabelField(
                    "Profile Spoils",
                    FormatSpoils(
                        selectedQuestProfile.baseCopper,
                        selectedQuestProfile.baseExperience,
                        selectedQuestProfile.copperPerWorkBlock,
                        selectedQuestProfile.experiencePerWorkBlock,
                        selectedQuestProfile.workBlockMinutes));
            }
        }

        private static string FormatSpoils(
            int baseCopper,
            int baseExperience,
            int copperPerWorkBlock,
            int experiencePerWorkBlock,
            int workBlockMinutes)
        {
            return
                $"{DeverQuestAdventurerService.FormatCoins(baseCopper)} " +
                $"+ {baseExperience} XP base · " +
                $"{DeverQuestAdventurerService.FormatCoins(copperPerWorkBlock)} " +
                $"+ {experiencePerWorkBlock} XP per " +
                $"{Math.Max(1, workBlockMinutes)}m block";
        }

        private void ApplySelectedQuestContract()
        {
            if (selectedQuestContract == null)
            {
                appliedQuestContractId = string.Empty;
                return;
            }

            if (appliedQuestContractId ==
                selectedQuestContract.ContractId)
            {
                return;
            }

            appliedQuestContractId =
                selectedQuestContract.ContractId;

            bool canRefreshContract =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageContracts,
                    selectedQuestContract.projectName) ||
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);
            if (canRefreshContract &&
                selectedQuestContract.CanRefreshSpoilsFromProfile() &&
                !selectedQuestContract.SpoilsMatchLinkedProfile())
            {
                selectedQuestContract.RefreshSpoilsFromProfile();
                EditorUtility.SetDirty(selectedQuestContract);
                AssetDatabase.SaveAssets();
            }

            selectedQuestProfile =
                selectedQuestContract.questProfile;
            appliedQuestProfileId = selectedQuestProfile == null
                ? string.Empty
                : selectedQuestProfile.ProfileId;
            newProjectName = selectedQuestContract.projectName;
            newTaskName = selectedQuestContract.taskName;
            newCategory = selectedQuestContract.department;
            newGoal = selectedQuestContract.objective;
        }

        private void ApplySelectedQuestProfile()
        {
            if (selectedQuestProfile == null)
            {
                appliedQuestProfileId = string.Empty;
                return;
            }

            if (appliedQuestProfileId ==
                selectedQuestProfile.ProfileId)
            {
                return;
            }

            appliedQuestProfileId = selectedQuestProfile.ProfileId;
            if (!string.IsNullOrWhiteSpace(
                    selectedQuestProfile.projectName))
            {
                newProjectName = selectedQuestProfile.projectName;
            }
            if (!string.IsNullOrWhiteSpace(
                    selectedQuestProfile.taskName))
            {
                newTaskName = selectedQuestProfile.taskName;
            }
            newCategory = selectedQuestProfile.department;
            newGoal = selectedQuestProfile.goalTemplate;
        }

        private void CreateQuestProfileAsset()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create DeverQuest Quest Profile",
                "NewDeverQuestProfile",
                "asset",
                "Choose where this Guild Quest Profile should be saved.");
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            DeverQuestQuestProfile questProfile =
                CreateInstance<DeverQuestQuestProfile>();
            AssetDatabase.CreateAsset(questProfile, path);
            AssetDatabase.SaveAssets();
            selectedQuestProfile = questProfile;
            appliedQuestProfileId = string.Empty;
            ApplySelectedQuestProfile();
            Selection.activeObject = questProfile;
            EditorGUIUtility.PingObject(questProfile);
        }

        private void CreateQuestContractAsset(
            DeverQuestAdventurer adventurer)
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create DeverQuest Quest Contract",
                "NewQuestContract",
                "asset",
                "Choose where this assigned work Contract should be saved.");
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            DeverQuestQuestContract contract =
                CreateInstance<DeverQuestQuestContract>();
            contract.InitializeFromProfile(
                selectedQuestProfile,
                adventurer.characterName);
            AssetDatabase.CreateAsset(contract, path);
            AssetDatabase.SaveAssets();
            selectedQuestContract = contract;
            appliedQuestContractId = string.Empty;
            ApplySelectedQuestContract();
            Selection.activeObject = contract;
            EditorGUIUtility.PingObject(contract);
        }

        private void DrawContractBoard(
            DeverQuestAdventurer adventurer,
            bool canManage)
        {
            contractBoardFoldout = EditorGUILayout.Foldout(
                contractBoardFoldout,
                "Guild Assignment Board",
                true);
            if (!contractBoardFoldout)
            {
                return;
            }

            string[] guids =
                AssetDatabase.FindAssets("t:DeverQuestQuestContract");
            int visibleCount = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DeverQuestQuestContract contract =
                    AssetDatabase.LoadAssetAtPath<DeverQuestQuestContract>(
                        path);
                if (contract == null)
                {
                    continue;
                }
                bool canManageContract =
                    DeverQuestGuildAccountService.HasPermission(
                        DeverQuestGuildPermission.ManageContracts,
                        contract.projectName);

                bool assigned =
                    DeverQuestContractService.CanJoin(
                        contract,
                        adventurer,
                        out string joinReason);
                bool memberVisible =
                    assigned &&
                    adventurer.level >=
                    contract.minimumAdventurerLevel &&
                    (contract.status ==
                     DeverQuestContractStatus.Offered ||
                     contract.status ==
                     DeverQuestContractStatus.Accepted ||
                     contract.status ==
                     DeverQuestContractStatus.Active);
                if (!canManageContract && !memberVisible)
                {
                    continue;
                }

                visibleCount++;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    contract.contractTitle,
                    $"{contract.status} · {contract.priority}");
                EditorGUILayout.LabelField(
                    "Assignee",
                    contract.openToAnyMember
                        ? "Any eligible Member"
                        : contract.assignedAdventurer);
                EditorGUILayout.LabelField(
                    "Due",
                    string.IsNullOrWhiteSpace(contract.dueDate)
                        ? "Unscheduled"
                        : contract.dueDate);
                EditorGUILayout.LabelField(
                    contract.groupQuest ? "Party" : "Capacity",
                    contract.groupQuest
                        ? $"{contract.partyMembers.Count}/" +
                          $"{contract.maximumParticipants} joined"
                        : "1 Adventurer");
                if (!assigned &&
                    !string.IsNullOrWhiteSpace(joinReason))
                {
                    EditorGUILayout.HelpBox(
                        joinReason, MessageType.Warning);
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select"))
                {
                    selectedQuestContract = contract;
                    appliedQuestContractId = string.Empty;
                    ApplySelectedQuestContract();
                }
                if (canManageContract)
                {
                    if ((contract.status ==
                         DeverQuestContractStatus.Draft ||
                         contract.status ==
                         DeverQuestContractStatus.Returned) &&
                        GUILayout.Button("Offer"))
                    {
                        DeverQuestContractService.SetStatus(
                            contract,
                            DeverQuestContractStatus.Offered);
                    }
                    else if (contract.status ==
                             DeverQuestContractStatus.Submitted)
                    {
                        if (GUILayout.Button("Return"))
                        {
                            DeverQuestContractService.SetStatus(
                                contract,
                                DeverQuestContractStatus.Returned);
                        }
                        if (GUILayout.Button("Approve"))
                        {
                            DeverQuestContractService.SetStatus(
                                contract,
                                DeverQuestContractStatus.Approved);
                        }
                    }
                    else if (contract.status ==
                             DeverQuestContractStatus.Approved &&
                             GUILayout.Button("Complete"))
                    {
                        DeverQuestContractService.SetStatus(
                            contract,
                            DeverQuestContractStatus.Completed);
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            if (visibleCount == 0)
            {
                EditorGUILayout.LabelField(
                    canManage
                        ? "No Quest Contracts have been created."
                        : "No Contracts are currently assigned to you.",
                    EditorStyles.miniLabel);
            }
        }

        private static void DrawQuestProgressSummary(
            DeverQuestSession session)
        {
            double focusedSeconds =
                DeverQuestSessionStore.GetFocusedSeconds();
            int stagedMinutes = session.questStages == null
                ? 0
                : session.questStages.Sum(stage =>
                    stage == null
                        ? 0
                        : Math.Max(0, stage.focusedMinutesRequired));
            int targetMinutes =
                session.questSuggestedFocusMinutes > 0
                    ? session.questSuggestedFocusMinutes
                    : stagedMinutes;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Quest Progress",
                EditorStyles.boldLabel);

            if (targetMinutes <= 0)
            {
                EditorGUILayout.LabelField(
                    "Time Remaining",
                    "No target duration configured");
                EditorGUILayout.HelpBox(
                    "Focused time is being recorded normally. Assign a " +
                    "suggested duration to the Quest Profile or Contract " +
                    "to enable pacing feedback.",
                    MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            double targetSeconds = targetMinutes * 60d;
            double remainingSeconds =
                targetSeconds - focusedSeconds;
            float progress = Mathf.Clamp01(
                (float)(focusedSeconds / targetSeconds));
            Rect progressRect = GUILayoutUtility.GetRect(
                18f,
                22f,
                GUILayout.ExpandWidth(true));
            EditorGUI.ProgressBar(
                progressRect,
                progress,
                $"{progress * 100f:0}% of {targetMinutes} minutes");

            EditorGUILayout.LabelField(
                remainingSeconds >= 0d
                    ? "Time Remaining"
                    : "Beyond Target",
                FormatDuration(Math.Abs(remainingSeconds)));

            int totalStages = session.questStages == null
                ? 0
                : session.questStages.Count(stage => stage != null);
            if (totalStages > 0)
            {
                int completedStages = session.questStages.Count(stage =>
                    stage != null && stage.completed);
                DeverQuestSessionStage currentStage =
                    DeverQuestSessionStore.CurrentQuestStage();
                string stageLabel = currentStage == null
                    ? $"{completedStages} of {totalStages} complete"
                    : $"{Math.Min(totalStages, completedStages + 1)} of " +
                      $"{totalStages}: {currentStage.stageTitle}";
                EditorGUILayout.LabelField(
                    "Current Encounter",
                    stageLabel);
            }

            EditorGUILayout.LabelField(
                "Progress Report",
                BuildQuestProgressFeedback(
                    session,
                    progress,
                    remainingSeconds),
                EditorStyles.wordWrappedLabel);

            if (session.usesQuestProfile)
            {
                int blockMinutes =
                    Math.Max(1, session.questWorkBlockMinutes);
                int completedBlocks = (int)Math.Floor(
                    focusedSeconds / (blockMinutes * 60d));
                long projectedCopper =
                    session.questBaseCopper +
                    completedBlocks *
                    (long)session.questCopperPerWorkBlock;
                long projectedExperience =
                    session.questBaseExperience +
                    completedBlocks *
                    (long)session.questExperiencePerWorkBlock;
                EditorGUILayout.LabelField(
                    "Current Spoils Estimate",
                    DeverQuestAdventurerService.FormatCoins(
                        projectedCopper) +
                    $" + {projectedExperience} XP");
            }

            EditorGUILayout.EndVertical();
        }

        private static string BuildQuestProgressFeedback(
            DeverQuestSession session,
            float progress,
            double remainingSeconds)
        {
            if (session.state == DeverQuestSessionState.Paused)
            {
                return string.IsNullOrWhiteSpace(session.pauseReason)
                    ? "The Quest is paused. Focused time is not increasing."
                    : "The Quest is paused: " + session.pauseReason + ".";
            }

            DeverQuestSessionStage currentStage =
                DeverQuestSessionStore.CurrentQuestStage();
            if (currentStage != null &&
                (currentStage.survivalMode ||
                 DeverQuestEncounterService.IsSurvival(currentStage)))
            {
                return currentStage.survivalFightPaused
                    ? "The Survival Encounter is paused at a safe decision " +
                      "point."
                    : "The party is advancing through Survival wave " +
                      Math.Max(1, currentStage.survivalWave) + ".";
            }

            if (remainingSeconds < 0d)
            {
                return "The planned duration has passed, but the Quest is " +
                       "still recording valid focused work.";
            }
            if (progress >= 0.9f)
            {
                return "The Quest is in its final stretch.";
            }
            if (progress >= 0.5f)
            {
                return "More than half of the planned Quest time is " +
                       "complete.";
            }
            if (progress >= 0.25f)
            {
                return "The Quest is underway and focused progress is being " +
                       "recorded.";
            }
            return "The Quest has begun. Establish the current objective and " +
                   "build momentum.";
        }

        private void DrawActiveSession(bool includeQuestLog = true)
        {
            DeverQuestSession session =
                DeverQuestSessionStore.ActiveSession;
            AnnounceCompletedStages();

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
            if (DeverQuestExternalActivityMonitor
                .HasRecentConfiguredActivity)
            {
                EditorGUILayout.LabelField(
                    "External craft active: " +
                    DeverQuestExternalActivityMonitor
                        .ActiveProviderName,
                    subtitleStyle,
                    GUILayout.Height(22f));
            }
            EditorGUILayout.EndVertical();

            DrawQuestProgressSummary(session);

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
            if (session.usesQuestProfile)
            {
                DrawReadOnlyValue(
                    "Quest Profile",
                    session.questProfileName);
                DrawReadOnlyValue(
                    "Suggested Focus",
                    $"{session.questSuggestedFocusMinutes} minutes");
            }
            if (session.usesQuestContract)
            {
                DrawReadOnlyValue(
                    "Quest Contract",
                    session.questContractTitle);
                DrawReadOnlyValue(
                    "Assignment",
                    $"{session.questContractAssignee} · " +
                    $"{session.questContractPriority} · Due " +
                    $"{(string.IsNullOrWhiteSpace(session.questContractDueDate) ? "Unscheduled" : session.questContractDueDate)}");
                if (!string.IsNullOrWhiteSpace(
                        session.questContractDeliverables))
                {
                    EditorGUILayout.LabelField(
                        "Deliverables",
                        session.questContractDeliverables,
                        wrappedLabelStyle);
                }
            }
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

            DeverQuestSessionStage activeSessionStage =
                DeverQuestSessionStore.CurrentQuestStage();
            if (activeSessionStage != null)
            {
                EditorGUILayout.Space(8f);
                double stageElapsed = Math.Max(
                    0d,
                    DeverQuestSessionStore.GetFocusedSeconds() -
                    activeSessionStage.startedFocusedSeconds);
                EditorGUILayout.LabelField(
                    "Current Focus Stage",
                    $"{activeSessionStage.stageTitle} · " +
                    $"{stageElapsed / 60d:0.0}/" +
                    $"{activeSessionStage.focusedMinutesRequired}m");
                if (activeSessionStage.survivalMode ||
                    DeverQuestEncounterService.IsSurvival(
                        activeSessionStage))
                {
                    DrawSurvivalControls(activeSessionStage);
                }
                else
                {
                    using (new EditorGUI.DisabledScope(
                               !activeSessionStage.allowEarlyTurnIn))
                    {
                        if (GUILayout.Button(
                                "Report Development Objective Complete"))
                        {
                            DeverQuestSessionStore
                                .CompleteCurrentStageEarly(
                                    out string message);
                            EditorUtility.DisplayDialog(
                                "Quest Stage Pace",
                                message,
                                "Continue");
                        }
                    }
                }
            }
            DrawBattleResults(session);
            if (includeQuestLog)
            {
                DrawCommitJournal(session);
            }

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
                DeverQuestSessionStage currentStage =
                    DeverQuestSessionStore.CurrentQuestStage();
                if (currentStage?.survivalFightPaused == true)
                {
                    if (DeverQuestSessionStore.ContinueSurvival(
                            out string continueMessage))
                    {
                        DeverQuestSessionStore.ResumeSession();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog(
                            "Survival Expedition",
                            continueMessage,
                            "Close");
                    }
                }
                else
                {
                    DeverQuestSessionStore.ResumeSession();
                }
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
                    if (session.usesQuestContract)
                    {
                        DeverQuestContractService.SetStatus(
                            session.questContractId,
                            DeverQuestContractStatus.Returned);
                    }
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
            DrawExternalActivityAndMedia(session);

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
                new GUIContent(
                    "Branch Context",
                    "Recorded with notes, but does not create a commit."),
                commitBranch);
            commitHash = EditorGUILayout.TextField(
                new GUIContent(
                    "Current HEAD",
                    "Attached only by Link Note to Current Commit."),
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
                        string.Empty,
                        "Quest Log Note");

                    commitComment = string.Empty;
                    commitHash = string.Empty;
                    Repaint();
                }

                using (new EditorGUI.DisabledScope(
                           gitStatus == null ||
                           !gitStatus.IsRepository ||
                           string.IsNullOrWhiteSpace(
                               gitStatus.ShortHash)))
                {
                    if (GUILayout.Button(
                            "Link Note to Current Commit"))
                    {
                        DeverQuestSessionStore.AddCommitEntry(
                            commitComment,
                            gitStatus.Branch,
                            gitStatus.ShortHash,
                            "Linked Commit Note");

                        commitComment = string.Empty;
                        commitBranch = gitStatus.Branch;
                        commitHash = gitStatus.ShortHash;
                        Repaint();
                    }
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
                    $"{index + 1}. [{entry.entryType}] " +
                    $"{entry.comment} " +
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

        private void DrawExternalActivityAndMedia(
            DeverQuestSession session)
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "External Craft and Voice Chronicle",
                EditorStyles.boldLabel);

            DeverQuestExternalActivityProfile profile =
                (DeverQuestExternalActivityProfile)
                EditorGUILayout.ObjectField(
                    "Activity Profile",
                    DeverQuestExternalActivityMonitor.Profile,
                    typeof(DeverQuestExternalActivityProfile),
                    false);
            if (profile !=
                DeverQuestExternalActivityMonitor.Profile)
            {
                DeverQuestExternalActivityMonitor.SetProfile(
                    profile);
            }

            string activityStatus =
                DeverQuestExternalActivityMonitor
                    .HasRecentConfiguredActivity
                    ? "Active: " +
                      DeverQuestExternalActivityMonitor
                          .ActiveProviderName
                    : DeverQuestExternalActivityMonitor.IsSupported
                        ? "No configured external tool is currently active."
                        : "Foreground external-tool detection is currently " +
                          "available on Windows only.";
            EditorGUILayout.HelpBox(
                activityStatus,
                DeverQuestExternalActivityMonitor
                    .HasRecentConfiguredActivity
                    ? MessageType.Info
                    : MessageType.None);

            EditorGUILayout.Space(4f);
            voiceMemoName = EditorGUILayout.TextField(
                "Memo Name",
                voiceMemoName);
            string[] devices =
                DeverQuestVoiceMemoService.Devices;
            if (devices.Length > 0)
            {
                selectedMicrophoneIndex = Mathf.Clamp(
                    selectedMicrophoneIndex,
                    0,
                    devices.Length - 1);
                selectedMicrophoneIndex =
                    EditorGUILayout.Popup(
                        "Microphone",
                        selectedMicrophoneIndex,
                        devices);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "No microphone detected. Existing media files can still " +
                    "be attached.",
                    MessageType.Info);
            }

            EditorGUILayout.BeginHorizontal();
            if (!DeverQuestVoiceMemoService.IsRecording)
            {
                using (new EditorGUI.DisabledScope(
                           devices.Length == 0))
                {
                    if (GUILayout.Button("Record Voice Memo"))
                    {
                        DeverQuestVoiceMemoService.Start(
                            devices[selectedMicrophoneIndex],
                            out mediaMessage);
                    }
                }
            }
            else
            {
                if (GUILayout.Button(
                        $"Stop and Attach " +
                        $"({DeverQuestVoiceMemoService.RecordingSeconds:0}s)"))
                {
                    DeverQuestVoiceMemoService.StopAndAttach(
                        voiceMemoName,
                        out mediaMessage);
                }
                if (GUILayout.Button("Cancel Recording"))
                {
                    DeverQuestVoiceMemoService.CancelRecording();
                    mediaMessage =
                        "Voice memo recording cancelled.";
                }
            }

            if (GUILayout.Button("Attach Existing File…"))
            {
                string sourcePath = EditorUtility.OpenFilePanel(
                    "Attach Media to Current Quest",
                    string.Empty,
                    string.Empty);
                if (!string.IsNullOrWhiteSpace(sourcePath))
                {
                    DeverQuestVoiceMemoService.AttachExistingFile(
                        sourcePath,
                        out mediaMessage);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrWhiteSpace(mediaMessage))
            {
                EditorGUILayout.HelpBox(
                    mediaMessage,
                    mediaMessage.Contains("failed") ||
                    mediaMessage.Contains("could not") ||
                    mediaMessage.Contains("No microphone")
                        ? MessageType.Warning
                        : MessageType.Info);
            }

            if (session.mediaAttachments != null)
            {
                foreach (DeverQuestMediaAttachment attachment
                         in session.mediaAttachments.ToList())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(
                        $"[{attachment.attachmentType}] " +
                        attachment.displayName,
                        wrappedLabelStyle);
                    if (GUILayout.Button(
                            "Reveal",
                            GUILayout.Width(58f)))
                    {
                        DeverQuestIdleMonitor
                            .BeginIntentionalExternalAction();
                        EditorUtility.RevealInFinder(
                            attachment.filePath);
                    }
                    if (GUILayout.Button(
                            "Unlink",
                            GUILayout.Width(58f)))
                    {
                        DeverQuestSessionStore
                            .RemoveMediaAttachment(
                                attachment.attachmentId);
                        GUIUtility.ExitGUI();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawGitPanel()
        {
            if (DeverQuestGitMonitor.LatestStatus != null)
            {
                gitStatus = DeverQuestGitMonitor.LatestStatus;
            }

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
                DrawReadOnlyValue(
                    "Unity Project",
                    gitStatus.UnityProjectRoot);
                if (GUILayout.Button("Choose Repository Folder…"))
                {
                    ChooseGitRepositoryFolder();
                }
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
            DrawReadOnlyValue(
                "Repository",
                gitStatus.RepositoryRoot);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Choose Different Repository…"))
            {
                ChooseGitRepositoryFolder();
            }
            if (!string.IsNullOrWhiteSpace(
                    DeverQuestSettingsStore.Profile
                        .gitRepositoryOverridePath) &&
                GUILayout.Button("Use Unity Project"))
            {
                DeverQuestSettingsStore.Profile
                    .gitRepositoryOverridePath = string.Empty;
                DeverQuestSettingsStore.Save();
                RefreshGitStatus();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(
                $"Staged: {gitStatus.StagedCount} · " +
                $"Modified: {gitStatus.UnstagedCount} · " +
                $"Untracked: {gitStatus.UntrackedCount}");
            EditorGUILayout.LabelField(
                "Upstream",
                string.IsNullOrWhiteSpace(gitStatus.UpstreamBranch)
                    ? "Not configured"
                    : gitStatus.UpstreamBranch);
            if (!string.IsNullOrWhiteSpace(
                    gitStatus.UpstreamBranch))
            {
                EditorGUILayout.LabelField(
                    $"Ahead: {gitStatus.AheadCount} · " +
                    $"Behind: {gitStatus.BehindCount}");
            }

            EditorGUILayout.HelpBox(
                "Branch = your current development path. Staged files = " +
                "the changes selected for the next Git commit. Hash = the " +
                "unique ID Git assigns after committing. Staging is not " +
                "stashing: a stash temporarily shelves changes.",
                MessageType.Info);

            using (new EditorGUI.DisabledScope(
                       gitOperationInProgress ||
                       !gitStatus.HasStagedChanges ||
                       string.IsNullOrWhiteSpace(commitComment)))
            {
                if (GUILayout.Button("Commit Staged Changes"))
                {
                    CommitWithGit(false);
                }
            }

            using (new EditorGUI.DisabledScope(
                       gitOperationInProgress ||
                       gitStatus.IsClean ||
                       string.IsNullOrWhiteSpace(commitComment)))
            {
                if (GUILayout.Button("Stage All and Commit…"))
                {
                    CommitWithGit(true);
                }
            }

            if (!string.IsNullOrWhiteSpace(
                    gitStatus.UpstreamBranch))
            {
                using (new EditorGUI.DisabledScope(
                           gitOperationInProgress ||
                           !gitStatus.IsClean ||
                           gitStatus.AheadCount <= 0 ||
                           gitStatus.BehindCount > 0))
                {
                    if (GUILayout.Button(
                            $"Push {gitStatus.AheadCount} Commit(s)…"))
                    {
                        PushWithGit(false);
                    }
                }

                if (gitStatus.BehindCount > 0)
                {
                    EditorGUILayout.HelpBox(
                        "The remote branch is ahead. DeverQuest will not " +
                        "push until you pull and resolve the repository " +
                        "through your normal Git client.",
                        MessageType.Warning);
                }
            }
            else
            {
                using (new EditorGUI.DisabledScope(
                           gitOperationInProgress ||
                           !gitStatus.IsClean ||
                           !gitStatus.HasOriginRemote ||
                           string.IsNullOrWhiteSpace(gitStatus.Branch)))
                {
                    if (GUILayout.Button(
                            "Publish Branch to origin…"))
                    {
                        PushWithGit(true);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(gitMessage))
            {
                EditorGUILayout.HelpBox(
                    gitMessage,
                    gitMessage.StartsWith("Git commit created") ||
                    gitMessage.StartsWith("Git push completed") ||
                    gitMessage.StartsWith("Git branch published")
                        ? MessageType.Info
                        : MessageType.Warning);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private async void CommitWithGit(bool stageAll)
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

            }

            string repositoryRoot = gitStatus.RepositoryRoot;
            string committedMessage = commitComment.Trim();
            gitOperationInProgress = true;
            gitMessage = stageAll
                ? "Staging project changes…"
                : "Creating Git commit…";
            Repaint();

            DeverQuestGitResult stageResult = null;
            DeverQuestGitResult commitResult = await Task.Run(() =>
            {
                if (stageAll)
                {
                    stageResult =
                        DeverQuestGitService.StageAll(repositoryRoot);
                    if (!stageResult.Succeeded)
                    {
                        return null;
                    }
                }

                return DeverQuestGitService.CommitStaged(
                    repositoryRoot,
                    committedMessage);
            });

            gitOperationInProgress = false;
            if (this == null)
            {
                return;
            }

            if (stageResult != null && !stageResult.Succeeded)
            {
                gitMessage =
                    $"Git could not stage the changes: " +
                    $"{stageResult.Error}";
                RefreshGitStatus(false);
                Repaint();
                return;
            }

            if (commitResult == null)
            {
                gitMessage = "Git commit was not started.";
                RefreshGitStatus(false);
                Repaint();
                return;
            }

            if (!commitResult.Succeeded)
            {
                gitMessage =
                    $"Git commit failed: {commitResult.Error}";
                RefreshGitStatus(false);
                Repaint();
                return;
            }

            RefreshGitStatus(false);
            DeverQuestGitMonitor.MarkObserved(gitStatus);
            DeverQuestSessionStore.AddCommitEntry(
                committedMessage,
                gitStatus.Branch,
                gitStatus.ShortHash,
                "Git Commit");
            commitBranch = gitStatus.Branch;
            commitHash = gitStatus.ShortHash;
            commitComment = string.Empty;
            gitMessage =
                $"Git commit created: {gitStatus.ShortHash}";
            Repaint();
        }

        private async void PushWithGit(bool publishBranch)
        {
            if (gitStatus == null || !gitStatus.IsRepository)
            {
                gitMessage = "No Git repository is available.";
                return;
            }

            string destination = publishBranch
                ? $"origin/{gitStatus.Branch}"
                : gitStatus.UpstreamBranch;
            bool confirmed = EditorUtility.DisplayDialog(
                publishBranch
                    ? "Publish Git Branch?"
                    : "Push Git Commits?",
                $"Send committed work to:\n\n{destination}\n\n" +
                "This does not include uncommitted files and will never " +
                "force-push.",
                publishBranch ? "Publish Branch" : "Push Commits",
                "Cancel");
            if (!confirmed)
            {
                return;
            }

            string repositoryRoot = gitStatus.RepositoryRoot;
            string branch = gitStatus.Branch;
            string hash = gitStatus.ShortHash;
            gitOperationInProgress = true;
            gitMessage = publishBranch
                ? $"Publishing {destination}…"
                : $"Pushing commits to {destination}…";
            Repaint();

            DeverQuestGitResult result = await Task.Run(() =>
                publishBranch
                    ? DeverQuestGitService.PublishBranch(
                        repositoryRoot,
                        branch)
                    : DeverQuestGitService.Push(repositoryRoot));

            gitOperationInProgress = false;
            if (this == null)
            {
                return;
            }

            if (!result.Succeeded)
            {
                gitMessage = $"Git push failed: {result.Error}";
                RefreshGitStatus(false);
                return;
            }

            RefreshGitStatus(false);
            DeverQuestGitMonitor.MarkObserved(gitStatus);
            DeverQuestSessionStore.AddCommitEntry(
                publishBranch
                    ? $"Published {branch} to origin"
                    : $"Pushed commits to {destination}",
                branch,
                hash,
                "Git Push");
            gitMessage = publishBranch
                ? $"Git branch published: {destination}"
                : $"Git push completed: {destination}";
            Repaint();
        }

        private void RefreshGitStatus(bool clearMessage = true)
        {
            gitStatus = DeverQuestGitService.Refresh();
            DeverQuestGitMonitor.SetLatestStatus(gitStatus);
            if (clearMessage)
            {
                gitMessage = string.Empty;
            }
        }

        private void ChooseGitRepositoryFolder()
        {
            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            string startingPath =
                !string.IsNullOrWhiteSpace(
                    profile.gitRepositoryOverridePath)
                    ? profile.gitRepositoryOverridePath
                    : gitStatus?.UnityProjectRoot ??
                      Path.GetFullPath(
                          Path.Combine(Application.dataPath, ".."));

            string selected = EditorUtility.OpenFolderPanel(
                "Choose the Git Repository Containing This Project",
                startingPath,
                string.Empty);
            if (string.IsNullOrWhiteSpace(selected))
            {
                return;
            }

            profile.gitRepositoryOverridePath = selected;
            DeverQuestSettingsStore.Save();
            RefreshGitStatus();
        }

        private void BeginFinalization(
            DeverQuestSession session)
        {
            DeverQuestSessionStage stage =
                DeverQuestSessionStore.CurrentQuestStage();
            if (stage != null &&
                (stage.survivalMode ||
                 DeverQuestEncounterService.IsSurvival(stage)))
            {
                string exitMethod =
                    stage.survivalExitOffered
                        ? "Wagon"
                        : DeverQuestTacticalCombatService
                            .HasReturnAbility(
                                DeverQuestAdventurerService
                                    .Adventurer)
                        ? "Return"
                        : "Flee";
                if (!DeverQuestSessionStore.TryExitSurvival(
                        exitMethod,
                        out string exitMessage))
                {
                    EditorUtility.DisplayDialog(
                        "Cannot Turn In Survival Quest Yet",
                        exitMessage,
                        "Return to Expedition");
                    return;
                }
                ShowNotification(
                    new GUIContent(exitMessage), 5d);
            }
            if (!session.idlePauseAcknowledged)
            {
                DeverQuestSessionStore.AcknowledgeIdlePause();
            }

            showFinalization = true;
            turnInStep = QuestTurnInStep.Chronicle;
            closingNotes = string.Empty;
            Repaint();
        }

        private void DrawFinalizationPanel(
            DeverQuestSession session)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                $"Quest Turn-In · Step {(int)turnInStep + 1} of 2",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                session.state == DeverQuestSessionState.Running
                    ? "Focused time continues through Quest Log, Git, and " +
                      "closing notes. It stops only when you claim spoils."
                    : "This Quest was already meditating when turn-in began. " +
                      "Return to the Quest to resume focused time.",
                MessageType.Info);

            if (turnInStep == QuestTurnInStep.Chronicle)
            {
                EditorGUILayout.LabelField(
                    "Review Chronicle",
                    EditorStyles.boldLabel);
                DrawReadOnlyValue("Quest", session.taskName);
                if (session.usesQuestContract)
                {
                    DrawReadOnlyValue(
                        "Contract",
                        session.questContractTitle);
                    DrawReadOnlyValue(
                        "Deliverables",
                        session.questContractDeliverables);
                }
                DrawReadOnlyValue(
                    "Focused",
                    FormatDuration(
                        DeverQuestSessionStore.GetFocusedSeconds()));

                EditorGUILayout.Space(6f);
                EditorGUILayout.LabelField("Final Quest Log Entry");
                commitComment = EditorGUILayout.TextArea(
                    commitComment,
                    GUILayout.MinHeight(54f));
                DrawGitPanel();
                DrawQuestLogReview(session);

                EditorGUILayout.Space(6f);
                EditorGUILayout.LabelField(
                    "Closing Notes",
                    EditorStyles.boldLabel);
                closingNotes = EditorGUILayout.TextArea(
                    closingNotes,
                    GUILayout.MinHeight(72f));

                if (GUILayout.Button(
                        "Review Spoils",
                        GUILayout.Height(32f)))
                {
                    turnInStep = QuestTurnInStep.Rewards;
                }
            }
            else
            {
                DrawRewardPreview(session);
                EditorGUILayout.HelpBox(
                    "Claiming these spoils completes the Quest and writes " +
                    "today's Chronicle.",
                    MessageType.Info);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Back to Chronicle"))
                {
                    turnInStep = QuestTurnInStep.Chronicle;
                }
                if (GUILayout.Button(
                        "Claim Spoils and Complete Quest",
                        GUILayout.Height(34f)))
                {
                    FinalizeSession();
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Return to Quest"))
            {
                CancelTurnIn();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawQuestLogReview(
            DeverQuestSession session)
        {
            EditorGUILayout.LabelField(
                "Review Quest Log",
                EditorStyles.boldLabel);
            if (session.commitEntries == null ||
                session.commitEntries.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No Quest Log entries were recorded.",
                    MessageType.Info);
                return;
            }

            foreach (DeverQuestCommitEntry entry
                     in session.commitEntries)
            {
                string gitLabel =
                    entry.entryType == "Git Commit"
                        ? $"Git Commit {entry.commitHash}"
                        : entry.entryType == "Git Push"
                            ? $"Git Push {entry.commitHash}"
                        : entry.entryType == "Linked Commit Note"
                            ? $"Linked to {entry.commitHash}"
                    : string.IsNullOrWhiteSpace(entry.entryType)
                        ? "Legacy Entry"
                        : entry.entryType;
                EditorGUILayout.LabelField(
                    gitLabel,
                    $"{entry.comment} · {entry.branch}",
                    wrappedLabelStyle);
            }
        }

        private static void DrawRewardPreview(
            DeverQuestSession session)
        {
            EditorGUILayout.LabelField(
                "Projected Rewards",
                EditorStyles.boldLabel);
            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            if (!profile.rewardsEnabled)
            {
                EditorGUILayout.LabelField("Rewards are disabled.");
                return;
            }

            double totalWorkSeconds =
                session.usesQuestProfile
                    ? DeverQuestSessionStore.GetFocusedSeconds()
                    : DeverQuestRewardService.Wallet
                          .unrewardedWorkSeconds +
                      DeverQuestSessionStore.GetFocusedSeconds();
            int workBlockMinutes = session.usesQuestProfile
                ? Math.Max(1, session.questWorkBlockMinutes)
                : profile.rewardWorkBlockMinutes;
            int copperPerBlock = session.usesQuestProfile
                ? session.questCopperPerWorkBlock
                : profile.copperPerWorkBlock;
            int experiencePerBlock = session.usesQuestProfile
                ? session.questExperiencePerWorkBlock
                : profile.experiencePerWorkBlock;
            int baseCopper = session.usesQuestProfile
                ? session.questBaseCopper
                : profile.baseQuestCopper;
            int baseExperience = session.usesQuestProfile
                ? session.questBaseExperience
                : profile.baseQuestExperience;
            double blockSeconds =
                Math.Max(60d, workBlockMinutes * 60d);
            int blocks =
                (int)Math.Floor(totalWorkSeconds / blockSeconds);

            EditorGUILayout.LabelField(
                "Completed Work Blocks",
                blocks.ToString());
            long copper =
                baseCopper +
                blocks * (long)copperPerBlock;
            long experience =
                baseExperience +
                blocks * (long)experiencePerBlock;
            if (session.usesQuestProfile)
            {
                EditorGUILayout.LabelField(
                    "Quest Profile",
                    session.questProfileName);
            }
            EditorGUILayout.LabelField(
                "Projected Coin",
                $"+{DeverQuestAdventurerService.FormatCoins(copper)}");
            EditorGUILayout.LabelField(
                "Projected Experience",
                $"+{experience} XP");
            EditorGUILayout.HelpBox(
                "Daily Decree bonus: " +
                $"{DeverQuestAdventurerService.FormatCoins(profile.dailyCopperBonus)} " +
                $"+ {profile.dailyExperienceBonus} XP when the focused-work " +
                "decree is fulfilled.",
                MessageType.Info);
        }

        private void CancelTurnIn()
        {
            showFinalization = false;
            turnInStep = QuestTurnInStep.Chronicle;

            Repaint();
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

            if (session.usesQuestContract)
            {
                DeverQuestContractService.SubmitParticipant(
                    session.questContractId,
                    DeverQuestAdventurerService.Adventurer
                        .characterName);
            }

            WriteTimecard(session);
            if (DeverQuestSettingsStore.Profile
                .notificationSoundsEnabled)
            {
                DeverQuestAudioDirector.PlayCue(
                    DeverQuestAudioCue.QuestComplete);
            }

            newProjectName = session.projectName;
            newCategory = session.category;
            newTaskName = string.Empty;
            newGoal = string.Empty;
            appliedQuestProfileId = string.Empty;
            selectedQuestContract = null;
            appliedQuestContractId = string.Empty;
            ApplySelectedQuestProfile();
            commitComment = string.Empty;
            commitHash = string.Empty;
            closingNotes = string.Empty;
            showFinalization = false;
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
                    DeverQuestIdleMonitor
                        .BeginIntentionalExternalAction();
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

            bool canManageGuild =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);
            if (!canManageGuild)
            {
                EditorGUILayout.HelpBox(
                    "Guild settings are read-only for this account.",
                    MessageType.Info);
            }
            using (new EditorGUI.DisabledScope(!canManageGuild))
            {
            EditorGUI.BeginChangeCheck();
            DrawPolishSetup(profile);
            DrawChronicleIntegritySetup(profile);
            DrawExternalActivitySetup();
            if (EditorGUI.EndChangeCheck())
            {
                profile.Sanitize();
                DeverQuestSettingsStore.Save();
                DeverQuestGuildAccountService.AddAudit(
                    "Guild Settings Updated",
                    profile.developerName,
                    "Appearance and Chronicle settings changed.");
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
                    DeverQuestIdleMonitor
                        .BeginIntentionalExternalAction();
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
                    DeverQuestGuildAccountService.AddAudit(
                        "Profile Reconfiguration",
                        profile.developerName,
                        "First-time setup was reopened.");
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
                        DeverQuestGuildAccountService.AddAudit(
                            "Profile Reset",
                            profile.developerName,
                            "Local DeverQuest settings were reset.");
                        DeverQuestSettingsStore.ResetProfile();
                        Repaint();
                    }
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
            if (!DeverQuestSessionStore.HasActiveSession ||
                (activeWorkspace != DeverQuestWorkspace.Quest &&
                 activeWorkspace !=
                 DeverQuestWorkspace.QuestLog) ||
                EditorApplication.timeSinceStartup <
                nextSessionRepaintTime)
            {
                return;
            }
            nextSessionRepaintTime =
                EditorApplication.timeSinceStartup + 0.25d;
            Repaint();
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
