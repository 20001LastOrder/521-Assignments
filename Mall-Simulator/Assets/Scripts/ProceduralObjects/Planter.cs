using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planter : ProceduralObject
{
    // Start is called before the first frame update
    void Start()
    {
        var currentSize = GetComponent<SpriteRenderer>().bounds.size;
        var scale = transform.localScale;
        var dimension = Mathf.Sqrt(Mathf.Pow(_radius, 2) / 2);
        scale.x = scale.x * dimension / (currentSize.x);
        scale.y = scale.y * dimension / (currentSize.y);
        transform.localScale = scale;
    }
}
