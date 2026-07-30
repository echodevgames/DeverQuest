using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestGuildPermission
    {
        WorkInput = 0,
        ManageContracts = 1,
        ReviewCorrections = 2,
        ManageProject = 3,
        ManageGuild = 4,
        DeleteRecords = 5,
        DeleteProgram = 6
    }

    [Serializable]
    internal sealed class DeverQuestGuildAccount
    {
        public string accountId = string.Empty;
        public string developerName = string.Empty;
        public string characterName = string.Empty;
        public string guildName = string.Empty;
        public string characterClass = "Warrior";
        public string guildRank = "Member";
        public List<string> assignedProjects = new List<string>();
        public string passwordSalt = string.Empty;
        public string passwordHash = string.Empty;
        public int passwordIterations = 100000;
        public int level = 1;
        public long currentExperience;
        public long lifetimeExperience;
        public long copperBalance;
        public long totalCopperEarned;
        public long totalCopperSpent;
        public bool disabled;
    }

    [Serializable]
    internal sealed class DeverQuestGuildAccountCollection
    {
        public int dataVersion = 1;
        public List<DeverQuestGuildAccount> accounts =
            new List<DeverQuestGuildAccount>();
    }

    [Serializable]
    internal sealed class DeverQuestGuildAuditEntry
    {
        public string createdUtc = string.Empty;
        public string actorAccountId = string.Empty;
        public string actorName = string.Empty;
        public string action = string.Empty;
        public string target = string.Empty;
        public string detail = string.Empty;
    }

    [Serializable]
    internal sealed class DeverQuestGuildAuditLog
    {
        public List<DeverQuestGuildAuditEntry> entries =
            new List<DeverQuestGuildAuditEntry>();
    }

    [InitializeOnLoad]
    internal static class DeverQuestGuildAccountService
    {
        private const string AccountsKey =
            "EchoDevGames.DeverQuest.GuildAccounts.v1";
        private const string CurrentAccountKey =
            "EchoDevGames.DeverQuest.CurrentGuildAccount.v1";
        private const string AuditKey =
            "EchoDevGames.DeverQuest.GuildAudit.v1";
        private const string AuthenticatedKey =
            "EchoDevGames.DeverQuest.GuildAuthenticated.v1";

        private static DeverQuestGuildAccountCollection collection;
        private static DeverQuestGuildAuditLog audit;

        static DeverQuestGuildAccountService()
        {
            Load();
            EnsureLegacyFounder();
        }

        public static IReadOnlyList<DeverQuestGuildAccount> Accounts =>
            collection.accounts;

        public static IReadOnlyList<DeverQuestGuildAuditEntry> AuditEntries =>
            audit.entries;

        public static DeverQuestGuildAccount CurrentAccount
        {
            get
            {
                string id = EditorPrefs.GetString(
                    CurrentAccountKey, string.Empty);
                return collection.accounts.FirstOrDefault(
                    item => item.accountId == id);
            }
        }

        public static bool IsAuthenticated =>
            CurrentAccount != null &&
            !CurrentAccount.disabled &&
            (SessionState.GetBool(AuthenticatedKey, false) ||
             string.IsNullOrWhiteSpace(CurrentAccount.passwordHash));

        public static bool RequiresPasscodeSetup =>
            CurrentAccount != null &&
            string.IsNullOrWhiteSpace(CurrentAccount.passwordHash);

        public static bool HasPermission(
            DeverQuestGuildPermission permission,
            string projectName = "")
        {
            if (!IsAuthenticated)
            {
                return false;
            }
            DeverQuestGuildAccount account = CurrentAccount;
            if (permission == DeverQuestGuildPermission.WorkInput)
            {
                return true;
            }
            if (account.guildRank == "CEO")
            {
                return true;
            }
            if (account.guildRank == "Boss")
            {
                return permission !=
                       DeverQuestGuildPermission.DeleteRecords &&
                       permission !=
                       DeverQuestGuildPermission.DeleteProgram;
            }
            if (account.guildRank != "Project Leader")
            {
                return false;
            }
            bool projectAssigned =
                account.assignedProjects.Any(
                    item => string.Equals(
                        item, projectName,
                        StringComparison.OrdinalIgnoreCase));
            return projectAssigned &&
                   (permission ==
                    DeverQuestGuildPermission.ManageContracts ||
                    permission ==
                    DeverQuestGuildPermission.ReviewCorrections ||
                    permission ==
                    DeverQuestGuildPermission.ManageProject);
        }

        public static bool Login(
            string developerName,
            string passcode,
            out string error)
        {
            error = string.Empty;
            DeverQuestGuildAccount account =
                collection.accounts.FirstOrDefault(
                    item => string.Equals(
                        item.developerName,
                        developerName?.Trim(),
                        StringComparison.OrdinalIgnoreCase));
            if (account == null || account.disabled)
            {
                error = "Guild account was not found or is disabled.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(account.passwordHash))
            {
                error = "This founding account must first be secured locally.";
                return false;
            }
            if (!VerifyPasscode(account, passcode))
            {
                error = "Passcode was incorrect.";
                AddAudit("Login Failed", account.developerName,
                    "Incorrect local passcode.");
                return false;
            }
            SelectAccount(account);
            SessionState.SetBool(AuthenticatedKey, true);
            AddAudit("Login", account.developerName,
                "Local Guild authentication succeeded.");
            return true;
        }

        public static void Logout()
        {
            SyncFromAdventurer();
            AddAudit("Logout",
                CurrentAccount?.developerName ?? string.Empty,
                "Local Guild session ended.");
            SessionState.SetBool(AuthenticatedKey, false);
        }

        public static bool SecureCurrentAccount(
            string passcode,
            out string error)
        {
            error = ValidatePasscode(passcode);
            if (!string.IsNullOrEmpty(error) || CurrentAccount == null)
            {
                return false;
            }
            SetPasscode(CurrentAccount, passcode);
            Save();
            SessionState.SetBool(AuthenticatedKey, true);
            AddAudit("Founder Secured", CurrentAccount.developerName,
                "The migrated founding account received a local passcode.");
            return true;
        }

        public static void RefreshUnsecuredFounderIdentity(
            string developerName)
        {
            DeverQuestGuildAccount account = CurrentAccount;
            developerName = developerName?.Trim() ?? string.Empty;
            if (account == null ||
                !string.IsNullOrWhiteSpace(account.passwordHash) ||
                string.IsNullOrWhiteSpace(developerName))
            {
                return;
            }
            account.developerName = developerName;
            if (string.IsNullOrWhiteSpace(account.characterName))
            {
                account.characterName = developerName;
            }
            Save();
        }

        public static bool CreateAccount(
            string developerName,
            string characterName,
            string characterClass,
            string rank,
            IEnumerable<string> projects,
            string temporaryPasscode,
            out string error)
        {
            error = string.Empty;
            if (!HasPermission(DeverQuestGuildPermission.ManageGuild))
            {
                error = "Only a Boss or CEO can create Guild accounts.";
                return false;
            }
            developerName = developerName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(developerName))
            {
                error = "Developer name is required.";
                return false;
            }
            if (collection.accounts.Any(item => string.Equals(
                    item.developerName, developerName,
                    StringComparison.OrdinalIgnoreCase)))
            {
                error = "That developer name already has an account.";
                return false;
            }
            error = ValidatePasscode(temporaryPasscode);
            if (!string.IsNullOrEmpty(error))
            {
                return false;
            }
            DeverQuestGuildAccount account =
                new DeverQuestGuildAccount
                {
                    accountId = Guid.NewGuid().ToString("N"),
                    developerName = developerName,
                    characterName = characterName?.Trim() ?? developerName,
                    guildName = DeverQuestAdventurerService.Adventurer.guildName,
                    characterClass = characterClass,
                    guildRank = rank,
                    assignedProjects = projects?
                        .Select(item => item.Trim())
                        .Where(item => !string.IsNullOrWhiteSpace(item))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList() ?? new List<string>()
                };
            SetPasscode(account, temporaryPasscode);
            collection.accounts.Add(account);
            Save();
            AddAudit("Account Created", account.developerName,
                $"{account.guildRank}; projects: " +
                string.Join(", ", account.assignedProjects));
            return true;
        }

        public static void SyncFromAdventurer()
        {
            DeverQuestGuildAccount account = CurrentAccount;
            if (account == null)
            {
                return;
            }
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            account.characterName = adventurer.characterName;
            account.guildName = adventurer.guildName;
            account.characterClass = adventurer.characterClass;
            account.guildRank = adventurer.guildRank;
            account.level = adventurer.level;
            account.currentExperience = adventurer.currentExperience;
            account.lifetimeExperience = adventurer.lifetimeExperience;
            account.copperBalance = adventurer.copperBalance;
            account.totalCopperEarned = adventurer.totalCopperEarned;
            account.totalCopperSpent = adventurer.totalCopperSpent;
            Save();
        }

        public static void AddAudit(
            string action,
            string target,
            string detail)
        {
            DeverQuestGuildAccount actor = CurrentAccount;
            audit.entries.Insert(0, new DeverQuestGuildAuditEntry
            {
                createdUtc = DateTime.UtcNow.ToString("O"),
                actorAccountId = actor?.accountId ?? string.Empty,
                actorName = actor?.developerName ?? "System",
                action = action ?? string.Empty,
                target = target ?? string.Empty,
                detail = detail ?? string.Empty
            });
            if (audit.entries.Count > 500)
            {
                audit.entries.RemoveRange(500, audit.entries.Count - 500);
            }
            EditorPrefs.SetString(AuditKey, JsonUtility.ToJson(audit));
        }

        private static void EnsureLegacyFounder()
        {
            if (collection.accounts.Count > 0)
            {
                return;
            }
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;
            DeverQuestAdventurer old =
                DeverQuestAdventurerService.Adventurer;
            DeverQuestGuildAccount founder =
                new DeverQuestGuildAccount
                {
                    accountId = Guid.NewGuid().ToString("N"),
                    developerName =
                        string.IsNullOrWhiteSpace(profile.developerName)
                            ? "Founder"
                            : profile.developerName,
                    characterName = old.characterName,
                    guildName = old.guildName,
                    characterClass = old.characterClass,
                    guildRank = "CEO",
                    level = old.level,
                    currentExperience = old.currentExperience,
                    lifetimeExperience = old.lifetimeExperience,
                    copperBalance = old.copperBalance,
                    totalCopperEarned = old.totalCopperEarned,
                    totalCopperSpent = old.totalCopperSpent
                };
            collection.accounts.Add(founder);
            EditorPrefs.SetString(CurrentAccountKey, founder.accountId);
            Save();
            AddAudit("Legacy Migration", founder.developerName,
                "Existing Adventurer migrated as the founding CEO.");
        }

        private static void SelectAccount(DeverQuestGuildAccount account)
        {
            EditorPrefs.SetString(CurrentAccountKey, account.accountId);
            DeverQuestSettingsStore.Profile.developerName =
                account.developerName;
            DeverQuestSettingsStore.Save();
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            adventurer.characterName = account.characterName;
            adventurer.guildName = account.guildName;
            adventurer.characterClass = account.characterClass;
            adventurer.guildRank = account.guildRank;
            adventurer.level = account.level;
            adventurer.currentExperience = account.currentExperience;
            adventurer.lifetimeExperience = account.lifetimeExperience;
            adventurer.copperBalance = account.copperBalance;
            adventurer.totalCopperEarned = account.totalCopperEarned;
            adventurer.totalCopperSpent = account.totalCopperSpent;
            DeverQuestAdventurerService.Save();
        }

        private static void SetPasscode(
            DeverQuestGuildAccount account,
            string passcode)
        {
            byte[] salt = new byte[16];
            using (RandomNumberGenerator generator =
                   RandomNumberGenerator.Create())
            {
                generator.GetBytes(salt);
            }
            account.passwordSalt = Convert.ToBase64String(salt);
            account.passwordHash = Convert.ToBase64String(
                Derive(passcode, salt, account.passwordIterations));
        }

        private static bool VerifyPasscode(
            DeverQuestGuildAccount account,
            string passcode)
        {
            byte[] expected = Convert.FromBase64String(
                account.passwordHash);
            byte[] actual = Derive(
                passcode,
                Convert.FromBase64String(account.passwordSalt),
                account.passwordIterations);
            int difference = expected.Length ^ actual.Length;
            for (int index = 0;
                 index < Math.Min(expected.Length, actual.Length);
                 index++)
            {
                difference |= expected[index] ^ actual[index];
            }
            return difference == 0;
        }

        private static byte[] Derive(
            string passcode,
            byte[] salt,
            int iterations)
        {
            using (Rfc2898DeriveBytes derive =
                   new Rfc2898DeriveBytes(
                       passcode ?? string.Empty,
                       salt,
                       Math.Max(10000, iterations)))
            {
                return derive.GetBytes(32);
            }
        }

        private static string ValidatePasscode(string passcode)
        {
            return string.IsNullOrWhiteSpace(passcode) ||
                   passcode.Length < 6
                ? "Passcode must contain at least six characters."
                : string.Empty;
        }

        private static void Load()
        {
            try
            {
                collection =
                    JsonUtility.FromJson<DeverQuestGuildAccountCollection>(
                        EditorPrefs.GetString(AccountsKey, string.Empty)) ??
                    new DeverQuestGuildAccountCollection();
                audit = JsonUtility.FromJson<DeverQuestGuildAuditLog>(
                            EditorPrefs.GetString(AuditKey, string.Empty)) ??
                        new DeverQuestGuildAuditLog();
            }
            catch
            {
                collection = new DeverQuestGuildAccountCollection();
                audit = new DeverQuestGuildAuditLog();
            }
        }

        private static void Save()
        {
            EditorPrefs.SetString(
                AccountsKey, JsonUtility.ToJson(collection));
        }
    }
}
