using System.Collections.Generic;

namespace AIService.Goap
{
    public class UtilityActionSelector<TAction> where TAction : class, IUtilityAction
    {
        private readonly List<TAction> _actions = new List<TAction>();

        public void Register(TAction action)
        {
            _actions.Add(action);
        }

        public TAction SelectBest(AIWorldState state)
        {
            TAction best = null;
            var bestScore = float.MinValue;

            foreach (var action in _actions)
            {
                var score = action.Score(state);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = action;
                }
            }

            return best;
        }
    }
}
