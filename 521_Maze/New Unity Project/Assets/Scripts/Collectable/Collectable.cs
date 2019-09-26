using System;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : CollectableBase
{
    [SerializeField]
    private int _projectileLayer = 10;

    public event Action<GameObject> OnDestroy;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == _projectileLayer && OnDestroy != null)
        {
            OnDestroy.Invoke(gameObject);
            Destroy(gameObject);
        }
    }
}
