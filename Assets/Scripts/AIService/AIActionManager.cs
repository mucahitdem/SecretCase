using System;
using Unity.Netcode;

namespace AIService
{
    public static class AIActionManager
    {
        public static Action<NetworkObject, AIGuardState> onGuardStateChanged;
    }
}
