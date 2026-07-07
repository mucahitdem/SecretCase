using Unity.Netcode;
using UnityEngine;

namespace ItemService
{
    public interface IPickupable
    {
        bool IsCarried { get; }
        void PickUp(NetworkObject carrierRoot, Transform carryPoint);
        void Drop(Vector3 dropPosition, Vector3 throwVelocity);
    }
}
