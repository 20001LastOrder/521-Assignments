using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Theif : Actor
{
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
    }

    void Update()
    {
        if (_isInSpecialAction)
        {
            return;
        }
        

        timeCounter += Time.deltaTime;

        if (timeCounter > 5)
        {
            timeCounter = 0;
            var draw = _numberGenerator.NextDouble();
               
            if (successTimes < 2 && draw <= 0.166f )
            {
                StartCoroutine(TakeRandomSpiceAndTakeRandomSpice());
            }else if(successTimes < 2 && draw > 0.166 && draw <= 0.33)
            {
                StartCoroutine(ReachPlayerAndTakeRandomSpice());
            }
        }

        if (Status == ActorState.Idle)
        {
            StartCoroutine(GoToRandomWayPoint());
        }
    }

    private IEnumerator GoToRandomWayPoint()
    {
        Status = ActorState.Processing;
        var index = _numberGenerator.Next(_wayPoints.Count);
        yield return PathFinding(_wayPoints[index].position);
        yield return new WaitForSeconds(2.0f);
        Status = ActorState.Idle;
    }

    private IEnumerator TakeRandomSpiceAndTakeRandomSpice()
    {
        _isInSpecialAction = true;
        //wait for idle
        while (Status != ActorState.Idle)
        {
            yield return null;
        }

        Status = ActorState.Processing;
        yield return PathFinding(_player.Caravan.TradePoint.position);
        TakeRandomSpice(_player.PlayerState.CaravanStorage);


        _isInSpecialAction = false;
        Status = ActorState.Idle;
    }

    private void TakeRandomSpice(SpiceVector vectorToStole)
    {
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
            Debug.Log("Get " + SpiceVector.SpiceNames[nonZeroIndexes[randomIndex]]);
            UIManager.Instance.LogInventory(_player.PlayerState.PlayerStorage, _player.PlayerState.CaravanStorage);
        }
    }

    private IEnumerator ReachPlayerAndTakeRandomSpice()
    {
        _isInSpecialAction = true;
        //wait for idle
        while (Status != ActorState.Idle)
        {
            yield return null;
        }


        Status = ActorState.Processing;
        StartCoroutine(FollowPlayer());

        yield return new WaitUntil(() => Vector3.Distance(transform.position, _player.transform.position) < 1);

        TakeRandomSpice(_player.PlayerState.PlayerStorage);

        _isReachedPlayer = true;
        _isInSpecialAction = false;
        Status = ActorState.Idle;
    }

    private IEnumerator FollowPlayer()
    {
        if(Status != ActorState.Processing)
        {
            yield break;
        }

        Status = ActorState.Moving;
        _isReachedPlayer = false;

        while (!_isReachedPlayer)
        {
            agent.SetDestination(_player.transform.position);
            yield return new WaitForSeconds(0.1f);
        }

        //current place is destination to stop moving
        agent.SetDestination(transform.position);
        Status = ActorState.Processing;
    }
}
