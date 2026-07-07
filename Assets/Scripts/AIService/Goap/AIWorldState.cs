namespace AIService.Goap
{
    public readonly struct AIWorldState
    {
        public readonly float DistanceToPlayer;
        public readonly bool IsAlive;
        public readonly bool TookDamageRecently;

        public AIWorldState(float distanceToPlayer, bool isAlive, bool tookDamageRecently)
        {
            DistanceToPlayer = distanceToPlayer;
            IsAlive = isAlive;
            TookDamageRecently = tookDamageRecently;
        }
    }
}
