using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private readonly float speed = 20f;
    [SerializeField]
    private readonly float maxDistance = 100;

    public event Action<Projectile> OnDestroy;
    private Vector3 _initialPosition;

    private void Awake()
    {
        _initialPosition = this.transform.position;
    }

    void Update()
    {
        //move the projective forward
        transform.Translate(speed * Time.deltaTime * Vector3.forward);

        if(Vector3.Distance(this.transform.position, _initialPosition) > maxDistance)
        {
            DestroySelf();
        }
    }

    private void OnCollisionEnter(Collision _)
    {
        DestroySelf();
    }

    private void DestroySelf()
    {
        // invoke the method to update the projectile status of the player
        if (OnDestroy != null)
        {
            OnDestroy.Invoke(this);
        }
        Destroy(this.gameObject);
    }
}
