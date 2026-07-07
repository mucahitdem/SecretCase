using System;
using Unity.Netcode;
using UnityEngine;

namespace GunService
{
    public static class GunActionManager
    {
        public static Action<NetworkObject, Vector3, Vector3> onFired;
        public static Action<NetworkObject, GameObject> onHit;
        public static Action<Vector3, Vector3, bool> onFireEffect;
    }
}
