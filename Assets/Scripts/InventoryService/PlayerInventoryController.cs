using System;
using Game.Core.Data;
using Unity.Collections;
using Unity.Netcode;

namespace InventoryService
{
    public struct NetworkItemStack : INetworkSerializable, IEquatable<NetworkItemStack>
    {
        public FixedString32Bytes ItemId;
        public int Quantity;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ItemId);
            serializer.SerializeValue(ref Quantity);
        }

        public bool Equals(NetworkItemStack other)
        {
            return ItemId.Equals(other.ItemId) && Quantity == other.Quantity;
        }
    }

    public class PlayerInventoryController : NetworkBehaviour
    {
        private readonly IInventoryService _inventory = new InventoryModel();

        private readonly NetworkVariable<int> _money = new NetworkVariable<int>(
            default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private readonly NetworkList<NetworkItemStack> _items = new NetworkList<NetworkItemStack>();

        public int Money => _money.Value;

        public void GrantMoney(int amount)
        {
            if (!IsServer) return;
            _inventory.AddMoney(amount);
            _money.Value = _inventory.Money;
            InventoryActionManager.onMoneyChanged?.Invoke(NetworkObject, _money.Value);
        }

        public void GrantItem(string itemId, int quantity)
        {
            if (!IsServer) return;
            _inventory.AddItem(itemId, quantity);
            SyncItems();
        }

        public SaveData ToSaveData()
        {
            return _inventory.ToSaveData();
        }

        public void LoadFrom(SaveData data)
        {
            if (!IsServer) return;
            _inventory.LoadFrom(data);
            _money.Value = _inventory.Money;
            SyncItems();
        }

        private void SyncItems()
        {
            _items.Clear();
            foreach (var item in _inventory.Items)
                _items.Add(new NetworkItemStack { ItemId = item.ItemId, Quantity = item.Quantity });

            InventoryActionManager.onItemsChanged?.Invoke(NetworkObject);
        }
    }
}
