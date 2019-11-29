using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// assume all procedural objects are circles / or using circular collider
public abstract class CircularObject : MonoBehaviour
{
    [SerializeField]
    protected float _radius;

    public float Radius => _radius;
}
