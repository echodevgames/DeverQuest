//----- DeverQuestReleaseReadinessService.cs START -----

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestReadinessSeverity
    {
        Pass = 0,
        Advisory = 1,
        Blocker = 2
    }

    internal sealed class DeverQuestReadinessFinding
    {
        public DeverQuestReadinessSeverity severity;
        public string title = string.Empty;
        public string detail = string.Empty;
    }

    internal sealed class DeverQuestReadinessReport
    {
        public readonly List<DeverQuestReadinessFinding> findings =
            new List<DeverQuestReadinessFinding>();

        public int PassCount => findings.Count(
            finding =>
                finding.severity == DeverQuestReadinessSeverity.Pass);

        public int AdvisoryCount => findings.Count(
            finding =>
                finding.severity == DeverQuestReadinessSeverity.Advisory);

        public int BlockerCount => findings.Count(
            finding =>
                finding.severity == DeverQuestReadinessSeverity.Blocker);

        public bool ReadyForRegression => BlockerCount == 0;

        public string Summary =>
            $"{PassCount} passed, {AdvisoryCount} advisories, " +
            $"{BlockerCount} blockers.";

        public string ToConsoleText()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(
                "[DeverQuest 0.30.6 Beta 1] Release Readiness Report");
            builder.AppendLine(Summary);

            foreach (DeverQuestReadinessFinding finding in findings)
            {
                builder.Append('[');
                builder.Append(finding.severity.ToString().ToUpperInvariant());
                builder.Append("] ");
                builder.AppendLine(finding.title);
                builder.AppendLine("  " + finding.detail);
            }

            return builder.ToString().TrimEnd();
        }
    }

    internal static class DeverQuestReleaseReadinessService
    {
        private const string ExpectedPackageVersion = "0.30.6";

        [MenuItem(
            "Tools/DeverQuest/Run Release Readiness Check",
            false,
            40)]
        private static void RunFromMenu()
        {
            DeverQuestReadinessReport report = Run();
            string text = report.ToConsoleText();

            if (report.BlockerCount > 0)
            {
                Debug.LogError(text);
            }
            else if (report.AdvisoryCount > 0)
            {
                Debug.LogWarning(text);
            }
            else
            {
                Debug.Log(text);
            }

            EditorUtility.DisplayDialog(
                report.ReadyForRegression
                    ? "DeverQuest Ready for Regression"
                    : "DeverQuest Release Blocked",
                report.Summary +
                "\n\nThe full report was written to the Console.",
                "Close");
        }

        public static DeverQuestReadinessReport Run()
        {
            DeverQuestReadinessReport report =
                new DeverQuestReadinessReport();

            CheckPackageVersion(report);
            CheckUnityVersion(report);
            CheckRepositoryHygiene(report);
            CheckProfile(report);
            CheckTimecardStorage(report);
            CheckChroniclePolicy(report);
            CheckSharedGuild(report);
            CheckAudioTransport(report);
            CheckIdentityCatalog(report);
            CheckSpoilsSnapshots(report);
            CheckSessionState(report);

            return report;
        }

        private static void CheckPackageVersion(
            DeverQuestReadinessReport report)
        {
            UnityEditor.PackageManager.PackageInfo package =
                UnityEditor.PackageManager.PackageInfo.FindForAssembly(
                    typeof(DeverQuestReleaseReadinessService).Assembly);

            if (package == null)
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Advisory,
                    "Package version",
                    "Unity could not resolve Package Manager metadata for " +
                    "the loaded DeverQuest assembly.");
                return;
            }

            bool correctVersion = string.Equals(
                package.version,
                ExpectedPackageVersion,
                StringComparison.Ordinal);

            Add(
                report,
                correctVersion
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Blocker,
                "Package version",
                correctVersion
                    ? $"Package Manager reports {ExpectedPackageVersion}."
                    : $"Package Manager reports {package.version}; expected " +
                      $"{ExpectedPackageVersion}.");
        }

        private static void CheckUnityVersion(
            DeverQuestReadinessReport report)
        {
            bool supported = TryReadUnityVersion(
                Application.unityVersion,
                out int major,
                out int minor) &&
                (major > 2022 ||
                 (major == 2022 && minor >= 3));

            Add(
                report,
                supported
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Blocker,
                "Unity editor version",
                supported
                    ? $"Unity {Application.unityVersion} meets the 2022.3 " +
                      "minimum."
                    : $"Unity {Application.unityVersion} is below or could " +
                      "not be validated against the 2022.3 minimum.");
        }

        private static void CheckRepositoryHygiene(
            DeverQuestReadinessReport report)
        {
            DirectoryInfo projectDirectory =
                Directory.GetParent(Application.dataPath);
            if (projectDirectory == null)
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Advisory,
                    "Repository hygiene",
                    "Unity could not resolve the project root directory.");
                return;
            }

            string rootPath = projectDirectory.FullName;
            string normalizedPath = rootPath
                .Replace("_", " ")
                .Replace("-", " ")
                .ToLowerInvariant();
            string prohibitedLegacyName = new string(
                new[]
                {
                    'c', 'o', 'd', 'e', ' ',
                    'n', 'a', 'z', 'i'
                });
            bool legacyNameFound =
                normalizedPath.Contains(prohibitedLegacyName) ||
                normalizedPath.Contains(
                    prohibitedLegacyName.Replace(" ", string.Empty));
            Add(
                report,
                legacyNameFound
                    ? DeverQuestReadinessSeverity.Blocker
                    : DeverQuestReadinessSeverity.Pass,
                "Legacy repository naming",
                legacyNameFound
                    ? "The Unity project path still contains a prohibited " +
                      "legacy name. Rename the folder before Beta distribution."
                    : "No prohibited legacy name was found in the Unity " +
                      "project path.");

            string[] expectedFiles =
            {
                "README.md",
                "CREDITS.md",
                "THIRD_PARTY_NOTICES.md"
            };
            List<string> missingFiles = expectedFiles
                .Where(file => !File.Exists(Path.Combine(rootPath, file)))
                .ToList();
            bool documentationExists =
                Directory.Exists(Path.Combine(rootPath, "Documentation"));
            if (!documentationExists)
            {
                missingFiles.Add("Documentation/");
            }

            Add(
                report,
                missingFiles.Count == 0
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Repository documentation",
                missingFiles.Count == 0
                    ? "README, credits, third-party notices, and the " +
                      "Documentation folder are present at the project root."
                    : "Add these release files to the project root: " +
                      string.Join(", ", missingFiles) + ".");
        }

        private static void CheckProfile(
            DeverQuestReadinessReport report)
        {
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;
            bool configured =
                profile != null &&
                profile.setupComplete &&
                !string.IsNullOrWhiteSpace(profile.developerName);

            Add(
                report,
                configured
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Blocker,
                "Developer profile",
                configured
                    ? $"Profile is configured for {profile.developerName}."
                    : "Complete DeverQuest setup and provide a developer " +
                      "name before release regression.");
        }

        private static void CheckTimecardStorage(
            DeverQuestReadinessReport report)
        {
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;
            if (profile == null ||
                string.IsNullOrWhiteSpace(profile.timecardRootPath) ||
                string.IsNullOrWhiteSpace(profile.developerName))
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Blocker,
                    "Timecard storage",
                    "The timecard root or developer name is not configured.");
                return;
            }

            string developerFolder =
                DeverQuestPathUtility.GetDeveloperFolder(
                    profile.timecardRootPath,
                    profile.developerName);

            if (!DeverQuestPathUtility.TryCreateDirectory(
                    developerFolder,
                    out string createError))
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Blocker,
                    "Timecard storage",
                    "The developer folder could not be created: " +
                    createError);
                return;
            }

            string probePath = Path.Combine(
                developerFolder,
                ".deverquest_write_probe");

            try
            {
                File.WriteAllText(
                    probePath,
                    DateTime.UtcNow.ToString("O"));
                File.Delete(probePath);

                Add(
                    report,
                    DeverQuestReadinessSeverity.Pass,
                    "Timecard storage",
                    "The developer folder exists and accepted a write/delete " +
                    "probe.");
            }
            catch (Exception exception)
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Blocker,
                    "Timecard storage",
                    "The developer folder is not writable: " +
                    exception.Message);
            }
        }

        private static void CheckChroniclePolicy(
            DeverQuestReadinessReport report)
        {
            bool enabled =
                DeverQuestSettingsStore.Profile.chronicleIntegrityEnabled;

            Add(
                report,
                enabled
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Chronicle integrity",
                enabled
                    ? "Chronicle integrity verification is enabled."
                    : "Chronicle integrity verification is disabled. Release " +
                      "testing is safer with it enabled.");
        }

        private static void CheckSharedGuild(
            DeverQuestReadinessReport report)
        {
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;
            if (!profile.sharedGuildEnabled)
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Pass,
                    "Shared Guild repository",
                    "Shared Guild publishing is disabled for this local " +
                    "profile.");
                return;
            }

            bool pathConfigured =
                !string.IsNullOrWhiteSpace(
                    profile.sharedGuildRepositoryPath) &&
                Directory.Exists(profile.sharedGuildRepositoryPath);

            Add(
                report,
                pathConfigured
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Blocker,
                "Shared Guild repository",
                pathConfigured
                    ? "The configured shared repository exists."
                    : "Shared Guild publishing is enabled, but its repository " +
                      "folder is missing or unavailable.");
        }

        private static void CheckAudioTransport(
            DeverQuestReadinessReport report)
        {
            Add(
                report,
                DeverQuestEditorAudioBridge.IsAvailable
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Editor audio transport",
                DeverQuestEditorAudioBridge.IsAvailable
                    ? "The two-channel logical preview bridge is available."
                    : "Unity did not expose editor preview playback. Timers " +
                      "still work, but music and warning cues will be silent.");

            Add(
                report,
                DeverQuestEditorAudioBridge.PlaybackStatusSupported
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Playlist completion detection",
                DeverQuestEditorAudioBridge.PlaybackStatusSupported
                    ? "Track completion status is available."
                    : "Automatic playlist advancement is unavailable in this " +
                      "Unity editor version.");
        }

        private static void CheckIdentityCatalog(
            DeverQuestReadinessReport report)
        {
            DeverQuestIdentityCatalog catalog =
                DeverQuestIdentityCatalogService.ActiveCatalog;
            bool ready = catalog != null &&
                         catalog.defaultAncestry != null &&
                         catalog.defaultClass != null &&
                         (catalog.ancestries?.Count ?? 0) > 0 &&
                         (catalog.classes?.Count ?? 0) > 0;
            Add(
                report,
                ready
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Starter Identity Catalog",
                ready
                    ? $"{catalog.displayName} is active with valid defaults."
                    : "No complete active Identity Catalog was found. " +
                      "Open Guild Hall > Campaign Content Scaffolding, then " +
                      "generate the original starter catalog before testing " +
                      "new-character onboarding.");
        }

        private static void CheckSpoilsSnapshots(
            DeverQuestReadinessReport report)
        {
            int mismatchCount = 0;
            foreach (string guid in AssetDatabase.FindAssets(
                         "t:DeverQuestQuestContract"))
            {
                DeverQuestQuestContract contract =
                    AssetDatabase.LoadAssetAtPath<
                        DeverQuestQuestContract>(
                        AssetDatabase.GUIDToAssetPath(guid));
                if (contract != null &&
                    contract.questProfile != null &&
                    !contract.SpoilsMatchLinkedProfile() &&
                    contract.CanRefreshSpoilsFromProfile())
                {
                    mismatchCount++;
                }
            }

            Add(
                report,
                mismatchCount == 0
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Quest Contract Spoils",
                mismatchCount == 0
                    ? "All refreshable Contract Spoils snapshots match their " +
                      "linked Quest Profiles."
                    : $"{mismatchCount} refreshable Contract(s) have Spoils " +
                      "that differ from their linked profiles. Select each " +
                      "Contract in DeverQuest to refresh it before testing.");
        }

        private static void CheckSessionState(
            DeverQuestReadinessReport report)
        {
            Add(
                report,
                DeverQuestSessionStore.HasActiveSession
                    ? DeverQuestReadinessSeverity.Advisory
                    : DeverQuestReadinessSeverity.Pass,
                "Active Quest state",
                DeverQuestSessionStore.HasActiveSession
                    ? "A Quest is active. Complete or abandon it before " +
                      "migration and clean-install regression."
                    : "No active Quest will interfere with clean regression.");
        }

        private static bool TryReadUnityVersion(
            string version,
            out int major,
            out int minor)
        {
            major = 0;
            minor = 0;
            if (string.IsNullOrWhiteSpace(version))
            {
                return false;
            }

            string[] parts = version.Split('.');
            return parts.Length >= 2 &&
                   int.TryParse(parts[0], out major) &&
                   int.TryParse(parts[1], out minor);
        }

        private static void Add(
            DeverQuestReadinessReport report,
            DeverQuestReadinessSeverity severity,
            string title,
            string detail)
        {
            report.findings.Add(
                new DeverQuestReadinessFinding
                {
                    severity = severity,
                    title = title,
                    detail = detail
                });
        }
    }
}

//----- DeverQuestReleaseReadinessService.cs END -----
