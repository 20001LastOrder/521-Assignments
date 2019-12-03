using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringManager : ManagerBase<SteeringManager>
{
	// seek behaviour
    public void Seek(SteeringAgent agent)
    {
        // if the agent does not have any target, do nothing
        if(agent._seekingTarget == null)
        {
            return;
        }

		agent.AddSteering(GetSeekForce(agent, agent._seekingTarget.Value));
    }

	// wandering behaviour
    public void Wandering(SteeringAgent agent, float wanderingFactor)
    {
        var wanderingForce = wanderingFactor * Random.insideUnitCircle.normalized;
        agent.AddSteering(wanderingForce);
    }

	// pursuit
	public void Pursue(SteeringAgent agent, SteeringAgent target) {
		agent.AddSteering(GetPursueForce(agent, target));
	}

	// flee
	public void Flee(SteeringAgent agent, SteeringAgent target) {
		agent.AddSteering(-GetPursueForce(agent, target));
	}

	// flow field following
    public void FlowFieldFollowing(SteeringAgent agent, Vector3 desiredVelocity)
    {
        // set to the maximum speed of the agent
        desiredVelocity = desiredVelocity.normalized * agent.MaxSpeed;
        agent.AddSteering(desiredVelocity - agent.Velocity);
    }

	// flow field ObstacleAvoidance
	public void ObstacleAvoidance(SteeringAgent agent) {
		//get the side vector
		var side = agent.transform.up;
		var forward = agent.transform.right;
		var ray1Pos = side.normalized * agent.Radius + agent.transform.position;
		var ray2Pos = side.normalized * -agent.Radius + agent.transform.position;

		// check collision using rays
		var c1 = CheckCollision(ray1Pos, forward, agent.ObstacleAvoidanceLookahead);
		var c2 = CheckCollision(ray2Pos, forward, agent.ObstacleAvoidanceLookahead);
		var colliderToAvoid = FindNearestCollider(agent, c1, c2);

		//if no obstacle in front then return
		if (colliderToAvoid == null) {
			return;
		}
		var colliderToAgent = agent.transform.position - colliderToAvoid.transform.position;
		//if the distance is smallar than the taget distance, reutrn null
		if (agent._seekingTarget!= null && Vector3.Distance(agent.transform.position, agent._seekingTarget.Value) < colliderToAgent.magnitude) {
			return;
		}

		// calculate the steering force

		// find engage vector
		var engagingVector = Vector3.Project(colliderToAgent, forward);
		// find a escaping vector
		var escapingVector = colliderToAgent - engagingVector;
		var colliderRadius = colliderToAvoid.bounds.size.x;

		if (colliderToAvoid.tag.Equals("Shop")) {
			escapingVector = -Mathf.Sign(agent.transform.position.y) * Vector3.up;
		} else {
			// if the direction is pointing to the center of the collider, choose either way
			if (escapingVector.magnitude < 0.01f) {
				escapingVector = -side;
			}
		}
			//add a force perpendicular to the engagingDistance (0.01 to avoid division by 0)
			agent.AddSteering(escapingVector.normalized / (Mathf.Max(0, engagingVector.magnitude - colliderRadius) + 0.1f));

	}

	private Collider2D CheckCollision(Vector3 rayPosition, Vector3 rayDirection, float distance) {
		// Bit shift the index of the layer (10) to get a bit mask
		int layerMask = (1 << 10) + (1 << 11);
		layerMask = ~layerMask;

		Debug.DrawRay(rayPosition, rayDirection * distance, Color.red, 1/60.0f);
		RaycastHit2D hit = Physics2D.Raycast(rayPosition, rayDirection, distance, layerMask);
		if(hit.collider != null) {
			return hit.collider;
		}
		return null;
	}

	private Collider2D FindNearestCollider(SteeringAgent agent, Collider2D c1, Collider2D c2) {
		if(c1 == null) {
			return c2;
		}

		if (c2 == null) {
			return c1;
		}

		if(Vector3.Distance(agent.transform.position, c1.transform.position) > Vector3.Distance(agent.transform.position, c2.transform.position)){
			return c2;
		} else {
			return c1;
		}
	}

	private Vector3 GetSeekForce(SteeringAgent agent, Vector3 target) {
		//calculate seeking force
		var offset = target - agent.transform.position;
		var distance = offset.magnitude;
		var allowedSpeed = Mathf.Min(agent.MaxSpeed, agent.MaxSpeed * (distance / agent.ArrivalDistance));
		var desiredSpeed = offset.normalized * allowedSpeed;
		return desiredSpeed - agent.Velocity;
	}

	private Vector3 GetPursueForce(SteeringAgent agent, SteeringAgent target) {
		var position = target.transform.position;
		var velocity = target.Velocity;
		var nextPosition = position + velocity * Time.deltaTime;
		return GetSeekForce(agent, nextPosition);
	}

}
