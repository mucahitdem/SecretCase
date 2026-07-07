using System;
using Unity.Netcode;

namespace InventoryService
{
    public static class InventoryActionManager
    {
        public static Action<NetworkObject, int> onMoneyChanged;
        public static Action<NetworkObject> onItemsChanged;
    }
}
