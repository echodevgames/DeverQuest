//----- DeverQuestContractService.cs START -----

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoDevGames.DeverQuest
{
    internal static class DeverQuestContractService
    {
        public static void SetStatus(
            DeverQuestQuestContract contract,
            DeverQuestContractStatus status)
        {
            if (contract == null)
            {
                return;
            }

            bool leadershipAction =
                status == DeverQuestContractStatus.Offered ||
                status == DeverQuestContractStatus.Returned ||
                status == DeverQuestContractStatus.Approved ||
                status == DeverQuestContractStatus.Completed;
            if (leadershipAction &&
                !DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageContracts,
                    contract.projectName))
            {
                Debug.LogWarning(
                    "[DeverQuest] Current Guild account cannot manage " +
                    $"Contracts for {contract.projectName}.");
                return;
            }

            contract.status = status;
            EditorUtility.SetDirty(contract);
            AssetDatabase.SaveAssets();
            DeverQuestGuildAccountService.AddAudit(
                "Contract " + status,
                contract.contractTitle,
                contract.projectName);
        }

        public static bool SetStatus(
            string contractId,
            DeverQuestContractStatus status)
        {
            DeverQuestQuestContract contract = Find(contractId);
            if (contract == null)
            {
                return false;
            }

            SetStatus(contract, status);
            return true;
        }

        public static DeverQuestQuestContract Find(string contractId)
        {
            if (string.IsNullOrWhiteSpace(contractId))
            {
                return null;
            }

            string[] guids =
                AssetDatabase.FindAssets("t:DeverQuestQuestContract");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DeverQuestQuestContract contract =
                    AssetDatabase.LoadAssetAtPath<DeverQuestQuestContract>(
                        path);
                if (contract != null &&
                    contract.ContractId == contractId)
                {
                    return contract;
                }
            }

            return null;
        }

        public static bool CanJoin(
            DeverQuestQuestContract contract,
            DeverQuestAdventurer adventurer,
            out string reason)
        {
            reason = string.Empty;
            if (contract == null || adventurer == null)
            {
                reason = "Contract or Adventurer was unavailable.";
                return false;
            }
            if (contract.ContainsAdventurer(
                    adventurer.characterName))
            {
                return true;
            }
            if (!contract.HasOpenPartySlot)
            {
                reason = "This Quest party is full.";
                return false;
            }
            if (adventurer.level <
                contract.minimumAdventurerLevel)
            {
                reason =
                    $"Requires Level {contract.minimumAdventurerLevel}.";
                return false;
            }
            if (contract.restrictToClasses &&
                !(contract.eligibleClassDefinitions ??
                  new List<DeverQuestClassDefinition>())
                    .Any(item =>
                        item != null &&
                        (item.IdentityId == adventurer.classId ||
                         string.Equals(
                             item.displayName,
                             adventurer.characterClass,
                             StringComparison.OrdinalIgnoreCase))) &&
                !(contract.eligibleClasses ??
                  new List<string>()).Any(
                    item => string.Equals(
                        item,
                        adventurer.characterClass,
                        StringComparison.OrdinalIgnoreCase)))
            {
                reason =
                    $"{adventurer.characterClass} is not eligible.";
                return false;
            }
            if (contract.restrictToAncestries &&
                !(contract.eligibleAncestries ??
                  new List<DeverQuestAncestry>())
                    .Any(item =>
                        item != null &&
                        (item.IdentityId ==
                         adventurer.ancestryId ||
                         string.Equals(
                             item.displayName,
                             adventurer.ancestryName,
                             StringComparison.OrdinalIgnoreCase))))
            {
                string ancestryDisplayName =
                    string.IsNullOrWhiteSpace(adventurer.ancestryName)
                        ? "This Ancestry"
                        : adventurer.ancestryName;
                reason =
                    $"{ancestryDisplayName} is not eligible.";
                return false;
            }
            if (contract.restrictToDepartments &&
                !contract.eligibleDepartments.Any(
                    item => string.Equals(
                        item,
                        adventurer.homeDepartment,
                        StringComparison.OrdinalIgnoreCase)))
            {
                reason =
                    $"{adventurer.homeDepartment} is not eligible.";
                return false;
            }
            if (contract.groupQuest &&
                !contract.openToAnyMember &&
                contract.assignedAdventurers != null &&
                contract.assignedAdventurers.Count > 0 &&
                !contract.assignedAdventurers.Any(
                    item => string.Equals(
                        item,
                        adventurer.characterName,
                        StringComparison.OrdinalIgnoreCase)))
            {
                reason =
                    "This Party Quest has a reserved Adventurer roster.";
                return false;
            }
            bool individuallyAssigned =
                string.IsNullOrWhiteSpace(
                    contract.assignedAdventurer) ||
                string.Equals(
                    contract.assignedAdventurer,
                    adventurer.characterName,
                    StringComparison.OrdinalIgnoreCase);
            if (!contract.openToAnyMember &&
                !contract.groupQuest &&
                !individuallyAssigned)
            {
                reason =
                    "This Quest is assigned to another Adventurer.";
                return false;
            }
            return true;
        }

        public static bool Join(
            DeverQuestQuestContract contract,
            DeverQuestAdventurer adventurer,
            string developerName,
            out string error)
        {
            if (!CanJoin(contract, adventurer, out error))
            {
                return false;
            }
            if (contract.ContainsAdventurer(
                    adventurer.characterName))
            {
                return true;
            }
            contract.partyMembers.Add(
                new DeverQuestPartyMember
                {
                    adventurerName =
                        adventurer.characterName,
                    developerName =
                        developerName?.Trim() ?? string.Empty,
                    partyRole =
                        adventurer.homeDepartment,
                    joinedUtc = DateTime.UtcNow.ToString("O")
                });
            contract.status =
                contract.HasOpenPartySlot && contract.groupQuest
                    ? DeverQuestContractStatus.Offered
                    : DeverQuestContractStatus.Accepted;
            EditorUtility.SetDirty(contract);
            AssetDatabase.SaveAssets();
            DeverQuestGuildAccountService.AddAudit(
                "Party Joined",
                contract.contractTitle,
                $"{adventurer.characterName} · " +
                $"{contract.partyMembers.Count}/" +
                $"{contract.maximumParticipants}");
            return true;
        }

        public static void SubmitParticipant(
            string contractId,
            string adventurerName)
        {
            DeverQuestQuestContract contract = Find(contractId);
            if (contract == null)
            {
                return;
            }
            DeverQuestPartyMember member =
                contract.partyMembers.FirstOrDefault(
                    item => string.Equals(
                        item.adventurerName,
                        adventurerName,
                        StringComparison.OrdinalIgnoreCase));
            if (member != null)
            {
                member.submitted = true;
                member.submittedUtc =
                    DateTime.UtcNow.ToString("O");
            }
            bool partyReady =
                !contract.groupQuest ||
                (contract.partyMembers.Count >=
                 contract.maximumParticipants &&
                 contract.partyMembers.All(item => item.submitted));
            contract.status = partyReady
                ? DeverQuestContractStatus.Submitted
                : DeverQuestContractStatus.Active;
            EditorUtility.SetDirty(contract);
            AssetDatabase.SaveAssets();
            DeverQuestGuildAccountService.AddAudit(
                "Party Turn-In",
                contract.contractTitle,
                $"{adventurerName} submitted; " +
                $"{contract.partyMembers.Count(item => item.submitted)}/" +
                $"{contract.maximumParticipants}");
        }

        public static void RecordStageCompletion(
            string contractId,
            string stageId,
            string stageTitle,
            string adventurerName)
        {
            DeverQuestQuestContract contract = Find(contractId);
            if (contract == null ||
                string.IsNullOrWhiteSpace(stageId) ||
                contract.stageProgress.Any(item =>
                    item.stageId == stageId &&
                    string.Equals(
                        item.adventurerName,
                        adventurerName,
                        StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }
            contract.stageProgress.Add(
                new DeverQuestPartyStageProgress
                {
                    stageId = stageId,
                    stageTitle = stageTitle,
                    adventurerName = adventurerName,
                    completedUtc = DateTime.UtcNow.ToString("O")
                });
            EditorUtility.SetDirty(contract);
            AssetDatabase.SaveAssets();
            DeverQuestGuildAccountService.AddAudit(
                "Focus Stage Completed",
                contract.contractTitle,
                $"{adventurerName} · {stageTitle}");
        }
    }
}

//----- DeverQuestContractService.cs END -----
