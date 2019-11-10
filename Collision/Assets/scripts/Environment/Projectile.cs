using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float restDestroySeconds = 3;

    private Vector3 velocity;
    private float secondTimer = 0;
    private float radius;
    private Vector3 lastFramePosition;
    private Vector3 lastSecondPosition;

    void Awake()
    {
        velocity = new Vector3(0, 0, 0);
        radius = transform.localScale.x / 2;
        lastSecondPosition = transform.position;
    }

    void FixedUpdate()
    {
        lastFramePosition = transform.position;
        //apply position update
        UpdatePosition();

        //apply velocity update
        if (!CheckCollision())
        {
            velocity += EnvironmentManager.Instance.GravityAcc * Time.deltaTime;
        }

        CheckForDestroy();
    }

    public bool CheckCollision()
    {
        Vector3 position = transform.position;

        bool hasCollision = false;
        // check collision to colliders
        foreach(var linePoints in EnvironmentManager.Instance.Colliders)
        {
            for(var i = 0; i < linePoints.Count - 1; i++)
            {
                if(EnvironmentManager.Instance.LinePointCollisionDetection(linePoints[i], linePoints[i + 1], position, radius))
                {
                    //collision response
                    velocity += EnvironmentManager.Instance.CollisionVelocityChange(linePoints[i], linePoints[i + 1], velocity, EnvironmentManager.Instance.ColliderRestitutionCoef);
                    transform.position = lastFramePosition;
                    UpdatePosition();
                    position = transform.position;
                    hasCollision = true;
                }
            }
        }

        return hasCollision;
    }

    private void UpdatePosition()
    {
        lastFramePosition = transform.position;
        if (transform.position.y > EnvironmentManager.Instance.WindStartHeight)
        {
            //apply wind acc
            transform.position += (velocity * Time.deltaTime);
            velocity += EnvironmentManager.Instance.WindStrength * Time.deltaTime;
        }
        else
        {
            transform.position += (velocity * Time.deltaTime);
        }
    }

    public void AddVelocity(Vector3 acc)
    {
        velocity += acc;
    }

    private void CheckForDestroy()
    {
        //Destroy the projectile if it hits the ground or it has been rest for over 1 sec
        if(transform.position.y <= EnvironmentManager.Instance.GroundLevel)
        {
            Destroy(this.gameObject);
        }

        // Destroy if it has been rest for 3 seconds
        secondTimer += Time.deltaTime;
        if(secondTimer > restDestroySeconds)
        {
            if((transform.position - lastSecondPosition).magnitude < 0.1f)
            {
                Destroy(this.gameObject);
            }
            else
            {
                lastSecondPosition = transform.position;
                secondTimer = 0;
            }
        }

        //Destroy if it collides with a ghost
        if (GhostManager.Instance.DoesCollideWithGhosts(transform.position, velocity, radius))
        {
            Destroy(this.gameObject);
        }

    }
}
