using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// assume all procedural objects are circles / or using circular collider
public abstract class CircularObject : MonoBehaviour
{
    [SerializeField]
    protected float _radius;

    protected virtual void Awake()
    {
        ObstacleManager.Instance.RegisterObstacle(this);
    }

    protected virtual void OnDestroy()
    {
        ObstacleManager.Instance.DeregisterObstacle(this);
    }

    public float Radius => _radius;
}
