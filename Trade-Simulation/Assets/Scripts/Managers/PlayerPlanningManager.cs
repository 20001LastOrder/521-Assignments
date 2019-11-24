using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manager for planning player action sequence
public class PlayerPlanningManager : ManagerBase<PlayerPlanningManager>
{
    [SerializeField]
    private Player _player;

    private Caravan _caravan;

    private List<Trader> _traders;

    private List<Action> _actions;

    private void Start()
    {
        _traders = TraderManager.Instance.Traders;
        _actions = new List<Action>();
        _caravan = _player.Caravan;

        // create a set of actions for the player
        CreateCaravanActions();
        CreateTradeActions();
        
    }

    // request a plan from current state w.r.t. some goal state
    public Stack<Action> RequestPlan(WorldState state, WorldState goalState)
    {
        // this search w.r.t. the heuristic is a Heuristic search
        StateSpaceSearch s = new StateSpaceSearch(goalState, Heuristic, _actions);
        return s.Plan(state);
    }

    private void CreateTradeActions()
    {
        for(var i = 0; i < _traders.Count; i++)
        {
            _actions.Add(new ExchangeAction(_traders[i], string.Format("trade with trader {0}", _traders[i].Name)));
        }
    }

    private void CreateCaravanActions()
    {
        // create take action
        for(var i = 0; i < _traders.Count; i++)
        {
            var deal = _traders[i].RequiredSpices();
            if(deal.Sum() > 0)
            {
                _actions.Add(new TakeAction(_caravan, "take items for trader " + _traders[i].Name, deal));
            }
        }
        var additionDeal = new SpiceVector();
        additionDeal.Spices[0] += 1;
        // create an action to take a single Tu
        _actions.Add(new TakeAction(_caravan, "caravan take for single " + SpiceVector.SpiceNames[0], additionDeal));
        // create an action to put everything to caravan
        _actions.Add(new PutAction(_caravan, "put everything to cavaran"));
    }

    // heuristic for the state1 w.r.t. the target as state 2
    private int Heuristic(WorldState state1, WorldState state2)
    {
        int distance = 0;
		int[] cost = { 1, 3, 7, 5, 9, 11, 29};
		//weight the items after more than items before 
		for (var i = 0; i < state1.CaravanStorage.Spices.Count; i++)
        {
            distance += cost[i] * System.Math.Abs(state1.PlayerStorage.Spices[i] + state1.CaravanStorage.Spices[i] - state2.CaravanStorage.Spices[i]);
        }
        distance += state1.steps;
        return distance;
    }
}
