//----- DeverQuestQuestContract.cs START -----

using System;
using System.Collections.Generic;
using System.Linq;
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

    [Serializable]
    public sealed class DeverQuestFocusStage
    {
        public string stageId = string.Empty;
        public string stageTitle = "New Focus Stage";
        [TextArea(2, 6)]
        public string workObjective = string.Empty;
        public int focusedMinutesRequired = 15;
        public string assignedPartyRole = string.Empty;
        public int copperReward;
        public int experienceReward;
        public bool allowEarlyTurnIn = true;
        public int earlyCompletionCopperBonus;
        public int earlyCompletionExperienceBonus;
        public string encounterProfileId = string.Empty;
        public DeverQuestEncounterProfile encounterProfile;

        public void Sanitize()
        {
            if (string.IsNullOrWhiteSpace(stageId))
            {
                stageId = Guid.NewGuid().ToString("N");
            }
            stageTitle = stageTitle?.Trim() ?? string.Empty;
            assignedPartyRole =
                assignedPartyRole?.Trim() ?? string.Empty;
            encounterProfileId =
                encounterProfile == null
                    ? encounterProfileId?.Trim() ?? string.Empty
                    : encounterProfile.EncounterId;
            focusedMinutesRequired =
                Mathf.Max(1, focusedMinutesRequired);
            copperReward = Mathf.Max(0, copperReward);
            experienceReward = Mathf.Max(0, experienceReward);
            earlyCompletionCopperBonus =
                Mathf.Max(0, earlyCompletionCopperBonus);
            earlyCompletionExperienceBonus =
                Mathf.Max(0, earlyCompletionExperienceBonus);
        }
    }

    [Serializable]
    public sealed class DeverQuestPartyMember
    {
        public string adventurerName = string.Empty;
        public string developerName = string.Empty;
        public string partyRole = string.Empty;
        public string joinedUtc = string.Empty;
        public bool submitted;
        public string submittedUtc = string.Empty;
    }

    [Serializable]
    public sealed class DeverQuestPartyStageProgress
    {
        public string stageId = string.Empty;
        public string stageTitle = string.Empty;
        public string adventurerName = string.Empty;
        public string completedUtc = string.Empty;
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
        public bool groupQuest;
        public int maximumParticipants = 1;
        public List<string> assignedAdventurers =
            new List<string>();
        public List<DeverQuestPartyMember> partyMembers =
            new List<DeverQuestPartyMember>();
        public List<DeverQuestPartyStageProgress> stageProgress =
            new List<DeverQuestPartyStageProgress>();
        [TextArea(2, 8)]
        public string questStory = string.Empty;
        public bool restrictToClasses;
        public List<DeverQuestClassDefinition>
            eligibleClassDefinitions =
                new List<DeverQuestClassDefinition>();
        public List<string> eligibleClasses =
            new List<string>();
        public bool restrictToAncestries;
        public List<DeverQuestAncestry> eligibleAncestries =
            new List<DeverQuestAncestry>();
        public bool restrictToDepartments;
        public List<string> eligibleDepartments =
            new List<string>();
        public int groupBonusCopper;
        public int groupBonusExperience;
        public List<DeverQuestFocusStage> focusStages =
            new List<DeverQuestFocusStage>();

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

        public bool HasOpenPartySlot =>
            partyMembers == null ||
            partyMembers.Count < Mathf.Max(1, maximumParticipants);

        public bool ContainsAdventurer(string adventurerName)
        {
            return partyMembers != null &&
                   partyMembers.Any(member =>
                       string.Equals(
                           member.adventurerName,
                           adventurerName,
                           StringComparison.OrdinalIgnoreCase));
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
            RefreshSpoilsFromProfile();
            Sanitize();
        }

        public bool SpoilsMatchLinkedProfile()
        {
            if (questProfile == null)
            {
                return true;
            }

            return suggestedFocusMinutes ==
                   questProfile.suggestedFocusMinutes &&
                   baseCopper == questProfile.baseCopper &&
                   baseExperience == questProfile.baseExperience &&
                   workBlockMinutes == questProfile.workBlockMinutes &&
                   copperPerWorkBlock ==
                   questProfile.copperPerWorkBlock &&
                   experiencePerWorkBlock ==
                   questProfile.experiencePerWorkBlock;
        }

        public bool CanRefreshSpoilsFromProfile()
        {
            return questProfile != null &&
                   (status == DeverQuestContractStatus.Draft ||
                    status == DeverQuestContractStatus.Offered ||
                    status == DeverQuestContractStatus.Returned);
        }

        public void RefreshSpoilsFromProfile()
        {
            if (questProfile == null)
            {
                return;
            }

            suggestedFocusMinutes =
                questProfile.suggestedFocusMinutes;
            baseCopper = questProfile.baseCopper;
            baseExperience = questProfile.baseExperience;
            workBlockMinutes = questProfile.workBlockMinutes;
            copperPerWorkBlock =
                questProfile.copperPerWorkBlock;
            experiencePerWorkBlock =
                questProfile.experiencePerWorkBlock;
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
            maximumParticipants =
                groupQuest
                    ? Mathf.Max(2, maximumParticipants)
                    : 1;
            partyMembers = partyMembers ??
                           new List<DeverQuestPartyMember>();
            assignedAdventurers = assignedAdventurers ??
                                  new List<string>();
            stageProgress = stageProgress ??
                            new List<DeverQuestPartyStageProgress>();
            if (!groupQuest && partyMembers.Count > 1)
            {
                partyMembers.RemoveRange(
                    1, partyMembers.Count - 1);
            }
            eligibleClasses = eligibleClasses ??
                              new List<string>();
            eligibleClassDefinitions =
                eligibleClassDefinitions ??
                new List<DeverQuestClassDefinition>();
            eligibleAncestries =
                eligibleAncestries ??
                new List<DeverQuestAncestry>();
            eligibleClassDefinitions.RemoveAll(
                value => value == null);
            eligibleAncestries.RemoveAll(
                value => value == null);
            eligibleDepartments = eligibleDepartments ??
                                  new List<string>();
            focusStages = focusStages ??
                          new List<DeverQuestFocusStage>();
            foreach (DeverQuestFocusStage stage in focusStages)
            {
                stage?.Sanitize();
            }
            groupBonusCopper = Mathf.Max(0, groupBonusCopper);
            groupBonusExperience =
                Mathf.Max(0, groupBonusExperience);
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
