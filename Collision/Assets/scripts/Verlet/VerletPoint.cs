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

        //perform verlet positional update
        var nextPosition = 2 * positionInThisFrame - lastPosition + instantaneousAcc * Mathf.Pow(Time.deltaTime, 2); ;

        // if the point has a collision with some point, simply put it but to the original position (no collision effect)
        if (EnvironmentManager.Instance.InBadArea(positionInThisFrame, nextPosition, 0.3f))
        {
            transform.position = positionInThisFrame;
        }
        else
        {
            transform.position = nextPosition;
            lastPosition = positionInThisFrame;
        }

        // treat all acceleration to be instananeous
        instantaneousAcc = new Vector3();
    }

    public void ReversePosition()
    {
        transform.position = lastPosition;
    }

    public void AddDisplacement(Vector3 changement)
    {
        var newPosition = transform.position + changement;
        if (!EnvironmentManager.Instance.InBadArea(transform.position, newPosition, 0.3f))
        {
            transform.position = newPosition;
        }

    }

    public void AddInstantaneousAcc(Vector3 acc)
    {
        instantaneousAcc += acc;
    }
}
