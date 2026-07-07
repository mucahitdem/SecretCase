using System;

namespace InteractionService
{
    public static class InteractionActionManager
    {
        public static Action<string> onPromptChanged;
        public static Action<IInteractable, ulong> onInteracted;
    }
}
