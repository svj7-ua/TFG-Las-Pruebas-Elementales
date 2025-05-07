
using System;
using System.Collections.Generic;

public class StateMachine{

    StateNode currentStateNode;
    Dictionary <Type, StateNode> stateNodes = new Dictionary<Type, StateNode>();

    HashSet<ITransition> anyTimeTransitions = new HashSet<ITransition>();

    // public void Update{
    //     var transition = GetTransition();
    //     if(transition != null){
    //         ChangeState(transition.To);
    //     }

    //     currentStateNode.state.Update();
    // }

    // public void FixedUpdate(){
    //     currentStateNode.state.FixedUpdate();
    // }

    // public void SetState(IState state){
    //     currentStateNode = stateNodes[state.GetType()];
    //     currentStateNode.state.OnEnter();
    // }

    // void ChangeState(IState newState){
    //     if(newState == currentStateNode.state){
    //         return;
    //     }

    //     var previousState = currentStateNode.state;
    //     var nextStateNode = stateNodes[newState.GetType()].state;

    //     previousState.OnExit();
    //     nextStateNode.OnEnter();

    //     currentStateNode = stateNodes[newState.GetType()];

    // }

    // ITransition GetTransition(){
    //     foreach(var transition in anyTimeTransitions){
    //         if(transition.Condition.Evaluate()){
    //             return transition;
    //         }
    //     }

    //     foreach(var transition in currentStateNode.transitions){
    //         if(transition.Condition.Evaluate()){
    //             return transition;
    //         }
    //     }

    //     return null;

    // }

    // public void AddTransition(IState from, IState to, IPredicate condition){
    //     GetOrCreateStateNode(from).AddTransition(to, condition);
    // }

    // public void AddTransition(IPredicate condition, IState to){
    //     anyTimeTransitions.Add(new Transition(condition, to));
    // }

    // private StateNode GetOrCreateStateNode(IState state){

    //     var node = stateNodes.GetValueOrDefault(state.GetType());
    //     if(node == null){
    //         node = new StateNode(state);
    //         stateNodes.Add(state.GetType(), node);
    //     }
    //     return node;

    // }

}
