public class Transition : ITransition
{
    public IPredicate Condition { get; private set; }
    public IState To { get; private set; }

    public Transition(IPredicate condition, IState to)
    {
        Condition = condition;
        To = to;
    }

}