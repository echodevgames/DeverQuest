//----- DeverQuestContractService.cs START -----

using UnityEditor;

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

            contract.status = status;
            EditorUtility.SetDirty(contract);
            AssetDatabase.SaveAssets();
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
    }
}

//----- DeverQuestContractService.cs END -----
