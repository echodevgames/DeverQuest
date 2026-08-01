using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    public enum DeverQuestAbility
    {
        Strength = 0,
        Dexterity = 1,
        Constitution = 2,
        Intelligence = 3,
        Wisdom = 4,
        Charisma = 5,
        Agility = 6,
        Stamina = 7,
        Luck = 8
    }

    public enum DeverQuestEquipmentSlot
    {
        Helm = 0,
        Chest = 1,
        Hands = 2,
        Boots = 3,
        MainHand = 4,
        OffHand = 5,
        Trinket = 6,
        Face = 7,
        Neck = 8,
        EarLeft = 9,
        EarRight = 10,
        Shoulders = 11,
        Back = 12,
        Legs = 13,
        Belt = 14,
        WristLeft = 15,
        WristRight = 16,
        Shirt = 17,
        RingLeft = 18,
        RingRight = 19
    }

    [CreateAssetMenu(
        fileName = "NewDeverQuestEquipment",
        menuName = "DeverQuest/Character/Equipment")]
    public sealed class DeverQuestEquipment : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string equipmentId = string.Empty;
        public string displayName = "New Equipment";
        [TextArea(2, 5)]
        public string description = string.Empty;
        public DeverQuestEquipmentSlot slot;
        public int armorClassBonus;
        public DeverQuestAbility abilityBonusType;
        public int abilityBonus;
        public int minimumLevel = 1;
        public string materialTier = "Copper";
        public string rarity = "Common";
        public int copperValue = 10;
        [Min(0f)]
        public float weight = 1f;
        public string damageDice = string.Empty;
        public DeverQuestDamageType weaponDamageType =
            DeverQuestDamageType.Slashing;
        public List<DeverQuestDamageAffinity> damageAffinities =
            new List<DeverQuestDamageAffinity>();

        public string EquipmentId
        {
            get
            {
                EnsureId();
                return equipmentId;
            }
        }

        private void OnEnable()
        {
            EnsureId();
            minimumLevel = Mathf.Max(1, minimumLevel);
            copperValue = Mathf.Max(0, copperValue);
            weight = Mathf.Max(0f, weight);
            damageDice = damageDice?.Trim() ?? string.Empty;
            damageAffinities = damageAffinities ??
                               new List<DeverQuestDamageAffinity>();
        }

        private void OnValidate()
        {
            EnsureId();
            minimumLevel = Mathf.Max(1, minimumLevel);
            copperValue = Mathf.Max(0, copperValue);
            weight = Mathf.Max(0f, weight);
            damageDice = damageDice?.Trim() ?? string.Empty;
            damageAffinities = damageAffinities ??
                               new List<DeverQuestDamageAffinity>();
        }

        private void EnsureId()
        {
            if (string.IsNullOrWhiteSpace(equipmentId))
            {
                equipmentId = Guid.NewGuid().ToString("N");
            }
        }
    }

    }
