using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBase : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 1;

    private void Update()
    {
        var rotation = _rotationSpeed * Time.deltaTime;
        transform.Rotate(new Vector3(0, _rotationSpeed, 0));
    }
}
