using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    public enum DeverQuestCompanionKind
    {
        BondedBeast = 0,
        Familiar = 1,
        BoundMinion = 2,
        Spirit = 3,
        Construct = 4,
        Mercenary = 5
    }

    public enum DeverQuestCompanionRole
    {
        Striker = 0,
        Guardian = 1,
        Support = 2,
        Controller = 3
    }

    [Serializable]
    public sealed class DeverQuestCompanionState
    {
        public string instanceId = string.Empty;
        public string profileId = string.Empty;
        public string customName = string.Empty;
        public int level = 1;
        public long currentExperience;
        public long lifetimeExperience;
        public int currentHitPoints;
        [Range(0, 100)]
        public int loyalty = 50;
        public bool isActive;
        public bool isFallen;
        public int battles;
        public int victories;
        public string recruitedUtc = string.Empty;

        public void Sanitize()
        {
            if (string.IsNullOrWhiteSpace(instanceId))
            {
                instanceId = Guid.NewGuid().ToString("N");
            }
            profileId = profileId?.Trim() ?? string.Empty;
            customName = customName?.Trim() ?? string.Empty;
            level = Mathf.Max(1, level);
            currentExperience = Math.Max(0L, currentExperience);
            lifetimeExperience = Math.Max(0L, lifetimeExperience);
            currentHitPoints = Mathf.Max(0, currentHitPoints);
            loyalty = Mathf.Clamp(loyalty, 0, 100);
            battles = Mathf.Max(0, battles);
            victories = Mathf.Max(0, victories);
            recruitedUtc = recruitedUtc?.Trim() ?? string.Empty;
        }
    }

    [CreateAssetMenu(
        fileName = "NewCompanionProfile",
        menuName = "DeverQuest/Companions/Companion Profile")]
    public sealed class DeverQuestCompanionProfile : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string companionId = string.Empty;
        public string displayName = "New Companion";
        [TextArea(3, 8)]
        public string lore = string.Empty;
        public DeverQuestCompanionKind kind =
            DeverQuestCompanionKind.BondedBeast;
        public DeverQuestCompanionRole role =
            DeverQuestCompanionRole.Striker;
        public DeverQuestCreatureType creatureType =
            DeverQuestCreatureType.Beast;
        public int minimumAdventurerLevel = 1;
        public bool requiresCompanionClass = true;
        public List<string> allowedClassIds = new List<string>();
        public List<string> allowedClassNames = new List<string>();
        public int maximumHitPoints = 8;
        public int hitPointsPerLevel = 2;
        public int armorClass = 11;
        public int attackModifier = 2;
        public string damageDice = "1d4";
        public DeverQuestDamageType damageType =
            DeverQuestDamageType.Piercing;
        public List<DeverQuestDamageAffinity> damageAffinities =
            new List<DeverQuestDamageAffinity>();
        [Range(0, 100)]
        public int startingLoyalty = 50;
        public int recruitCopperCost;
        public int recoveryCopperCost = 10;

        public string CompanionId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(companionId))
                {
                    companionId = Guid.NewGuid().ToString("N");
                }
                return companionId;
            }
        }

        private void OnEnable()
        {
            Sanitize();
        }

        private void OnValidate()
        {
            Sanitize();
        }

        private void Sanitize()
        {
            _ = CompanionId;
            displayName = displayName?.Trim() ?? string.Empty;
            lore = lore?.Trim() ?? string.Empty;
            minimumAdventurerLevel =
                Mathf.Max(1, minimumAdventurerLevel);
            maximumHitPoints = Mathf.Max(1, maximumHitPoints);
            hitPointsPerLevel = Mathf.Max(0, hitPointsPerLevel);
            armorClass = Mathf.Max(1, armorClass);
            damageDice = damageDice?.Trim() ?? string.Empty;
            startingLoyalty = Mathf.Clamp(startingLoyalty, 0, 100);
            recruitCopperCost = Mathf.Max(0, recruitCopperCost);
            recoveryCopperCost = Mathf.Max(0, recoveryCopperCost);
            allowedClassIds = allowedClassIds ?? new List<string>();
            allowedClassNames =
                allowedClassNames ?? new List<string>();
            damageAffinities = damageAffinities ??
                               new List<DeverQuestDamageAffinity>();
        }
    }

    }
