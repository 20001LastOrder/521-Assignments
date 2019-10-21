using UnityEngine;

public class VerletPoint
{
    private Vector3 lastPosition;

    private Transform transform;

    private Vector3 instantaneousAcc;

    public Vector3 CurrentPosition => transform.position;

    public VerletPoint(Transform transform)
    {
        this.transform = transform;
        lastPosition = transform.position;
    }

    public void UpdatePosition()
    {
        var positionInThisFrame = transform.position;

        var nextPosition = 2 * positionInThisFrame - lastPosition + instantaneousAcc * Mathf.Pow(Time.deltaTime, 2); ;

        if (EnvironmentManager.Instance.InBadArea(nextPosition, 0.3f))
        {
            transform.position = positionInThisFrame;
        }
        else
        {
            transform.position = nextPosition;
            lastPosition = positionInThisFrame;
        }

        instantaneousAcc = new Vector3();
    }

    public bool CheckCollision()
    {
        foreach (var linePoints in EnvironmentManager.Instance.Colliders)
        {
            for (var i = 0; i < linePoints.Count - 1; i++)
            {
                //CollisionToCollider(linePoints[i], linePoints[i + 1]);
                if (EnvironmentManager.Instance.LinePointCollisionDetection(linePoints[i], linePoints[i + 1], transform.position, 0.3f))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void AddPosition(Vector3 changement)
    {
        var newPosition = transform.position + changement;
        if (!EnvironmentManager.Instance.InBadArea(newPosition, 0.3f))
        {
            transform.position = newPosition;
        }

    }

    public void AddInstantaneousAcc(Vector3 acc)
    {
        instantaneousAcc += acc;
    }
}
