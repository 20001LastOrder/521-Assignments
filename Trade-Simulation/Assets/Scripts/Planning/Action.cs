using System.Collections;
using System;

// Generic action calss
public abstract class Action
{
    public string Name {
        get;
        protected set;
    }

    public int Cost
    {
        get;
        protected set;
    }

    public abstract bool PreCondition(WorldState w);

    // the operator for the action: where the action actually happens
    public abstract IEnumerator Operator(WorldState w, Player player);

    public abstract WorldState Effect(WorldState w);
}
