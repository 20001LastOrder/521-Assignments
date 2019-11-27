using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : CircularObject
{
    private bool _available = true;

    public bool IsAvailable {
        get => _available;
        set {
            if(!value && _available)
            {
                FoodCourtManager.Instance.DeregisterAvailableSeat(this);
            }else if (value && !_available)
            {
                FoodCourtManager.Instance.RegisterAvailableSeat(this);
            }
            _available = value;
        }
    }

    // Start is called before the first frame update
    protected override void  Awake()
    {
        base.Awake();
        ObstacleManager.Instance.DeregisterObstacle(this);
        var currentSize = GetComponent<SpriteRenderer>().bounds.size;
        var scale = transform.localScale;
        scale.x = scale.x * _radius * 2 / (currentSize.x);
        scale.y = scale.y * _radius * 2 / (currentSize.y);
        transform.localScale = scale;
        FoodCourtManager.Instance.RegisterAvailableSeat(this);
    }
}
