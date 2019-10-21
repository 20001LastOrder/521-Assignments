using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private Vector3 velocity;


    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector3();
        EnvironmentManager.Instance.OnWindChange += UpdateVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;

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
