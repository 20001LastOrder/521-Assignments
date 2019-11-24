using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// take action that takes something from the cavaran
public class TakeAction : Action
{
    private Caravan _p;
    private SpiceVector _deal;

    // deal is the spice that this action can take from the caravan
    public TakeAction(Caravan p, string name, SpiceVector deal)
    {
        _p = p;
        Name = name;
        _deal = deal;
        Cost = 1;
    }

    public override WorldState Effect(WorldState s)
    {
        var clone = s.Clone();
        clone.PlayerStorage.Add(_deal);
        clone.CaravanStorage.Minus(_deal);
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

        try
        {
            _p.Transfer(_deal, player.Storage);
        }
        catch
        {
        }

        player.EndProcessAction();
    }

    // check if caravan has enough storage and player has enough capacity
    public override bool PreCondition(WorldState s)
    {
        return hasEnoughStorage(s) && EnoughCapacity(s);
    }

    private bool hasEnoughStorage(WorldState s)
    {
        for (var i = 0; i < _deal.Spices.Count; i++)
        {
            if (s.CaravanStorage.Spices[i] < _deal.Spices[i])
            {
                return false;
            }
        }

        return true;
    }

    private bool EnoughCapacity(WorldState s)
    {
        var storage_c = s.PlayerStorage.Clone();
        storage_c.Add(_deal);
        return storage_c.Sum() <= 4;
    }
}
