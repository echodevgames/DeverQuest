//----- DeverQuestProfile.cs START -----

using System;

namespace EchoDevGames.DeverQuest
{
    [Serializable]
    internal sealed class DeverQuestProfile
    {
        public const int CurrentDataVersion = 2;

        public int dataVersion = CurrentDataVersion;
        public bool setupComplete;
        public string developerName = string.Empty;
        public string timecardRootPath = string.Empty;
        public int defaultFocusMinutes = 50;
        public bool idleDetectionEnabled = true;
        public int idleTimeoutMinutes = 5;
        public int idleWarningSeconds = 30;
        public bool countPlayModeAsActivity = true;
        public bool countCompilationAsActivity = true;
        public bool countAssetImportAsActivity = true;
        public bool countBuildsAsActivity = true;

        public void Sanitize()
        {
            if (dataVersion < 2)
            {
                idleWarningSeconds = 30;
                countPlayModeAsActivity = true;
                countCompilationAsActivity = true;
                countAssetImportAsActivity = true;
                countBuildsAsActivity = true;
            }

            developerName = developerName?.Trim() ?? string.Empty;
            timecardRootPath = timecardRootPath?.Trim() ?? string.Empty;
            defaultFocusMinutes = Math.Max(1, defaultFocusMinutes);
            idleTimeoutMinutes = Math.Max(1, idleTimeoutMinutes);
            idleWarningSeconds = Math.Max(0, idleWarningSeconds);
            dataVersion = CurrentDataVersion;
        }
    }
}

//----- DeverQuestProfile.cs END -----
