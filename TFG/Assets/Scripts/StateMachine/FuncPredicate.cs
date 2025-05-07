using System;

public class FuncPredicate : IPredicate
{
    private Func<bool> _predicateFunc;

    public FuncPredicate(Func<bool> predicateFunc)
    {
        _predicateFunc = predicateFunc;
    }

    public bool Evaluate()
    {
        return _predicateFunc.Invoke();
    }
}