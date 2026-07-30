//----- DeverQuestAdventurerService.cs START -----

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [Serializable]
    internal sealed class DeverQuestAdventurer
    {
        public int dataVersion = 1;
        public string characterName = string.Empty;
        public string guildName = "Isekai Studios";
        public string guildRank = "Member";
        public string characterClass = "Warrior";
        public int level = 1;
        public long currentExperience;
        public long lifetimeExperience;
        public long copperBalance;
        public long totalCopperEarned;
        public long totalCopperSpent;
        public int strength = 10;
        public int dexterity = 10;
        public int constitution = 10;
        public int intelligence = 10;
        public int wisdom = 10;
        public int charisma = 10;
        public int hitDie = 8;
        public int maximumHitPoints = 8;
        public int currentHitPoints = 8;
        public List<string> proficientSaves =
            new List<string>();
        public List<string> statusEffects =
            new List<string>();
        public List<string> equippedEquipmentIds =
            new List<string>();
        public List<string> knownSpellIds =
            new List<string>();

        public void Sanitize()
        {
            if (dataVersion < 2)
            {
                DeverQuestAdventurerService.ApplyClassFoundation(
                    this, characterClass, false);
            }
            characterName = characterName?.Trim() ?? string.Empty;
            guildName = guildName?.Trim() ?? string.Empty;
            guildRank = guildRank?.Trim() ?? string.Empty;
            characterClass = characterClass?.Trim() ?? string.Empty;
            level = Math.Max(1, level);
            currentExperience = Math.Max(0L, currentExperience);
            lifetimeExperience = Math.Max(0L, lifetimeExperience);
            copperBalance = Math.Max(0L, copperBalance);
            totalCopperEarned = Math.Max(0L, totalCopperEarned);
            totalCopperSpent = Math.Max(0L, totalCopperSpent);
            strength = ClampAbility(strength);
            dexterity = ClampAbility(dexterity);
            constitution = ClampAbility(constitution);
            intelligence = ClampAbility(intelligence);
            wisdom = ClampAbility(wisdom);
            charisma = ClampAbility(charisma);
            hitDie = Math.Max(4, hitDie);
            maximumHitPoints = Math.Max(1, maximumHitPoints);
            currentHitPoints =
                Math.Min(maximumHitPoints,
                    Math.Max(0, currentHitPoints));
            proficientSaves = proficientSaves ??
                               new List<string>();
            statusEffects = statusEffects ?? new List<string>();
            equippedEquipmentIds = equippedEquipmentIds ??
                                   new List<string>();
            knownSpellIds = knownSpellIds ?? new List<string>();
            dataVersion = 2;
        }

        private static int ClampAbility(int value)
        {
            return Math.Min(30, Math.Max(1, value));
        }
    }

    internal readonly struct DeverQuestProgressionResult
    {
        public readonly int StartingLevel;
        public readonly int EndingLevel;
        public readonly long ExperienceAwarded;

        public bool LeveledUp => EndingLevel > StartingLevel;

        public DeverQuestProgressionResult(
            int startingLevel,
            int endingLevel,
            long experienceAwarded)
        {
            StartingLevel = startingLevel;
            EndingLevel = endingLevel;
            ExperienceAwarded = experienceAwarded;
        }
    }

    [InitializeOnLoad]
    internal static class DeverQuestAdventurerService
    {
        private const string AdventurerKey =
            "EchoDevGames.DeverQuest.Adventurer.v1";

        private static DeverQuestAdventurer adventurer;

        public static readonly string[] Classes =
        {
            "Warrior", "Paladin", "Ranger", "Rogue", "Cleric", "Druid",
            "Wizard", "Sorcerer", "Necromancer", "Bard", "Monk", "Barbarian"
        };

        public static readonly string[] GuildRanks =
        {
            "Member", "Project Leader", "Boss", "CEO"
        };

        static DeverQuestAdventurerService()
        {
            Load();
        }

        public static DeverQuestAdventurer Adventurer
        {
            get
            {
                if (adventurer == null)
                {
                    Load();
                }

                return adventurer;
            }
        }

        public static long ExperienceForNextLevel(int level)
        {
            return Math.Max(100L, level * 100L);
        }

        public static void ApplyClassFoundation(
            DeverQuestAdventurer target,
            string characterClass,
            bool resetVitals)
        {
            if (target == null)
            {
                return;
            }
            target.characterClass =
                string.IsNullOrWhiteSpace(characterClass)
                    ? "Warrior"
                    : characterClass;
            int[] scores;
            string[] saves;
            switch (target.characterClass)
            {
                case "Necromancer":
                case "Wizard":
                    scores = new[] { 8, 14, 14, 16, 12, 10 };
                    target.hitDie = 6;
                    saves = new[] { "Intelligence", "Wisdom" };
                    break;
                case "Sorcerer":
                case "Bard":
                    scores = new[] { 8, 14, 14, 10, 12, 16 };
                    target.hitDie = 6;
                    saves = new[] { "Constitution", "Charisma" };
                    break;
                case "Rogue":
                case "Ranger":
                    scores = new[] { 10, 16, 14, 12, 14, 8 };
                    target.hitDie = 8;
                    saves = new[] { "Dexterity", "Intelligence" };
                    break;
                case "Cleric":
                case "Druid":
                    scores = new[] { 10, 12, 14, 8, 16, 10 };
                    target.hitDie = 8;
                    saves = new[] { "Wisdom", "Charisma" };
                    break;
                case "Paladin":
                    scores = new[] { 16, 10, 14, 8, 10, 14 };
                    target.hitDie = 10;
                    saves = new[] { "Wisdom", "Charisma" };
                    break;
                case "Monk":
                    scores = new[] { 12, 16, 14, 8, 16, 8 };
                    target.hitDie = 8;
                    saves = new[] { "Strength", "Dexterity" };
                    break;
                default:
                    scores = new[] { 16, 12, 16, 8, 10, 10 };
                    target.hitDie = 10;
                    saves = new[] { "Strength", "Constitution" };
                    break;
            }
            target.strength = scores[0];
            target.dexterity = scores[1];
            target.constitution = scores[2];
            target.intelligence = scores[3];
            target.wisdom = scores[4];
            target.charisma = scores[5];
            target.proficientSaves =
                new List<string>(saves);
            int constitutionModifier =
                DeverQuestRulesService.AbilityModifier(
                    target.constitution);
            int calculatedMaximum =
                Math.Max(1,
                    target.hitDie + constitutionModifier +
                    Math.Max(0, target.level - 1) *
                    (target.hitDie / 2 + 1 +
                     constitutionModifier));
            target.maximumHitPoints = calculatedMaximum;
            if (resetVitals || target.currentHitPoints <= 0)
            {
                target.currentHitPoints = calculatedMaximum;
            }
        }

        public static DeverQuestProgressionResult Award(
            long copper,
            long experience)
        {
            DeverQuestAdventurer target = Adventurer;
            int startingLevel = target.level;
            copper = Math.Max(0L, copper);
            experience = Math.Max(0L, experience);

            target.copperBalance += copper;
            target.totalCopperEarned += copper;
            target.currentExperience += experience;
            target.lifetimeExperience += experience;

            while (target.currentExperience >=
                   ExperienceForNextLevel(target.level))
            {
                target.currentExperience -=
                    ExperienceForNextLevel(target.level);
                target.level++;
                int constitutionModifier =
                    DeverQuestRulesService.AbilityModifier(
                        target.constitution);
                int hitPointGain = Math.Max(
                    1,
                    target.hitDie / 2 + 1 +
                    constitutionModifier);
                target.maximumHitPoints += hitPointGain;
                target.currentHitPoints += hitPointGain;
            }

            Save();
            return new DeverQuestProgressionResult(
                startingLevel,
                target.level,
                experience);
        }

        public static bool SpendCopper(long copper, out string error)
        {
            error = string.Empty;
            copper = Math.Max(0L, copper);
            if (copper <= 0L)
            {
                error = "Enter a coin cost greater than zero.";
                return false;
            }

            if (copper > Adventurer.copperBalance)
            {
                error =
                    $"Only {FormatCoins(Adventurer.copperBalance)} is available.";
                return false;
            }

            Adventurer.copperBalance -= copper;
            Adventurer.totalCopperSpent += copper;
            Save();
            return true;
        }

        public static void MigrateLegacyCopper(long copper)
        {
            copper = Math.Max(0L, copper);
            if (copper <= 0L)
            {
                return;
            }

            Adventurer.copperBalance += copper;
            Adventurer.totalCopperEarned += copper;
            Save();
        }

        public static string FormatCoins(long totalCopper)
        {
            totalCopper = Math.Max(0L, totalCopper);
            long platinum = totalCopper / 1000000L;
            long remainder = totalCopper % 1000000L;
            long gold = remainder / 10000L;
            remainder %= 10000L;
            long silver = remainder / 100L;
            long copper = remainder % 100L;

            return $"{platinum}p {gold}g {silver}s {copper}c";
        }

        public static void Save()
        {
            Adventurer.Sanitize();
            EditorPrefs.SetString(
                AdventurerKey,
                JsonUtility.ToJson(Adventurer));
            DeverQuestGuildAccountService.SyncFromAdventurer();
        }

        private static void Load()
        {
            string json = EditorPrefs.GetString(AdventurerKey, string.Empty);
            try
            {
                adventurer = string.IsNullOrWhiteSpace(json)
                    ? new DeverQuestAdventurer()
                    : JsonUtility.FromJson<DeverQuestAdventurer>(json) ??
                      new DeverQuestAdventurer();
                adventurer.Sanitize();
            }
            catch
            {
                Debug.LogWarning(
                    "[DeverQuest] Adventurer profile could not be loaded.");
                adventurer = new DeverQuestAdventurer();
            }
        }
    }
}

//----- DeverQuestAdventurerService.cs END -----
