namespace AIService.Goap
{
    public class DeadAction : IUtilityAction
    {
        public string Name => "Dead";
        public float Score(AIWorldState state) => state.IsAlive ? -1f : 100f;
    }

    public class CombatAction : IUtilityAction
    {
        private readonly float _combatRange;

        public CombatAction(float combatRange)
        {
            _combatRange = combatRange;
        }

        public string Name => "Combat";

        public float Score(AIWorldState state)
        {
            if (!state.IsAlive) return -1f;
            if (state.TookDamageRecently) return 90f;
            return state.DistanceToPlayer <= _combatRange ? 80f : -1f;
        }
    }

    public class AlertAction : IUtilityAction
    {
        private readonly float _alertRange;

        public AlertAction(float alertRange)
        {
            _alertRange = alertRange;
        }

        public string Name => "Alert";

        public float Score(AIWorldState state)
        {
            if (!state.IsAlive) return -1f;
            return state.DistanceToPlayer <= _alertRange ? 50f : -1f;
        }
    }

    public class IdleAction : IUtilityAction
    {
        public string Name => "Idle";
        public float Score(AIWorldState state) => state.IsAlive ? 1f : -1f;
    }
}
