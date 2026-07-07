namespace AIService.Goap
{
    public interface IUtilityAction
    {
        string Name { get; }
        float Score(AIWorldState state);
    }
}
