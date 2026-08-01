using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    public enum DeverQuestShopItemType
    {
        Equipment = 0,
        Spell = 1,
        Consumable = 2,
        Food = 3,
        Drink = 4,
        InnRest = 5,
        BreakPermit = 6,
        Redemption = 7
    }

    public enum DeverQuestItemRarity
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4,
        Artifact = 5
    }

    public enum DeverQuestItemBinding
    {
        Unbound = 0,
        BindOnPickup = 1,
        BindOnEquip = 2,
        AccountBound = 3,
        GuildBound = 4
    }

    public enum DeverQuestRealRewardType
    {
        None = 0,
        DiscordNitro = 1,
        Merchandise = 2,
        GiftCard = 3,
        MonetaryBonus = 4,
        Custom = 5
    }

    [Serializable]
    public sealed class DeverQuestInventoryEntry
    {
        public string shopItemId = string.Empty;
        public string displayName = string.Empty;
        public DeverQuestShopItemType itemType;
        public int quantity;
        public string ownershipId = string.Empty;
        public DeverQuestItemRarity rarity;
        public DeverQuestItemBinding binding;
        public string boundAccountId = string.Empty;
        public bool tradable = true;
        public string acquiredUtc = string.Empty;
        public string acquisitionSource = string.Empty;
        [Min(0f)]
        public float unitWeight = 0.25f;

        public void EnsureOwnership(string accountId)
        {
            bool legacyEntry =
                string.IsNullOrWhiteSpace(ownershipId) &&
                string.IsNullOrWhiteSpace(acquiredUtc) &&
                binding == DeverQuestItemBinding.Unbound &&
                string.IsNullOrWhiteSpace(boundAccountId);
            if (string.IsNullOrWhiteSpace(ownershipId))
            {
                ownershipId = Guid.NewGuid().ToString("N");
            }
            if (string.IsNullOrWhiteSpace(acquiredUtc))
            {
                acquiredUtc = DateTime.UtcNow.ToString("O");
            }
            if (legacyEntry)
            {
                tradable = true;
                acquisitionSource = "Legacy Inventory Migration";
            }
            if (binding == DeverQuestItemBinding.BindOnPickup ||
                binding == DeverQuestItemBinding.AccountBound)
            {
                boundAccountId =
                    accountId?.Trim() ?? string.Empty;
                tradable = false;
                binding = DeverQuestItemBinding.AccountBound;
            }
            quantity = Mathf.Max(0, quantity);
            unitWeight = Mathf.Max(0f, unitWeight);
        }
    }

    [CreateAssetMenu(
        fileName = "NewGuildShopItem",
        menuName = "DeverQuest/Guild Shop/Shop Item")]
    public sealed class DeverQuestShopItem : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string shopItemId = string.Empty;

        public string displayName = "New Guild Shop Item";
        [TextArea(2, 6)]
        public string description = string.Empty;
        public DeverQuestShopItemType itemType;
        public int copperCost = 10;
        public int minimumLevel = 1;
        public bool requiresLeadershipApproval;
        public bool reusable;
        public int maximumOwned = 99;
        [Min(0f)]
        public float unitWeight = 0.25f;

        [Header("Rarity, Ownership, and Trading")]
        public DeverQuestItemRarity rarity =
            DeverQuestItemRarity.Common;
        public DeverQuestItemBinding binding =
            DeverQuestItemBinding.Unbound;
        public bool tradable = true;

        [Header("Real Reward Redemption")]
        public DeverQuestRealRewardType realRewardType =
            DeverQuestRealRewardType.None;
        [TextArea(2, 5)]
        public string fulfillmentInstructions = string.Empty;

        [Header("Character Assets")]
        public DeverQuestEquipment equipment;
        public DeverQuestSpell spell;

        [Header("Wellness Effects")]
        public int restoreHitPoints;
        public int restoreMana;
        public int hungerChange;
        public int restChange;
        public int happinessChange;
        public int approvedBreakMinutes;

        public string ShopItemId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(shopItemId))
                {
                    shopItemId = Guid.NewGuid().ToString("N");
                }
                return shopItemId;
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
            _ = ShopItemId;
            displayName = displayName?.Trim() ?? string.Empty;
            copperCost = Mathf.Max(0, copperCost);
            minimumLevel = Mathf.Max(1, minimumLevel);
            maximumOwned = Mathf.Max(1, maximumOwned);
            unitWeight = Mathf.Max(0f, unitWeight);
            if (itemType == DeverQuestShopItemType.Redemption)
            {
                requiresLeadershipApproval = true;
                tradable = false;
                binding = DeverQuestItemBinding.AccountBound;
                maximumOwned = 1;
            }
            restoreHitPoints = Mathf.Max(0, restoreHitPoints);
            restoreMana = Mathf.Max(0, restoreMana);
            approvedBreakMinutes =
                Mathf.Max(0, approvedBreakMinutes);
        }
    }

    }
