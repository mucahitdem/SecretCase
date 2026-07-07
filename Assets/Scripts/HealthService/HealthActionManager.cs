using System;
using Unity.Netcode;

namespace HealthService
{
    public static class HealthActionManager
    {
        public static Action<NetworkObject, float, ulong> onDamaged;
        public static Action<NetworkObject> onDied;
    }
}
