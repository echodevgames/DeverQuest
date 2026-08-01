#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

/// <summary>
/// One-time Beta utility for clearing DeverQuest data stored in Unity EditorPrefs.
///
/// Place this file anywhere under Assets/Editor, let Unity compile, then run:
/// Tools > DeverQuest > QA > Clear All Local DeverQuest Data
///
/// This does not delete timecard folders, shared Guild repository files,
/// ScriptableObject assets, audio clips, or repository documentation.
/// </summary>
public static class DeverQuestFreshStartReset
{
    private static readonly string[] EditorPreferenceKeys =
    {
        "EchoDevGames.DeverQuest.Profile.v1",

        "EchoDevGames.DeverQuest.GuildAccounts.v1",
        "EchoDevGames.DeverQuest.CurrentGuildAccount.v1",
        "EchoDevGames.DeverQuest.GuildAudit.v1",

        "EchoDevGames.DeverQuest.Adventurer.v1",
        "EchoDevGames.DeverQuest.RewardWallet.v1",
        "EchoDevGames.DeverQuest.TradeLedger.v1",
        "EchoDevGames.DeverQuest.GuildShopLedger.v1",

        "EchoDevGames.DeverQuest.ActiveSession.v1",
        "EchoDevGames.DeverQuest.LastCompletedSession.v1",

        "EchoDevGames.DeverQuest.Playlist.Guid",
        "EchoDevGames.DeverQuest.Playlist.TrackIndex",

        "EchoDevGames.DeverQuest.Audio.WarningProfile",
        "EchoDevGames.DeverQuest.Audio.AmbienceProfile",
        "EchoDevGames.DeverQuest.Audio.AmbienceIndex",

        "EchoDevGames.DeverQuest.ExternalActivity.Profile",

        "EchoDevGames.DeverQuest.Wellness.SnoozeUntil",
        "EchoDevGames.DeverQuest.Wellness.LunchDate",
        "EchoDevGames.DeverQuest.Wellness.DinnerDate",
        "EchoDevGames.DeverQuest.Wellness.QuietDate",

        "EchoDevGames.DeverQuest.Git.ObservedRepository",
        "EchoDevGames.DeverQuest.Git.ObservedHead"
    };

    private const string AuthenticationSessionKey =
        "EchoDevGames.DeverQuest.GuildAuthenticated.v1";

    [MenuItem("Tools/DeverQuest/QA/Clear All Local DeverQuest Data")]
    private static void ClearAllLocalData()
    {
        bool confirmed = EditorUtility.DisplayDialog(
            "Clear All Local DeverQuest Data?",
            "This permanently deletes DeverQuest settings, Guild accounts, " +
            "characters, rewards, ledgers, active-session recovery, selected " +
            "audio profiles, and local testing state stored in Unity EditorPrefs.\n\n" +
            "It does NOT delete timecards, shared Guild repository files, " +
            "ScriptableObject assets, audio files, or documentation.\n\n" +
            "Continue?",
            "Clear Local Data",
            "Cancel");

        if (!confirmed)
        {
            return;
        }

        int deletedCount = 0;

        foreach (string key in EditorPreferenceKeys)
        {
            if (EditorPrefs.HasKey(key))
            {
                EditorPrefs.DeleteKey(key);
                deletedCount++;
            }
        }

        // SessionState is cleared automatically when Unity closes, but this
        // immediately removes the current local authentication state.
        SessionState.SetBool(AuthenticationSessionKey, false);

        Debug.LogWarning(
            $"[DeverQuest Fresh Start] Deleted {deletedCount} persisted " +
            "EditorPrefs entries. Close Unity now, then reopen the project " +
            "before using or reinstalling DeverQuest.");

        EditorUtility.DisplayDialog(
            "Local DeverQuest Data Cleared",
            $"Deleted {deletedCount} persisted entries.\n\n" +
            "Close Unity now. Then clear or replace the external shared Guild " +
            "repository, reopen the project, and reinstall the tarball.\n\n" +
            "Delete this temporary reset script after the restart.",
            "Understood");
    }
}

#endif
