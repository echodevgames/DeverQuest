using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
[CreateAssetMenu(
        fileName = "NewGuildShop",
        menuName = "DeverQuest/Guild Shop/Shop Profile")]
    public sealed class DeverQuestShopProfile : ScriptableObject
    {
        public string displayName = "Guild Quartermaster";
        [TextArea(2, 6)]
        public string welcomeMessage =
            "Spend earned coin on provisions, training, and gear.";
        public bool availableToMembers = true;
        public List<DeverQuestShopItem> items =
            new List<DeverQuestShopItem>();
    }
}
