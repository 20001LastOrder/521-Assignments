using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// assume all procedural objects are circles / or using circular collider
public abstract class ProceduralObject : MonoBehaviour
{
    [SerializeField]
    protected float _radius;

    public float Radius => _radius;

    public abstract bool IsPosSatisfyConstraint(Vector3 position, System.Type objectType);
}
