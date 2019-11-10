using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// constraint for left and right eyes
public class EyePointDistanceConstraint : Constraint
{
    List<float> leftEyeDistances;
    List<float> rightEyeDistances;

    public EyePointDistanceConstraint(Ghost ghost, int iterationPerFrame) : base(ghost, iterationPerFrame)
    {
    }

    public override void ResolveConstriant(Ghost ghost, float fixFactor)
    {
        ResolveDistanceConstraint(ghost.LeftEye, ghost.Points, leftEyeDistances, fixFactor);
        ResolveDistanceConstraint(ghost.RightEye, ghost.Points, rightEyeDistances, fixFactor);
    }

    //set up the constraint by maintaining distance from left and right eye to each points
    protected override void SetupConstraint(Ghost ghost)
    {
        leftEyeDistances = CalculateDistances(ghost.LeftEye.CurrentPosition, ghost.Points);
        rightEyeDistances = CalculateDistances(ghost.RightEye.CurrentPosition, ghost.Points);
    }

    private List<float> CalculateDistances(Vector3 position, List<VerletPoint> points)
    {
        List<float> distances = new List<float>();

        foreach(var point in points)
        {
            distances.Add(Vector3.Distance(position, point.CurrentPosition));
        }

        return distances;
    }

    private void ResolveDistanceConstraint(VerletPoint point, List<VerletPoint> points, List<float> distances, float fixFactor)
    {
        for (var i = 0; i < points.Count; i++)
        {
            var distance = Vector3.Distance(point.CurrentPosition, points[i].CurrentPosition);
            var direction = (points[i].CurrentPosition - point.CurrentPosition).normalized;
            var changement = (distance - distances[i]) * fixFactor / 2;

            // resolve constraints by move both points colser / far away
            point.AddDisplacement(changement * direction);
            changement *= -1;
            points[i].AddDisplacement(changement * direction);
        }
    }
}
