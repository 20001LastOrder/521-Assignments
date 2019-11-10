using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : ManagerBase<EnvironmentManager>
{
    [SerializeField]
    private Vector3 gravityAcc = new Vector3(0, -1, 0);

    [SerializeField]
    private GameObject ground;

    [SerializeField]
    private float maxWind;

    [SerializeField]
    private float windStartHeight = 0;

    [SerializeField]
    private float screenMax = 12;

    [SerializeField]
    private float colliderRestitutionCoef = 0.6f;

    [SerializeField]
    private Vector3 upperRightCorner;

    [SerializeField]
    private Vector3 lowerLeftCorner;

    private Vector3 windStrength;

    private float groundLevel;

    //set it to 2 so that the wind will be initialize in the first frame
    private float elapsedSeconds = 2;

    public event System.Action<Vector3> OnWindChange;

    private List<List<Vector3>> colliders;

    public Vector3 GravityAcc => gravityAcc;

    public Vector3 WindStrength => windStrength;

    public float WindStartHeight => windStartHeight;

    public float GroundLevel => groundLevel;

    public float ScreenMax => screenMax;

    public List<List<Vector3>> Colliders => colliders;

    public float ColliderRestitutionCoef => colliderRestitutionCoef;

    public Vector3 UpperRightCorner => upperRightCorner;

    public Vector3 LowerLeftCorner => lowerLeftCorner;


    void Start()
    {

        groundLevel = ground.transform.position.y;
        windStrength = new Vector3();
        colliders = new List<List<Vector3>>();
    }

    void Update()
    {
        elapsedSeconds += Time.deltaTime;

        //wind change every 2 seconds
        if(elapsedSeconds >= 2.0f)
        {
            windStrength.x = Random.Range(-maxWind, maxWind);
            if(OnWindChange != null)
            {
                OnWindChange.Invoke(windStrength);
            }
            elapsedSeconds = 0;

        }
    }

    public void RegisterCollider(List<Vector3> r)
    {
        colliders.Add(r);
    }


    // Check line point collision
    public bool LinePointCollisionDetection(Vector3 start, Vector3 end, Vector3 point, float radius)
    {
        Vector3 line = end - start;

        Vector3 endToPoint = end - point;
        float angle = Vector3.Angle(line, endToPoint) * Mathf.Deg2Rad;
        float parallelDistance = Vector3.Project(endToPoint, line).magnitude;

        // if the point is beyound the end of the line, there is no collision
        if (angle > 0.5 * Mathf.PI && (Vector3.Project(endToPoint, line).magnitude > radius))
        {
            return false;
        }

        float perpendicularDistance = endToPoint.magnitude * Mathf.Sin(angle);

        // if point is to far away, or is beyond the start of the line, there is no collision
        if (perpendicularDistance < radius && parallelDistance - line.magnitude <= radius)
        {
            return true;
        }

        return false;
    }

    // Calculate the Velocity change for if there is a collision (make sure there is a collision before call this method)
    public Vector3 CollisionVelocityChange(Vector3 start, Vector3 end, Vector3 velocity, float restitutionCoef)
    {
        Vector3 line = end - start;

        Vector3 parallelComponent = Vector3.Project(velocity, line);
        Vector3 perpendicularComponent = parallelComponent - velocity;

        // change the velocity based on the perpendicular component
        return (1 + restitutionCoef) * (perpendicularComponent);
        
    }

    // line line intersection with method from the lecture
    public bool LineLineIntersection(Vector3 a0, Vector3 a1, Vector3 b0, Vector3 b1)
    {
        Vector3 alpha = a1 - a0;
        Vector3 beta0 = a1 - b0;
        Vector3 beta1 = a1 - b1;

        if(Mathf.Sign(Vector3.Cross(alpha, beta0).z) == Mathf.Sign(Vector3.Cross(alpha, beta1).z)){
            return false;
        }

        Vector3 beta = b1 - b0;
        Vector3 alpha0 = b1 - a0;
        Vector3 alpha1 = b1 - a1;

        if (Mathf.Sign(Vector3.Cross(beta, alpha0).z) == Mathf.Sign(Vector3.Cross(beta, alpha1).z)){
            return false;
        }

        return true;
    }

    // check if a (Verlet) point is in a bad place (collision or the ground)
    public bool InBadArea(Vector3 lastPosition, Vector3 position, float radius)
    {
        if(position.y <= groundLevel)
        {
            return true;
        }
        else
        {
            foreach (var linePoints in colliders)
            {
                for (var i = 0; i < linePoints.Count - 1; i++)
                {
                    //CollisionToCollider(linePoints[i], linePoints[i + 1]);
                    if (EnvironmentManager.Instance.LineLineIntersection(linePoints[i], linePoints[i + 1], lastPosition, position))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
