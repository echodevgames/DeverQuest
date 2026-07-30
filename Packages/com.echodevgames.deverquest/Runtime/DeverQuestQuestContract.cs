//----- DeverQuestQuestContract.cs START -----

using System;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    public enum DeverQuestContractStatus
    {
        Draft = 0,
        Offered = 1,
        Accepted = 2,
        Active = 3,
        Submitted = 4,
        Approved = 5,
        Returned = 6,
        Completed = 7
    }

    public enum DeverQuestContractPriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Critical = 3
    }

    [CreateAssetMenu(
        fileName = "NewQuestContract",
        menuName = "DeverQuest/Quest Contract")]
    public sealed class DeverQuestQuestContract : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string contractId = string.Empty;

        [Header("Assignment")]
        public string contractTitle = "New Quest Contract";
        public DeverQuestContractStatus status =
            DeverQuestContractStatus.Draft;
        public DeverQuestContractPriority priority =
            DeverQuestContractPriority.Normal;
        public string createdBy = string.Empty;
        public string assignedAdventurer = string.Empty;
        public bool openToAnyMember;
        public int minimumAdventurerLevel = 1;
        public string dueDate = string.Empty;

        [Header("Template")]
        public DeverQuestQuestProfile questProfile;

        [Header("Actual Work")]
        public string projectName = string.Empty;
        public string taskName = string.Empty;
        public string department = "Programming";
        [TextArea(3, 8)]
        public string objective = string.Empty;
        [TextArea(3, 10)]
        public string deliverables = string.Empty;

        [Header("Snapshotted Spoils")]
        public int suggestedFocusMinutes = 50;
        public int baseCopper = 10;
        public int baseExperience = 10;
        public int workBlockMinutes = 30;
        public int copperPerWorkBlock = 25;
        public int experiencePerWorkBlock = 50;

        [Header("Future Encounter Hook")]
        public string encounterProfileId = string.Empty;
        [TextArea(2, 5)]
        public string encounterNotes = string.Empty;

        public string ContractId
        {
            get
            {
                EnsureId();
                return contractId;
            }
        }

        public void InitializeFromProfile(
            DeverQuestQuestProfile profile,
            string creator)
        {
            questProfile = profile;
            createdBy = creator?.Trim() ?? string.Empty;
            if (profile == null)
            {
                return;
            }

            contractTitle = profile.displayName;
            projectName = profile.projectName;
            taskName = profile.taskName;
            department = profile.department;
            objective = profile.goalTemplate;
            minimumAdventurerLevel =
                profile.minimumAdventurerLevel;
            suggestedFocusMinutes = profile.suggestedFocusMinutes;
            baseCopper = profile.baseCopper;
            baseExperience = profile.baseExperience;
            workBlockMinutes = profile.workBlockMinutes;
            copperPerWorkBlock = profile.copperPerWorkBlock;
            experiencePerWorkBlock =
                profile.experiencePerWorkBlock;
            Sanitize();
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
            if (string.IsNullOrWhiteSpace(contractId))
            {
                contractId = Guid.NewGuid().ToString("N");
            }
        }

        private void Sanitize()
        {
            contractTitle = contractTitle?.Trim() ?? string.Empty;
            createdBy = createdBy?.Trim() ?? string.Empty;
            assignedAdventurer =
                assignedAdventurer?.Trim() ?? string.Empty;
            dueDate = dueDate?.Trim() ?? string.Empty;
            projectName = projectName?.Trim() ?? string.Empty;
            taskName = taskName?.Trim() ?? string.Empty;
            department = department?.Trim() ?? string.Empty;
            encounterProfileId =
                encounterProfileId?.Trim() ?? string.Empty;
            suggestedFocusMinutes = Mathf.Max(1, suggestedFocusMinutes);
            minimumAdventurerLevel =
                Mathf.Max(1, minimumAdventurerLevel);
            baseCopper = Mathf.Max(0, baseCopper);
            baseExperience = Mathf.Max(0, baseExperience);
            workBlockMinutes = Mathf.Max(1, workBlockMinutes);
            copperPerWorkBlock = Mathf.Max(0, copperPerWorkBlock);
            experiencePerWorkBlock =
                Mathf.Max(0, experiencePerWorkBlock);
        }
    }
}

//----- DeverQuestQuestContract.cs END -----
