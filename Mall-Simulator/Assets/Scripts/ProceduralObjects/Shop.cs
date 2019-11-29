using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : CircularObject
{
    [SerializeField]
    private Transform _upperRightCorner;

    [SerializeField]
    private Transform _lowerLeftCorner;

    public Vector3 UpperRightCorner => _upperRightCorner.position;

    public Vector3 LowerLeftCorner => _lowerLeftCorner.position;

}
