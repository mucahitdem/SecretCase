using System;
using Unity.Netcode;

namespace ItemService
{
    public static class ItemActionManager
    {
        public static Action<NetworkObject, ulong> onPickedUp;
        public static Action<NetworkObject> onDropped;
        public static Action<NetworkObject, bool> onLidToggled;
        public static Action<NetworkObject, ulong> onChestOpened;
    }
}
