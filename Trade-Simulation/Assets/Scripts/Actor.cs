using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// An actor that can perform pathfinding through NavMesh
[RequireComponent(typeof(NavMeshAgent))]
public abstract class Actor : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected NavMeshPath path;

    public ActorState Status
    {
        get;
        protected set;
    }

    protected virtual void Start()
    {
        // initized navMesh and state
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

        // wait for one frame for the navMesh to start
        yield return null;
        //wait for pathfinding to finish
        yield return new WaitUntil(() => (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= 0.2));

        Status = ActorState.Processing;
    }
}

// Status for the actor to perform the plan
public enum ActorState
{
    Idle,
    Moving,
    Processing,
    FindGoal
}