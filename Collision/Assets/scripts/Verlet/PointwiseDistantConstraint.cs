using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointwiseConstraint : Constraint
{
    // distances between every point
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
            for (var i = 0; i < ghost.Points.Count - 1; i++)
            {
                var distanceToOtherPoints = distances[i];
                for(var j = i + 1; j < System.Math.Min(i + 2, ghost.Points.Count); j++)
                {
                    var distance = Vector3.Distance(ghost.Points[i].CurrentPosition, ghost.Points[j].CurrentPosition);
                    var direction = (ghost.Points[j].CurrentPosition - ghost.Points[i].CurrentPosition).normalized;
                    var changement = (distance - distanceToOtherPoints[j - i - 1]) * fixFactor / 2;
                    
                    // resolve constraints
                    ghost.Points[i].AddPosition(changement * direction);
                    if (ghost.Points[i].CheckCollision())
                    {
                        ghost.Points[i].AddPosition(-changement * direction);
                        changement *= 2;
                    }

                    changement *= -1;
                    ghost.Points[j].AddPosition(changement * direction);
                }
            }
        }
    }

    protected override void SetupConstraint(Ghost ghost)
    {
        distances = new List<List<float>>();
        //add distance constraint;
        for(var i = 0; i < ghost.Points.Count - 1; i++)
        {
            var distancesToOtherPoints = new List<float>();

            for(var j = i + 1; j < System.Math.Min(i+2, ghost.Points.Count); j++)
            {
                distancesToOtherPoints.Add(Vector3.Distance(ghost.Points[i].CurrentPosition, ghost.Points[j].CurrentPosition));
            }
            distances.Add(distancesToOtherPoints);
        }
    }
}
