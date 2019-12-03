using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringAgent : CircularObject
{
    [SerializeField]
    protected float _maxForce = 1;

    [SerializeField]
    protected float _maxSpeed = 1;

    [SerializeField]
    protected float _mass = 1;

    [SerializeField]
    protected float _brakingFactor = 1;

    [SerializeField]
    protected float _steeringFactor = 0.5f;

    [SerializeField]
    protected float _thrustFactor = 0.5f;

    [SerializeField]
    protected float _arrivalDistance = 1;

    [SerializeField]
    protected float _obstacleAvoidanceLookahead = 1;

    // target in seeking behaviour
    public Vector3? _seekingTarget;

    // steering force at each time frame
    protected Vector3 _steering;
    // velocity
    protected Vector3 _velocity;

    public Vector3 Velocity
    {
        get => _velocity;
    }

    public float MaxSpeed => _maxSpeed;

    public float ArrivalDistance => _arrivalDistance;

    public float ObstacleAvoidanceLookahead => _obstacleAvoidanceLookahead;

    public void AddSteering(Vector3 steering)
    {
        _steering = steering;
    }

    protected virtual void Start()
    {
        _steering = new Vector3();
        _velocity = new Vector3();
        _seekingTarget = null;
    }

    protected void FixedUpdate()
    {
        ActionSelection();
        Steering();
        Locommotion();
    }

    protected void ResetSteering()
    {
        _velocity = new Vector3();
        _steering = new Vector3(); ;
    }

    protected abstract void ActionSelection();

    protected abstract void Steering();

	// simple vihecle model locomotion applied to all agents
    protected void Locommotion()
    {
        _steering = Vector3.ClampMagnitude(_steering, _maxForce);
        var acceleration = _steering / _mass;

        // transform.right is the x axis and up is the y axis
        var forward = Vector3.Project(acceleration, transform.right);
        var side = Vector3.Project(acceleration, transform.up);

        //apply simple vehicle model and calculate new acceleration
        if(Vector3.Dot(forward, transform.right) < 0)
        {
            forward = forward * _brakingFactor;
        }
        else
        {
            forward = forward * _thrustFactor;
        }
        side = side * _steeringFactor;
        acceleration = forward + side;

        // calculate new velocity
        _velocity = Vector3.ClampMagnitude(_velocity + (acceleration * Time.deltaTime), _maxSpeed);

        if(_velocity.magnitude > 0)
        {
            // rotate the agent to face new rotation
            transform.right = Vector3.Normalize(_velocity);
        }

        // calculate new position
        transform.position += _velocity * Time.deltaTime;
        _steering = new Vector3();
    }
}
