using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Action Planning and Executing for the thief
public class Theif : Actor
{
    // way point that the theif travels to
    [SerializeField]
    private List<Transform> _wayPoints;

    [SerializeField]
    private Player _player;

    private float timeCounter;

    private bool _isInSpecialAction = false;

    private bool _isReachedPlayer = false;

    private int successTimes;
    private SpiceVector _inventory;

    private System.Random _numberGenerator;

    protected override void Start()
    {
        base.Start();
        _numberGenerator = new System.Random();
        _inventory = new SpiceVector();
        // log initial theif inventory
		UIManager.Instance.LogThiefInventory(_inventory);
    }

    void Update()
    {
        // check if the theif is on a stealing mission
        if (_isInSpecialAction)
        {
            return;
        }
        

        timeCounter += Time.deltaTime;

        // every 5 seconds, toss to check if there is a stealing action to be executed
        if (timeCounter > 5)
        {
            timeCounter = 0;

            // if already steal 2 items, does not do anything
			if (successTimes >= 2) {
				return;
			}

			var draw = _numberGenerator.NextDouble();
			if (draw <= 0.166)
            {
				_isInSpecialAction = true;
				UIManager.Instance.LogThiefAction("Toss " + draw.ToString("0.0000") + " steal in cavaran");
				StartCoroutine(GoToCaravanAndTakeRandomSpice());
            }else if(successTimes < 2 && draw > 0.166 && draw <= 0.33)
            {
				_isInSpecialAction = true;
				UIManager.Instance.LogThiefAction("Toss " + draw.ToString("0.0000") + " steal from player");
				StartCoroutine(ReachPlayerAndTakeRandomSpice());
			} else{
				UIManager.Instance.LogThiefAction("Toss " + draw.ToString("0.0000") + " do nothing");
			}
        }

        // else if player is idle, move it to a random way point
        if (Status == ActorState.Idle)
        {
			Status = ActorState.Processing;
			StartCoroutine(GoToRandomWayPoint());
			UIManager.Instance.LogThiefAction("Traveling To Random Way Point");
		}
	}

    /**
     * Pathing finding and travel to a waypoint
     */
    private IEnumerator GoToRandomWayPoint()
    {
        Status = ActorState.Processing;
        var index = _numberGenerator.Next(_wayPoints.Count);
        yield return PathFinding(_wayPoints[index].position);
        yield return new WaitForSeconds(0.5f);
        Status = ActorState.Idle;
    }

    /**
     * Try to steal from the caravan
     */
    private IEnumerator GoToCaravanAndTakeRandomSpice()
    {
        _isInSpecialAction = true;
        //wait for idle
        while (Status != ActorState.Idle)
        {
            yield return null;
        }

        Status = ActorState.Processing;
        yield return PathFinding(_player.Caravan.TradePoint.position);
        // steal from caravan
        TakeRandomSpice(_player.PlayerState.CaravanStorage);

        _isInSpecialAction = false;
        Status = ActorState.Idle;
    }

    // Take a random Spice fro from the vector
    // vectorToStole: player inventory or cavaran inventory
    private void TakeRandomSpice(SpiceVector vectorToStole)
    {
        // first find all non-zero entries
        var nonZeroIndexes = new List<int>();
        for (var i = 0; i < vectorToStole.Spices.Count; i++)
        {
            if (vectorToStole.Spices[i] > 0)
            {
                nonZeroIndexes.Add(i);
            }
        }

        // if there is something in the Caravan, randomly pick one of them
        if (nonZeroIndexes.Count > 0)
        {
            var randomIndex = _numberGenerator.Next(nonZeroIndexes.Count);
            vectorToStole.Spices[nonZeroIndexes[randomIndex]] -= 1;
            _inventory.Spices[nonZeroIndexes[randomIndex]] += 1;
            successTimes += 1;
			UIManager.Instance.LogThiefInventory(_inventory);
            UIManager.Instance.LogStateInventory(_player.PlayerState.PlayerStorage, _player.PlayerState.CaravanStorage);
			UIManager.Instance.LogThiefAction("Get " + SpiceVector.SpiceNames[nonZeroIndexes[randomIndex]]);
			if(successTimes >= 2) {
				UIManager.Instance.LogThiefAction("Finish Steal Mission, YEAH!!");
			}
		}
	}

    // try steal from the player
    private IEnumerator ReachPlayerAndTakeRandomSpice()
    {
        _isInSpecialAction = true;
        //wait for idle
        while (Status != ActorState.Idle)
        {
            yield return null;
        }

        Status = ActorState.Processing;

        // start trace the player
        StartCoroutine(FollowPlayer());
        // wait until reach player
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _player.transform.position) < 1);
        // take a spice from player
        TakeRandomSpice(_player.PlayerState.PlayerStorage);

        _isReachedPlayer = true;

		// wait for the "FollowPlayer" coroutine to finish
		while (Status != ActorState.Processing) {
			yield return null;
		}
		Status = ActorState.Idle;
		_isInSpecialAction = false;
	}

    // trace the player
	private IEnumerator FollowPlayer()
    {
        if(Status != ActorState.Processing)
        {
            yield break;
        }

        Status = ActorState.Moving;
        _isReachedPlayer = false;

        // continue re-pathfinding since the player is moving as well
        while (!_isReachedPlayer)
        {
            agent.SetDestination(_player.transform.position);
            yield return new WaitForSeconds(0.1f);
        }

		//current place is destination to stop moving
		agent.isStopped = true;
		agent.isStopped = false;
		Status = ActorState.Processing;
    }
}
