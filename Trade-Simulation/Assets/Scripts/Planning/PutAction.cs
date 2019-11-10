using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutAction : Action
{
    private Caravan _p;

    public PutAction(Caravan p, string name)
    {
        _p = p;
        Name = name;
        Cost = 1;
    }

    public override WorldState Effect(WorldState s)
    {
        var clone = s.Clone();
        clone.CaravanStorage.Add(s.PlayerStorage);
        clone.PlayerStorage.Minus(s.PlayerStorage);
        return clone;
    }

    public override IEnumerator Operator(WorldState w, Player player)
    {
        if (!player.CanProcessAction())
        {
            yield break;
        }
        player.BeginProcessAction();
        yield return player.PathFinding(_p.TradePoint.position);
        yield return new WaitForSeconds(0.5f);

        var deal = new SpiceVector();
        deal.Minus(player.Storage);
        _p.Transfer(deal, player.Storage);

        player.EndProcessAction();
    }

    public override bool PreCondition(WorldState s)
    {
        return HasSomething(s);
    }

    private bool HasSomething(WorldState s)
    {
        return s.PlayerStorage.Sum() > 0;
    }
}
