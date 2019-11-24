using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

// the class for executing player action
public class Player : Actor
{
    // Cavaran information
    [SerializeField]
    private Caravan _caravan;

    // Spice for the goal state
    [SerializeField]
    public SpiceVector _goalVector;

    // plan for Player to execute
    private Stack<Action> _plan;
    // current player state
    private WorldState _playerState;
    // goal for the player -> comes from the goal vector
    private WorldState _goalState;

    /**
     * current inventory of the player 
     */ 
    public SpiceVector Storage {
        get;
        private set;
    }

    /**
     * current world state (including inventory of the player and the caravan)
     */
    public WorldState PlayerState => _playerState;

    public Caravan Caravan => _caravan;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Storage = new SpiceVector();
        _plan = new Stack<Action>();
        _playerState = new WorldState(_caravan.Storage, Storage);
        _goalState = new WorldState(_goalVector, null);

        //initial logging for the inventory
        UIManager.Instance.LogStateInventory(_playerState.PlayerStorage, _playerState.CaravanStorage);
    }

    // Update is called once per frame
    void Update()
    {
       if(Status == ActorState.Idle && _plan.Count > 0)
        {
           // get new action
           var action = _plan.Pop();

            // check precondition and execure action
			if (action.PreCondition(_playerState))
			{
				// move indicator to the next action of the plan
				UIManager.Instance.MoveToNextPlayerAction();
				StartCoroutine(ActionWithLogging(action));
			}
			else
			{
                // if the precondition does not meet, clear plan to triger replanning.
				_plan = new Stack<Action>();
			}
        }
        else if(_plan.Count == 0 && Status == ActorState.Idle)
        {
            // if there is no action in plan and we reaches goal state, finish
            if (_playerState.Reaches(_goalState))
            {
				UIManager.Instance.AddNewPlayerActoinLog("Found Goal, Idle");
				UIManager.Instance.ShowPlayerActionLog();
				UIManager.Instance.MoveToNextPlayerAction();
				Status = ActorState.FindGoal;
                SimulationManager.Instance.IsGoalReached = true;
            }
            else
            {
                // Replan with new plan action log
				//clear old action logs
				UIManager.Instance.ClearPlayerActionLog();
				UIManager.Instance.AddNewPlayerActoinLog("----------------");
				UIManager.Instance.AddNewPlayerActoinLog("Request New Plan...");
               _plan = PlayerPlanningManager.Instance.RequestPlan(_playerState, _goalState);

				//add new action log and put indicator to that action
				UIManager.Instance.AddNewPlayerActoinLog("Find Plan with " + _plan.Count + " steps");
				UIManager.Instance.AddNewPlayerActoinLog("Mission Start!!!!");
				Debug.Log("Find Plan with " + _plan.Count + " steps");
				

				//log all the actions
				foreach (var action in _plan) {
					UIManager.Instance.AddNewPlayerActoinLog(action.Name);
				}

				//show the player action log 
				UIManager.Instance.ShowPlayerActionLog();
			}
		}
    }

    // perform action and log inventory after
    public IEnumerator ActionWithLogging(Action action)
    {
        yield return action.Operator(_playerState, this);
        UIManager.Instance.LogStateInventory(_playerState.PlayerStorage, _playerState.CaravanStorage);
    }

    // state transition
    public void BeginProcessAction()
    {
        if(Status == ActorState.Idle)
        {
            Status = ActorState.Processing;
        }
    }

    // state transition
    public void EndProcessAction()
    {
        if (Status == ActorState.Processing)
        {
            Status = ActorState.Idle;
        }
    }

    /**
     * Deprecated: used to consider composite action
     */
    public void AddNewActions(List<Action> actions)
    {
        for(var i = actions.Count - 1; i >= 0; i--)
        {
            _plan.Push(actions[i]);
        }
    }

    /**
     * Check if there is a current action performing by the Player
     */
    public bool CanProcessAction()
    {
        return Status == ActorState.Idle;
    }
}
