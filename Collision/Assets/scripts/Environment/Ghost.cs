using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ghost : MonoBehaviour
{
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

    private bool hasChangedVelocity = false;

    private List<VerletPoint> points;

    private VerletPoint leftEyePoint;

    private VerletPoint rightEyePoint;
    
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
        //transform from local axis to world axis
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
        constraints.Add(new PointwiseConstraint(this, 5));
        constraints.Add(new EyePointDistanceConstraint(this, 5));
    }

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

    private void UpdatePointLocation()
    {
        points.ForEach(p =>
        {
            p.UpdatePosition();
        });

        leftEyePoint.UpdatePosition();
        rightEyePoint.UpdatePosition();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckDestroy();

        if (!hasChangedVelocity && rightEyePoint.CurrentPosition.y > GhostManager.Instance.GhostChangeMovementHeight)
        {
            hasChangedVelocity = true;
            AddAcc( (horizontalAcc - initialAcc) / Time.deltaTime);
        }

        UpdatePointLocation();

        constraints.ForEach(c =>
        {
            c.ResolveConstriant(this, 0.2f);
        });

        for (var i = 0; i < points.Count; i++)
        {
            body.SetPosition(i, points[i].CurrentPosition);
        }
    }

    private void CheckDestroy()
    {
        if(rightEyePoint.CurrentPosition.y > respawnThresholdTop.y + 1 || rightEyePoint.CurrentPosition.x > respawnThresholdTop.x + 1 ||
            rightEyePoint.CurrentPosition.y < respawnThresholdButtom.y - 1 || rightEyePoint.CurrentPosition.x < respawnThresholdButtom.x - 1)
        {
            OnDestroy.Invoke(this);
            Destroy(this.gameObject);
        }
    }
}
