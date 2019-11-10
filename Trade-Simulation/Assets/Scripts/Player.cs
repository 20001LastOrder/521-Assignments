using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Player : Actor
{
    [SerializeField]
    private Caravan _caravan;

    [SerializeField]
    public SpiceVector _goalVector;

	[SerializeField]
	private int _numFutureActionLog = 3;

    private int i = -1;
    private Stack<Action> _plan;
    private WorldState _playerState;
    private WorldState _goalState;

    public SpiceVector Storage {
        get;
        private set;
    }

    public WorldState PlayerState => _playerState;

    public Caravan Caravan => _caravan;

    public void SetPlan(Stack<Action> plan)
    {
        _plan = plan;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Storage = new SpiceVector();
        _plan = new Stack<Action>();
        _playerState = new WorldState(_caravan.Storage, Storage);
        _goalState = new WorldState(_goalVector, null);

        //initial logging
        UIManager.Instance.LogStateInventory(_playerState.PlayerStorage, _playerState.CaravanStorage);
    }

    // Update is called once per frame
    void Update()
    {
       if(Status == ActorState.Idle && _plan.Count > 0)
        {
           UIManager.Instance.LogStateInventory(_playerState.PlayerStorage, _playerState.CaravanStorage); 
           var action = _plan.Pop();

			if (action.PreCondition(_playerState))
			{
				// move indicator to the next action of the plan
				UIManager.Instance.MoveToNextPlayerAction();
				StartCoroutine(ActionWithLogging(action));
			}
			else
			{
				_plan = new Stack<Action>();
			}
        }
        else if(_plan.Count == 0 && Status == ActorState.Idle)
        {
            if (_playerState.Reaches(_goalState))
            {
				UIManager.Instance.AddNewPlayerActoinLog("Found Goal, Idle");
				UIManager.Instance.ShowPlayerActionLog();
				UIManager.Instance.MoveToNextPlayerAction();
				Status = ActorState.FindGoal;
            }
            else
            {
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

    public IEnumerator ActionWithLogging(Action action)
    {
        yield return action.Operator(_playerState, this);
        UIManager.Instance.LogStateInventory(_playerState.PlayerStorage, _playerState.CaravanStorage);
    }

    public void BeginProcessAction()
    {
        if(Status == ActorState.Idle)
        {
            Status = ActorState.Processing;
        }
    }

    public void EndProcessAction()
    {
        if (Status == ActorState.Processing)
        {
            Status = ActorState.Idle;
        }
    }

    public void AddNewActions(List<Action> actions)
    {
        for(var i = actions.Count - 1; i >= 0; i--)
        {
            _plan.Push(actions[i]);
        }
    }

    public bool CanProcessAction()
    {
        return Status == ActorState.Idle;
    }

    public void Trade(SpiceTransferPoint p, SpiceVector deal)
    {
        p.Transfer(deal, Storage);
    }

    public Vector3 GetMousePosition()
    {
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
        {
            return hit.point;
        }
        return new Vector3();
    }
}
