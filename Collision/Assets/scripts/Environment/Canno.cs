using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canno : MonoBehaviour
{
    //cannon parts
    [SerializeField]
    private Transform cannoBody;

    [SerializeField]
    private Transform cannoWheel;

    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private Transform projectileFirePoint;
    
    [SerializeField]
    private float force = 1;

    private Vector3 position;

    private void Start()
    {
        position = transform.position;
    }

    void Update()
    {
        UpdateRotation();

        //check for fire
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Projectile p = Instantiate(projectilePrefab, projectileFirePoint).GetComponent<Projectile>();

            // remove parent
            p.transform.parent = null;

            //get current angle and calculate each component of the instantanious acceleration
            // minus sign because we calculate the angle wrt negative x direction
            float zRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            //add minus as the ball is supposed to move in negative x direction
            Vector3 acc = new Vector3(-force * Mathf.Cos(zRotation), force * Mathf.Sin(zRotation), 0);
            p.AddVelocity(acc);
        }

        
    }

    private void UpdateRotation()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // want to flip the rotation, -180
        var angle = Mathf.Rad2Deg * Mathf.Atan2(mousePosition.y - position.y, (mousePosition.x - position.x)) - 180;
        // round angle
        if (angle < -180)
        {
            angle = angle + 360;
        }

        // restrict the rotation of the cannon
        if (angle < -90)
        {
            angle = -90;
        }
        else if (angle > 0)
        {
            angle = 0;
        }

        this.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
