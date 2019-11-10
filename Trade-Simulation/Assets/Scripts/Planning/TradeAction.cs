using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// assume unlimited supply
public class TradeAction : Action
{
    private SpiceTransferPoint _p;

    private CompositeMethod _directTrade;
    private CompositeMethod _storeAndTrade;

    public TradeAction(SpiceTransferPoint p, string name, Caravan caravan)
    {
        _p = p;
        Name = name;

        var getVector = p.TradeRule.Clone();
        for(var i = 0; i < getVector.Spices.Count; i++)
        {
            if(getVector.Spices[i] >= 0)
            {
                getVector.Spices[i] = 0;
            }
            else
            {
                getVector.Spices[i] *= -1;
            }
        }

        var exchange = new ExchangeAction(p, name + " exchange");
        var put = new PutAction(caravan, name + " put first");
        var take = new TakeAction(caravan, name + " take", getVector);
        _directTrade = new CompositeMethod(new List<Action>() { exchange });
        _storeAndTrade = new CompositeMethod(new List<Action>() { put, take, exchange });

    }

    public override IEnumerator Operator(WorldState s, Player player)
    {
        if (!player.CanProcessAction())
        {
            yield break;
        }

        player.BeginProcessAction();

        if (EnoughItem(s.PlayerStorage) && EnoughCapacity(s))
        {
            player.AddNewActions(_directTrade.Actions); 
        }
        else
        {
            player.AddNewActions(_storeAndTrade.Actions);
        }

        player.EndProcessAction();
    }

    public override WorldState Effect(WorldState s)
    {
        var clone = s.Clone();
        if (EnoughItem(s.PlayerStorage) && EnoughCapacity(s))
        {
            foreach(var action in _directTrade.Actions)
            {
                clone = action.Effect(clone);
            }
        }
        else
        {
            foreach(var action in _storeAndTrade.Actions)
            {
                clone = action.Effect(clone);
            }
        }

        return clone;
    }

    public override bool PreCondition(WorldState s)
    {
        var totalStorage = new SpiceVector();
        totalStorage.Add(s.PlayerStorage);
        totalStorage.Add(s.CaravanStorage);
        return EnoughItem(totalStorage);
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

public class CompositeMethod{
    private List<Action> _actions;

    public List<Action> Actions => _actions;

    public CompositeMethod(List<Action> actions)
    {
        _actions = actions;
    }
}
