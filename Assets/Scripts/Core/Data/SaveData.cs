using System;
using System.Collections.Generic;

namespace Game.Core.Data
{
    [Serializable]
    public class SaveItemEntry
    {
        public string ItemId;
        public int Quantity;
    }

    [Serializable]
    public class SaveData
    {
        public int Money;
        public List<SaveItemEntry> Items = new List<SaveItemEntry>();
        public List<string> LootedChestIds = new List<string>();
    }
}
