using Unity.Netcode;

namespace InteractionService
{
    public interface IInteractable
    {
        string GetInteractionPrompt(NetworkObject requester);
        bool CanInteract(ulong clientId);
        void Interact(ulong clientId);
    }
}
