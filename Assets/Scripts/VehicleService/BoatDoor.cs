using InteractionService;
using Unity.Netcode;
using UnityEngine;

namespace VehicleService
{
    public class BoatDoor : NetworkBehaviour, IInteractable
    {
        [SerializeField]
        private Boat boat;

        [SerializeField]
        private Transform boardPoint;

        public string GetInteractionPrompt(NetworkObject requester) => "Board Ship";

        public bool CanInteract(ulong clientId) => true;

        public void Interact(ulong clientId)
        {
            if (!IsServer)
            {
                Debug.LogError("NOT SERVER");
                return;
            }

            if (!NetworkManager.ConnectedClients.TryGetValue(clientId, out var client))
            {
                Debug.LogError("NOT IN LIST OF CONNECTED CLIENTS");
                return;
            }
            
            var playerObject = client.PlayerObject;
            if (playerObject == null)
                return;

            var target = boardPoint != null ? boardPoint : (boat != null ? boat.DeckAnchor : null);
            if (target == null)
                return;

            playerObject.transform.SetPositionAndRotation(target.position, target.rotation);

            if (boat != null)
            {
                var tracker = playerObject.GetComponent<PlayerVehicleTracker>();
                if (tracker != null)
                    tracker.SetCurrentVehicle(boat.NetworkObject);

                VehicleActionManager.onBoarded?.Invoke(boat.NetworkObject, clientId);
            }
        }
    }
}