using System.Collections;
using System;
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

    public abstract IEnumerator Operator(WorldState w, Player player);

    public abstract WorldState Effect(WorldState w);
}
