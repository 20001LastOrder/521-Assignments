using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Actor : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected NavMeshPath path;
    protected float distanceToDestination;

    public ActorState Status
    {
        get;
        protected set;
    }

    protected virtual void Start()
    {
        path = new NavMeshPath();
        agent = GetComponent<NavMeshAgent>();
        Status = ActorState.Idle;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
    }

    public IEnumerator PathFinding(Vector3 destination)
    {
        if (Status == ActorState.Moving || Status != ActorState.Processing)
        {
            yield break;
        }

        Status = ActorState.Moving;
        destination.y = transform.position.y;
        //agent.CalculatePath(destination, path);
        agent.SetDestination(destination);

        yield return null;
        //wait for pathfinding to finish
        yield return new WaitUntil(() => (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= 0.2));

        Status = ActorState.Processing;
    }
}

public enum ActorState
{
    Idle,
    Moving,
    Processing,
    FindGoal
}