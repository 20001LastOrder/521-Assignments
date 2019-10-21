using System.Collections;
using System.Collections.Generic;
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

        transform.position = 2 * positionInThisFrame - lastPosition + instantaneousAcc * Mathf.Pow(Time.deltaTime, 2);
        lastPosition = positionInThisFrame;
        instantaneousAcc = new Vector3();
    }

    public void AddPosition(Vector3 changement)
    {
        transform.position += changement;
    }

    public void AddInstantaneousAcc(Vector3 acc)
    {
        instantaneousAcc += acc;
    }
}
