using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InteractionService
{
    public class PlayerInteractor : NetworkBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float interactRange = 3f;
        [SerializeField] private LayerMask interactMask = ~0;

        private string _lastPrompt = string.Empty;

        private void Update()
        {
            if (!IsOwner || playerCamera == null) return;

            var hasTarget = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward,
                out var hit, interactRange, interactMask);
            var interactable = hasTarget ? hit.collider.GetComponentInParent<IInteractable>() : null;

            var rawPrompt = interactable != null ? interactable.GetInteractionPrompt(NetworkObject) : string.Empty;
            var displayPrompt = string.IsNullOrEmpty(rawPrompt) ? string.Empty : $"[E] {rawPrompt}";

            if (displayPrompt != _lastPrompt)
            {
                _lastPrompt = displayPrompt;
                InteractionActionManager.onPromptChanged?.Invoke(displayPrompt);
            }

            var keyboard = Keyboard.current;
            if (keyboard == null || !keyboard.eKey.wasPressedThisFrame) return;

            var interactableBehaviour = interactable as NetworkBehaviour;
            if (interactableBehaviour == null) return;

            InteractServerRpc(new NetworkBehaviourReference(interactableBehaviour));
        }

        [ServerRpc]
        private void InteractServerRpc(NetworkBehaviourReference interactableRef)
        {
            if (!interactableRef.TryGet(out NetworkBehaviour behaviour)) return;

            var interactable = behaviour as IInteractable;
            if (interactable == null) return;
            if (!interactable.CanInteract(OwnerClientId)) return;

            interactable.Interact(OwnerClientId);
            InteractionActionManager.onInteracted?.Invoke(interactable, OwnerClientId);
        }
    }
}
