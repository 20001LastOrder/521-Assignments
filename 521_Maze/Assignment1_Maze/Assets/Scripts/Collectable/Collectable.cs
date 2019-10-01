using System;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    private int _projectileLayer = 10;

    [SerializeField]
    private float _rotationSpeed = 1;

    // delegate when destroying, decreasing the counter etc.
    public event Action<GameObject> OnDestroy;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == _projectileLayer && OnDestroy != null)
        {
            OnDestroy.Invoke(gameObject);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        var rotation = _rotationSpeed * Time.deltaTime;
        transform.Rotate(new Vector3(0, _rotationSpeed, 0));
    }

}
