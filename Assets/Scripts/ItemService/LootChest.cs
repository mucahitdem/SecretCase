using Core;
using InteractionService;
using InventoryService;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace ItemService
{
    public class LootChest : NetworkBehaviour, IInteractable, IPickupable
    {
        [SerializeField]
        private int lootSeed;

        [SerializeField]
        private Transform lidTransform;

        [SerializeField]
        private float lidOpenAngle = 90f;

        private readonly NetworkVariable<bool> _isCarried = new NetworkVariable<bool>(
            false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<ulong> _carrierClientId = new NetworkVariable<ulong>(
            ulong.MaxValue, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<bool> _lidOpen = new NetworkVariable<bool>(
            false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<bool> _looted = new NetworkVariable<bool>(
            false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private readonly LootTable _lootTable = new LootTable();

        public bool IsCarried => _isCarried.Value;

        public string GetInteractionPrompt(NetworkObject requester)
        {
            if (!_isCarried.Value)
                return "Pick Up";

            var isMine = requester != null && _carrierClientId.Value == requester.OwnerClientId;
            if (!isMine)
                return string.Empty;

            return _lidOpen.Value ? "Close Chest" : "Open Chest";
        }

        public bool CanInteract(ulong clientId)
        {
            if (!_isCarried.Value)
                return ChestInteractionRules.CanPickUp(_isCarried.Value);
            return ChestInteractionRules.CanToggleLid(_isCarried.Value, _carrierClientId.Value, clientId);
        }

        public void Interact(ulong clientId)
        {
            if (!IsServer)
                return;

            if (!_isCarried.Value)
            {
                if (!NetworkManager.ConnectedClients.TryGetValue(clientId, out var client))
                    return;
                var playerObject = client.PlayerObject;
                if (playerObject == null)
                    return;

                var carryPoint = playerObject.GetComponentInChildren<CarryPoint>();
                if (carryPoint == null)
                    return;

                PickUp(playerObject, carryPoint.transform);
                _carrierClientId.Value = clientId;
                ItemActionManager.onPickedUp?.Invoke(NetworkObject, clientId);
            }
            else if (_carrierClientId.Value == clientId)
            {
                ToggleLid(clientId);
            }
        }

        public void PickUp(NetworkObject carrierRoot, Transform carryPoint)
        {
            if (!IsServer)
                return;
            if (carrierRoot == null)
                return;

            NetworkAttachUtility.AttachToAnchor(NetworkObject, carrierRoot.transform, carryPoint);
            _isCarried.Value = true;

            var rb = GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = true;
        }

        public void Drop(Vector3 dropPosition, Vector3 throwVelocity)
        {
            if (!IsServer)
                return;

            NetworkObject.TrySetParent((Transform)null, false);
            transform.position = dropPosition;

            var rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = throwVelocity;
            }

            _isCarried.Value = false;
            _carrierClientId.Value = ulong.MaxValue;
            ItemActionManager.onDropped?.Invoke(NetworkObject);
        }

        private void ToggleLid(ulong clientId)
        {
            var newState = !_lidOpen.Value;
            _lidOpen.Value = newState;
            UpdateLidVisual(newState);
            ItemActionManager.onLidToggled?.Invoke(NetworkObject, newState);

            if (ChestInteractionRules.ShouldGrantLoot(newState, _looted.Value))
                GrantLoot(clientId);
        }

        private void UpdateLidVisual(bool open)
        {
            if (lidTransform == null)
                return;
            lidTransform.localRotation = Quaternion.Euler(open ? -lidOpenAngle : 0f, 0f, 0f);
        }

        private void GrantLoot(ulong clientId)
        {
            if (!NetworkManager.ConnectedClients.TryGetValue(clientId, out var client))
                return;
            var playerObject = client.PlayerObject;
            if (playerObject == null)
                return;

            var inventory = playerObject.GetComponent<PlayerInventoryController>();
            if (inventory != null)
            {
                var seed = lootSeed != 0 ? lootSeed : (int)NetworkObjectId;
                inventory.GrantMoney(_lootTable.GenerateMoney(seed));
            }

            _looted.Value = true;
            ItemActionManager.onChestOpened?.Invoke(NetworkObject, clientId);
        }
    }
}