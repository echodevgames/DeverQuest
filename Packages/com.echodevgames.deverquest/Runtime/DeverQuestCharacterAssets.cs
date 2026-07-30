using System;
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
        Charisma = 5
    }

    public enum DeverQuestEquipmentSlot
    {
        Head = 0,
        Body = 1,
        Hands = 2,
        Feet = 3,
        MainHand = 4,
        OffHand = 5,
        Trinket = 6
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
        }

        private void OnValidate()
        {
            EnsureId();
            minimumLevel = Mathf.Max(1, minimumLevel);
        }

        private void EnsureId()
        {
            if (string.IsNullOrWhiteSpace(equipmentId))
            {
                equipmentId = Guid.NewGuid().ToString("N");
            }
        }
    }

    [CreateAssetMenu(
        fileName = "NewDeverQuestSpell",
        menuName = "DeverQuest/Character/Spell")]
    public sealed class DeverQuestSpell : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string spellId = string.Empty;
        public string displayName = "New Spell";
        [TextArea(2, 5)]
        public string description = string.Empty;
        public int spellLevel;
        public DeverQuestAbility castingAbility =
            DeverQuestAbility.Intelligence;
        public string damageDice = "1d6";
        public string statusEffect = string.Empty;
        public int minimumCharacterLevel = 1;

        public string SpellId
        {
            get
            {
                EnsureId();
                return spellId;
            }
        }

        private void OnEnable()
        {
            EnsureId();
            Sanitize();
        }

        private void OnValidate()
        {
            EnsureId();
            Sanitize();
        }

        private void EnsureId()
        {
            if (string.IsNullOrWhiteSpace(spellId))
            {
                spellId = Guid.NewGuid().ToString("N");
            }
        }

        private void Sanitize()
        {
            spellLevel = Mathf.Max(0, spellLevel);
            minimumCharacterLevel =
                Mathf.Max(1, minimumCharacterLevel);
            damageDice = damageDice?.Trim() ?? string.Empty;
            statusEffect = statusEffect?.Trim() ?? string.Empty;
        }
    }
}
