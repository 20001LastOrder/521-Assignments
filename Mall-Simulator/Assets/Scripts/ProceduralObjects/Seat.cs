﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : ProceduralObject
{

    // Start is called before the first frame update
    void Awake()
    {
        var currentSize = GetComponent<SpriteRenderer>().bounds.size;
        var scale = transform.localScale;
        scale.x = scale.x * _radius * 2 / (currentSize.x);
        scale.y = scale.y * _radius * 2 / (currentSize.y);
        transform.localScale = scale;
    }
}
