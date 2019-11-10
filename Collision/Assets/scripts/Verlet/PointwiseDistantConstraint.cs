using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointwiseConstraint : Constraint
{
    // distances between one point to some other points
    List<List<float>> distances;

    public PointwiseConstraint(Ghost ghost, int iterationPerFrame) : base(ghost, iterationPerFrame)
    {
    }

    public override void ResolveConstriant(Ghost ghost, float fixFactor)
    {
        for(var iter = 0; iter < iterationPerFrame; iter++)
        {
            //check each point
            //add distance constraint;
            for (var i = 0; i < ghost.Points.Count; i++)
            {
                var distanceToOtherPoints = distances[i];
                for(var j = i + 1; j < i + 4; j++)
                {
                    var next = j % ghost.Points.Count;
                    var distance = Vector3.Distance(ghost.Points[i].CurrentPosition, ghost.Points[next].CurrentPosition);
                    var direction = (ghost.Points[next].CurrentPosition - ghost.Points[i].CurrentPosition).normalized;
                    var changement = (distance - distanceToOtherPoints[j - i - 1]) * fixFactor / 2;
                    
                    // resolve constraints by move both points closer / far away
                    ghost.Points[i].AddDisplacement(changement * direction);
                    changement *= -1;
                    ghost.Points[next].AddDisplacement(changement * direction);
                }
            }
        }
    }

    protected override void SetupConstraint(Ghost ghost)
    {
        distances = new List<List<float>>();
        //add distance constraint;
        for(var i = 0; i < ghost.Points.Count; i++)
        {
            var distancesToOtherPoints = new List<float>();

            // consider the distance from one point to the next 3 points close to it (enough to maintain the shape unless very extreme cases)
            for(var j = i + 1; j < i + 4; j++)
            {
                var next = j % ghost.Points.Count;
                distancesToOtherPoints.Add(Vector3.Distance(ghost.Points[i].CurrentPosition, ghost.Points[next].CurrentPosition));
            }
            distances.Add(distancesToOtherPoints);
        }
    }
}
