using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeAction : Action
{
    private SpiceTransferPoint _p;

    public ExchangeAction(SpiceTransferPoint p, string name)
    {
        _p = p;
        Name = name;
        Cost = 1;
    }

    public override IEnumerator Operator(WorldState s, Player player)
    {
        if (!player.CanProcessAction())
        {
            yield break;
        }
        player.BeginProcessAction();

        yield return player.PathFinding(_p.TradePoint.position);
        yield return new WaitForSeconds(0.5f);

        if (PreCondition(s))
        {
            _p.Transfer(_p.TradeRule, player.Storage);

        }


        player.EndProcessAction();
    }

    public override WorldState Effect(WorldState s)
    {
        var clone = s.Clone();
        clone.PlayerStorage.Add(_p.TradeRule);
        return clone;
    }

    public override bool PreCondition(WorldState s)
    {
        bool satisfied = EnoughItem(s.PlayerStorage) && EnoughCapacity(s);
        return satisfied;
    }

    private bool EnoughItem(SpiceVector s)
    {
        for (var i = 0; i < s.Spices.Count; i++)
        {
            if (s.Spices[i] < -_p.TradeRule.Spices[i])
            {
                return false;
            }
        }

        return true;
    }

    private bool EnoughCapacity(WorldState s)
    {
        var storage_c = s.PlayerStorage.Clone();
        storage_c.Add(_p.TradeRule);
        return storage_c.Sum() <= 4;
    }
}
