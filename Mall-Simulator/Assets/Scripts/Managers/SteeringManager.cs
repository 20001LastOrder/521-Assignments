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

    public void ObstacleAvoidance(SteeringAgent agent)
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
    }
}
