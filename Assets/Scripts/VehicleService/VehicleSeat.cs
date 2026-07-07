using Core;
using InteractionService;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace VehicleService
{
    public class VehicleSeat : NetworkBehaviour, IInteractable
    {
        [SerializeField]
        private bool isDriverSeat;

        [SerializeField]
        private Transform seatAnchor;

        [SerializeField]
        private NetworkBehaviour vehicleBehaviour;

        private IVehicle _vehicle;

        private readonly NetworkVariable<ulong> _occupantClientId = new NetworkVariable<ulong>(ulong.MaxValue, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private bool IsOccupied => _occupantClientId.Value != ulong.MaxValue;

        private void Awake()
        {
            _vehicle = vehicleBehaviour.transform.GetComponent<IVehicle>();
        }

        public string GetInteractionPrompt(NetworkObject requester)
        {
            if (IsOccupied)
                return requester != null && _occupantClientId.Value == requester.OwnerClientId ? "Exit" : string.Empty;

            return isDriverSeat ? "Drive" : "Board";
        }

        public bool CanInteract(ulong clientId) => !IsOccupied || _occupantClientId.Value == clientId;

        public void Interact(ulong clientId)
        {
            if (!IsServer)
                return;

            if (IsOccupied && _occupantClientId.Value == clientId)
                ExitSeat(clientId);
            else if (!IsOccupied)
                EnterSeat(clientId);
        }

        private void EnterSeat(ulong clientId)
        {
            if (!NetworkManager.ConnectedClients.TryGetValue(clientId, out var client))
                return;
            var playerObject = client.PlayerObject;
            if (playerObject == null)
                return;

            _occupantClientId.Value = clientId;

            if (vehicleBehaviour != null)
                NetworkAttachUtility.AttachToAnchor(playerObject, vehicleBehaviour.transform, seatAnchor);

            var playerMovement = playerObject.GetComponent<NetworkPlayer>();
            if (playerMovement != null)
                playerMovement.SetMovementEnabled(false);

            if (isDriverSeat && vehicleBehaviour != null)
            {
                vehicleBehaviour.NetworkObject.ChangeOwnership(clientId);
                _vehicle?.SetCaptured(true);
                VehicleActionManager.onDriverChanged?.Invoke(vehicleBehaviour.NetworkObject, clientId);
            }

            var tracker = playerObject.GetComponent<PlayerVehicleTracker>();
            if (tracker != null && vehicleBehaviour != null)
                tracker.SetCurrentVehicle(vehicleBehaviour.NetworkObject);
        }

        private void ExitSeat(ulong clientId)
        {
            if (!NetworkManager.ConnectedClients.TryGetValue(clientId, out var client))
                return;
            var playerObject = client.PlayerObject;
            if (playerObject == null)
                return;

            if (_vehicle != null && _vehicle.DeckAnchor != null && vehicleBehaviour != null)
            {
                NetworkAttachUtility.AttachToAnchor(playerObject, vehicleBehaviour.transform, _vehicle.DeckAnchor);
            }
            else
            {
                var exitPosition = seatAnchor.position;
                playerObject.TrySetParent((Transform)null, false);
                playerObject.transform.position = exitPosition;
            }

            var playerMovement = playerObject.GetComponent<NetworkPlayer>();
            if (playerMovement != null)
                playerMovement.SetMovementEnabled(true);

            if (isDriverSeat && vehicleBehaviour != null)
            {
                _vehicle?.SetControlInput(0f, 0f);
                vehicleBehaviour.NetworkObject.ChangeOwnership(NetworkManager.ServerClientId);
                _vehicle?.SetCaptured(false);
            }

            _occupantClientId.Value = ulong.MaxValue;
        }
    }
}