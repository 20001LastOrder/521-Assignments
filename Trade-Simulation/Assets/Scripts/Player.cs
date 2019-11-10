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
				// log three actions beyond
				if(_numFutureActionLog - 1 < _plan.Count) {
					UIManager.Instance.LogPlayerAction(_plan.ElementAt(_numFutureActionLog - 1).Name);
				} else {
					UIManager.Instance.LogPlayerAction("Found Goal, Idle");
				}
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
				UIManager.Instance.LogPlayerAction("Found Goal, Idle");
				Status = ActorState.FindGoal;
            }
            else
            {
				UIManager.Instance.LogPlayerAction("Request New Plan...");
               _plan = PlayerPlanningManager.Instance.RequestPlan(_playerState, _goalState);
				UIManager.Instance.LogPlayerAction("Find Plan with " + _plan.Count + " steps");
				Debug.Log("Find Plan with " + _plan.Count + " steps");
				//One empty Action log for formatting
				UIManager.Instance.LogPlayerAction("");

				//log the first 3 actions
				for(var i = 0; i < 3; i++) {
					UIManager.Instance.LogPlayerAction(_plan.ElementAt(i).Name);
				}
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
