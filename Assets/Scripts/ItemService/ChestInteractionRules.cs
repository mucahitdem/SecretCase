namespace ItemService
{
    public static class ChestInteractionRules
    {
        public static bool CanPickUp(bool isCarried) => !isCarried;

        public static bool CanToggleLid(bool isCarried, ulong carrierClientId, ulong requestingClientId) =>
            isCarried && carrierClientId == requestingClientId;

        public static bool ShouldGrantLoot(bool lidOpen, bool alreadyLooted) => lidOpen && !alreadyLooted;
    }
}
