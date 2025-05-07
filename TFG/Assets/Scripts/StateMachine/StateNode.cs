using System.Collections.Generic;

public class StateNode{

    public IState state { get; private set; }
    public HashSet<ITransition> transitions {get;}

    public StateNode(IState state){
        this.state = state;
        transitions = new HashSet<ITransition>();
    }

    public void AddTransition(IPredicate condition, IState to){
        transitions.Add(new Transition(condition, to));
    }

}