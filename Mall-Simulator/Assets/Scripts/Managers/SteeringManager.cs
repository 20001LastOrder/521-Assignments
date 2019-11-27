using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringManager : ManagerBase<SteeringManager>
{
    public void Seek(SteeringAgent agent)
    {
        // if the agent does not have any target, do nothing
        if(agent._seekingTarget == null)
        {
            return;
        }

        //calculate seeking force
        var offset = agent._seekingTarget.Value - agent.transform.position;
        var distance = offset.magnitude;
        var allowedSpeed = Mathf.Min(agent.MaxSpeed, agent.MaxSpeed * (distance / agent.ArrivalDistance));
        var desiredSpeed = offset.normalized * allowedSpeed;
        agent.AddSteering(desiredSpeed - agent.Velocity);
    }

    public void FlowFieldFollowing(SteeringAgent agent, Vector3 desiredVelocity)
    {
        agent.AddSteering(desiredVelocity - agent.Velocity);
    }

	public void ObstacleAvoidance(SteeringAgent agent) {
		//agent.gameObject.layer = 10;
		//get the side vector
		var side = agent.transform.up;
		var forward = agent.transform.right;
		var ray1Pos = side.normalized * agent.Radius + agent.transform.position;
		var ray2Pos = side.normalized * -agent.Radius + agent.transform.position;
		var c1 = CheckCollision(ray1Pos, forward, agent.ObstacleAvoidanceLookahead);
		var c2 = CheckCollision(ray2Pos, forward, agent.ObstacleAvoidanceLookahead);
		var colliderToAvoid = FindNearestCollider(agent, c1, c2);

		//if no obstacle in front then return
		if (colliderToAvoid == null) {
			return;
		}
		var colliderToAgent = agent.transform.position - colliderToAvoid.transform.position;
		agent.gameObject.layer = 0;
		//if the distance is smallar than the taget distance, reutrn null
		if (agent._seekingTarget!= null && Vector3.Distance(agent.transform.position, agent._seekingTarget.Value) < colliderToAgent.magnitude) {
			return;
		}

		// calculate the steering force

		// find engage vector
		var engagingVector = Vector3.Project(colliderToAgent, forward);
		// find a escaping vector
		var escapingVector = colliderToAgent - engagingVector;


		if (colliderToAvoid.tag.Equals("Shop")) {
			escapingVector = -Mathf.Sign(agent.transform.position.y) * Vector3.up;
		} else {
			// if the direction is pointing to the center of the collider, choose either way
			if (escapingVector.magnitude < 0.01f) {
				escapingVector = -side;
			}
		}
			//add a force perpendicular to the engagingDistance (0.01 to avoid division by 0)
			agent.AddSteering(escapingVector.normalized / (engagingVector.magnitude + 0.01f));

	}

	private Collider2D CheckCollision(Vector3 rayPosition, Vector3 rayDirection, float distance) {
		// Bit shift the index of the layer (10) to get a bit mask
		int layerMask = 1 << 10;
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

/*    public void ObstacleAvoidance(SteeringAgent agent)
    {
        var destinationVector = agent._seekingTarget != null ? (agent._seekingTarget.Value - agent.transform.position) : agent.transform.right * agent.ObstacleAvoidanceLookahead;

        float objectDistance = -1;
        Vector3 objectSideVector = new Vector3();

        foreach (var obj in ObstacleManager.Instance.Obstacles)
        {
            // check the distance vector
            var offset = obj.transform.position - agent.transform.position;

            // we want to treat the table as a whole
            var radius = (obj.GetType() == typeof(Table)) ? ((Table)obj).FullRadius : obj.Radius;
            var minSafeDistance = agent.Radius + radius;

            //discard trivial non dangerous objects
            if (obj == agent ||
                offset.magnitude > destinationVector.magnitude ||
                offset.magnitude > agent.ObstacleAvoidanceLookahead + minSafeDistance ||
                // if the object is behind the agent
                Vector3.Dot(offset, agent.transform.right) < 0 ||
                // if the goal is begind the agent, the agent will eventually turn
                Vector3.Dot(destinationVector, agent.transform.right) < 0)
            {
                continue;
            }

            //check the on the agent's local axis
            var forward = Vector3.Project(offset, agent.transform.right);
            var side = Vector3.Project(offset, agent.transform.up);
            
            //if the object is not on the way, discard
            if(side.magnitude > minSafeDistance)
            {
                continue;
            }

            // if the object is the most dangerous, avoid it
            if(objectDistance == -1 || forward.magnitude < objectDistance){
                objectDistance = forward.magnitude;
                objectSideVector = side;
            }
        }

        // add steering force to avoid the object
        if (objectDistance > 0)
        {
            if (Vector3.Dot(objectSideVector, agent.transform.up) > 0)
            {
                agent.AddSteering(-agent.transform.up * agent.MaxSpeed / objectDistance);
            }
            else
            {
                agent.AddSteering(agent.transform.up * agent.MaxSpeed / objectDistance);
            }
        }
    }*/
}
