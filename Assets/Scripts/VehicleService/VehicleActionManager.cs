using System;
using Unity.Netcode;

namespace VehicleService
{
    public static class VehicleActionManager
    {
        public static Action<NetworkObject, ulong> onBoarded;
        public static Action<NetworkObject, ulong> onDriverChanged;
        public static Action<NetworkObject> onCaptured;
    }
}
