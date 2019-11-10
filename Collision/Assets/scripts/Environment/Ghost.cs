using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ghost : MonoBehaviour
{
    //ghost parts
    [SerializeField]
    private LineRenderer body;

    [SerializeField]
    private Transform rightEye;

    [SerializeField]
    private Transform leftEye;

    [SerializeField]
    private GameObject pointPrefab;

    [SerializeField]
    private Vector3 initialAcc;

    [SerializeField]
    private Vector3 horizontalAcc;

    [SerializeField]
    private int constraintResolveIterationPerFrame = 1;

    [SerializeField]
    private float constraintResolveCoef = 0.05f;
    
    private bool hasChangedVelocity = false;

    private List<VerletPoint> points;

    private VerletPoint leftEyePoint;

    private VerletPoint rightEyePoint;
    
    //action to do when the ghost is destroyed (create a new ghost)
    public event Action<Ghost> OnDestroy;

    public List<VerletPoint> Points => points;

    public VerletPoint LeftEye => leftEyePoint;

    public VerletPoint RightEye => rightEyePoint;

    private Vector3 respawnThresholdTop;

    private Vector3 respawnThresholdButtom;

    private List<Constraint> constraints;

    // Start is called before the first frame update
    void Start()
    {
        //transform from local axis to world axis and create a set of verlet points
        points = new List<VerletPoint>();
        Vector3[] bodyPoints = new Vector3[body.positionCount];
        body.GetPositions(bodyPoints);
        for(var i = 0; i < body.positionCount; i++)
        {
            var worldPosition = transform.TransformPoint(bodyPoints[i]);
            bodyPoints[i] = worldPosition;
            var pointTransform = Instantiate(pointPrefab, bodyPoints[i], Quaternion.identity, transform).transform;
            points.Add(new VerletPoint(pointTransform));
        }
        body.useWorldSpace = true;
        body.SetPositions(bodyPoints);

        leftEyePoint = new VerletPoint(leftEye);
        rightEyePoint = new VerletPoint(rightEye);

        //apply init Acc, divide by time fragment to make sure it has the velocity after apply the acc
        AddAcc(initialAcc / Time.deltaTime);

        // get respawn threshold
        respawnThresholdTop = EnvironmentManager.Instance.UpperRightCorner;
        respawnThresholdButtom = EnvironmentManager.Instance.LowerLeftCorner;
        //load constraints
        constraints = new List<Constraint>();
        constraints.Add(new PointwiseConstraint(this, constraintResolveIterationPerFrame));
        constraints.Add(new EyePointDistanceConstraint(this, constraintResolveIterationPerFrame));
    }

    // add instantaneous acceleration to all of the points
    private void AddAcc(Vector3 acc)
    {
        //apply initial acc
        points.ForEach(p =>
        {
            p.AddInstantaneousAcc(acc);
        });

        leftEyePoint.AddInstantaneousAcc(acc);
        rightEyePoint.AddInstantaneousAcc(acc);
    }

    // update position for all the points
    private void UpdatePointLocation()
    {
        points.ForEach(p =>
        {
            p.UpdatePosition();
        });

        leftEyePoint.UpdatePosition();
        rightEyePoint.UpdatePosition();
    }

    void FixedUpdate()
    {
        CheckDestroy();

        if (!hasChangedVelocity && rightEyePoint.CurrentPosition.y > GhostManager.Instance.GhostChangeMovementHeight)
        {
            hasChangedVelocity = true;
            AddAcc( (horizontalAcc - initialAcc) / Time.deltaTime);
        }

        UpdatePointLocation();

        // check and resolve constraint
        constraints.ForEach(c =>
        {
            c.ResolveConstriant(this, constraintResolveCoef);
        });

        CheckCollision();

        //draw ghost
        for (var i = 0; i < points.Count; i++)
        {
            body.SetPosition(i, points[i].CurrentPosition);
        }
    }

    // check collision for each side of the ghost
    public void CheckCollision()
    {
        foreach (var linePoints in EnvironmentManager.Instance.Colliders)
        {
            for (var i = 0; i < linePoints.Count - 1; i++)
            {
                for(var j = 0; j < points.Count; j++)
                {
                    var next = (j + 1) % points.Count;
                    if (EnvironmentManager.Instance.LineLineIntersection(linePoints[i], linePoints[i + 1], points[j].CurrentPosition, points[next].CurrentPosition))
                    {
                        //points.ForEach(p => p.ReversePosition());
                        points[j].ReversePosition();
                        points[next].ReversePosition();
                        return;
                    }
                }
            }
        }
    }

    // check whether the ghost should be destroyed
    private void CheckDestroy()
    {
        if(rightEyePoint.CurrentPosition.y > respawnThresholdTop.y + 2 || rightEyePoint.CurrentPosition.x > respawnThresholdTop.x + 2 ||
            rightEyePoint.CurrentPosition.y < respawnThresholdButtom.y - 2 || rightEyePoint.CurrentPosition.x < respawnThresholdButtom.x - 2)
        {
            OnDestroy.Invoke(this);
            Destroy(this.gameObject);
        }
    }
}
