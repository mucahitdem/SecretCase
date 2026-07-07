using System;
using System.Collections.Generic;
using Game.Core.Data;

namespace InventoryService
{
    public struct ItemStack
    {
        public string ItemId;
        public int Quantity;
    }

    public interface IInventoryService
    {
        int Money { get; }
        IReadOnlyList<ItemStack> Items { get; }

        event Action Changed;

        void AddMoney(int amount);
        bool RemoveMoney(int amount);
        void AddItem(string itemId, int quantity);
        bool RemoveItem(string itemId, int quantity);
        int GetItemQuantity(string itemId);

        SaveData ToSaveData();
        void LoadFrom(SaveData data);
    }
}
