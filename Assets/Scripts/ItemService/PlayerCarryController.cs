using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ItemService
{
    public class PlayerCarryController : NetworkBehaviour
    {
        [SerializeField]
        private float throwForce = 6f;

        private void Update()
        {
            if (!IsOwner)
                return;

            var keyboard = Keyboard.current;
            if (keyboard == null || !keyboard.gKey.wasPressedThisFrame)
                return;

            var carried = FindCarriedObject();
            if (carried == null)
                return;

            DropServerRpc(carried.NetworkObjectId);
        }

        private NetworkObject FindCarriedObject()
        {
            foreach (Transform child in transform)
            {
                var pickupable = child.GetComponent<IPickupable>();
                if (pickupable != null && pickupable.IsCarried)
                    return child.GetComponent<NetworkObject>();
            }

            return null;
        }

        [ServerRpc]
        private void DropServerRpc(ulong carriedNetworkObjectId)
        {
            if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(carriedNetworkObjectId, out var carriedObject))
                return;

            var pickupable = carriedObject.GetComponent<IPickupable>();
            if (pickupable == null)
                return;

            var dropPosition = transform.position + transform.forward * 1.5f + Vector3.up * 0.5f;
            var throwVelocity = transform.forward * throwForce + Vector3.up * 2f;
            pickupable.Drop(dropPosition, throwVelocity);
        }
    }
}