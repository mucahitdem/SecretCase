using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core.Data;

namespace InventoryService
{
    public class InventoryModel : IInventoryService
    {
        private readonly List<ItemStack> _items = new List<ItemStack>();

        public int Money { get; private set; }
        public IReadOnlyList<ItemStack> Items => _items;
        public event Action Changed;

        public void AddMoney(int amount)
        {
            if (amount <= 0) return;
            Money += amount;
            Changed?.Invoke();
        }

        public bool RemoveMoney(int amount)
        {
            if (amount <= 0 || amount > Money) return false;
            Money -= amount;
            Changed?.Invoke();
            return true;
        }

        public void AddItem(string itemId, int quantity)
        {
            if (quantity <= 0) return;

            var index = _items.FindIndex(i => i.ItemId == itemId);
            if (index >= 0)
            {
                var existing = _items[index];
                existing.Quantity += quantity;
                _items[index] = existing;
            }
            else
            {
                _items.Add(new ItemStack { ItemId = itemId, Quantity = quantity });
            }

            Changed?.Invoke();
        }

        public bool RemoveItem(string itemId, int quantity)
        {
            if (quantity <= 0) return false;

            var index = _items.FindIndex(i => i.ItemId == itemId);
            if (index < 0) return false;

            var existing = _items[index];
            if (existing.Quantity < quantity) return false;

            existing.Quantity -= quantity;
            if (existing.Quantity == 0) _items.RemoveAt(index);
            else _items[index] = existing;

            Changed?.Invoke();
            return true;
        }

        public int GetItemQuantity(string itemId)
        {
            var index = _items.FindIndex(i => i.ItemId == itemId);
            return index >= 0 ? _items[index].Quantity : 0;
        }

        public SaveData ToSaveData()
        {
            return new SaveData
            {
                Money = Money,
                Items = _items
                    .Select(i => new SaveItemEntry { ItemId = i.ItemId, Quantity = i.Quantity })
                    .ToList()
            };
        }

        public void LoadFrom(SaveData data)
        {
            Money = data.Money;
            _items.Clear();

            foreach (var entry in data.Items)
                _items.Add(new ItemStack { ItemId = entry.ItemId, Quantity = entry.Quantity });

            Changed?.Invoke();
        }
    }
}
