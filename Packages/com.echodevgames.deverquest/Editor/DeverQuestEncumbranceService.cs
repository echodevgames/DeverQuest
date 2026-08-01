using System;
using System.Linq;

namespace EchoDevGames.DeverQuest
{
    internal static class DeverQuestEncumbranceService
    {
        private const float CoinWeight = 0.01f;

        public static float CarryCapacity(
            DeverQuestAdventurer adventurer = null)
        {
            adventurer =
                adventurer ?? DeverQuestAdventurerService.Adventurer;
            return Math.Max(
                20f,
                30f + adventurer.strength * 2f +
                adventurer.level);
        }

        public static float CarriedWeight(
            DeverQuestAdventurer adventurer = null)
        {
            adventurer =
                adventurer ?? DeverQuestAdventurerService.Adventurer;
            float inventoryWeight =
                (adventurer.inventory ??
                 new System.Collections.Generic.List<
                     DeverQuestInventoryEntry>())
                .Where(item => item != null && item.quantity > 0)
                .Sum(item =>
                    Math.Max(0f, item.unitWeight) * item.quantity);
            return inventoryWeight +
                   DeverQuestAdventurerService.CoinPieceCount(
                       adventurer) * CoinWeight;
        }

        public static bool IsEncumbered(
            DeverQuestAdventurer adventurer = null)
        {
            return CarriedWeight(adventurer) >
                   CarryCapacity(adventurer);
        }

        public static bool DropInventory(
            string ownershipId,
            int quantity,
            out string message)
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            DeverQuestInventoryEntry entry =
                adventurer.inventory.FirstOrDefault(
                    item => item != null &&
                            item.ownershipId == ownershipId);
            if (entry == null)
            {
                message = "That carried item was not found.";
                return false;
            }
            quantity = Math.Max(1, Math.Min(quantity, entry.quantity));
            entry.quantity -= quantity;
            if (entry.quantity <= 0)
            {
                adventurer.inventory.Remove(entry);
            }
            DeverQuestAdventurerService.Save();
            message =
                $"Dropped {quantity} × {entry.displayName}. " +
                $"{CarriedWeight(adventurer):0.0}/" +
                $"{CarryCapacity(adventurer):0.0} weight remains.";
            return true;
        }
    }
}
