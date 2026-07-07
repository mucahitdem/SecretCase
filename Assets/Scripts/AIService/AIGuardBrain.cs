using AIService.Goap;

namespace AIService
{
    public enum AIGuardState
    {
        Idle,
        Alert,
        Combat,
        Dead
    }

    public class AIGuardBrain
    {
        private readonly UtilityActionSelector<IUtilityAction> _selector = new UtilityActionSelector<IUtilityAction>();

        public AIGuardState CurrentState { get; private set; } = AIGuardState.Idle;

        public AIGuardBrain(float alertRange, float combatRange)
        {
            _selector.Register(new DeadAction());
            _selector.Register(new CombatAction(combatRange));
            _selector.Register(new AlertAction(alertRange));
            _selector.Register(new IdleAction());
        }

        public AIGuardState Evaluate(float distanceToPlayer, bool isAlive, bool tookDamage)
        {
            if (CurrentState == AIGuardState.Dead) return CurrentState;

            var state = new AIWorldState(distanceToPlayer, isAlive, tookDamage);
            var best = _selector.SelectBest(state);

            CurrentState = best?.Name switch
            {
                "Dead" => AIGuardState.Dead,
                "Combat" => AIGuardState.Combat,
                "Alert" => AIGuardState.Alert,
                _ => AIGuardState.Idle
            };

            return CurrentState;
        }
    }
}
