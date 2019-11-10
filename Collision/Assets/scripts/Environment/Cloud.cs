using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private Vector3 velocity;

    void Start()
    {
        velocity = new Vector3();

        // Synchronize the Cloud velocity with wind velocity
        EnvironmentManager.Instance.OnWindChange += UpdateVelocity;
    }

    void Update()
    {
        transform.position += velocity * Time.deltaTime;

        // wrapping clouds
        if(transform.position.x > EnvironmentManager.Instance.ScreenMax || transform.position.x < -EnvironmentManager.Instance.ScreenMax)
        {
            Vector3 position = transform.position;
            position.x = -Mathf.Sign(position.x) * EnvironmentManager.Instance.ScreenMax;
            transform.position = position;
        }
    }

    private void UpdateVelocity(Vector3 v)
    {
        velocity = v;
    }
}
