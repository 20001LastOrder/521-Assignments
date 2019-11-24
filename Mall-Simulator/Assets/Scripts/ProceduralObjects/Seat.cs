using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : ProceduralObject
{
    [SerializeField]
    private float _seatSeatDistance;

    // Start is called before the first frame update
    void Awake()
    {
        _radius = GetComponent<SpriteRenderer>().bounds.size.x / 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool IsPosSatisfyConstraint(Vector3 position, System.Type objectType)
    {
        if(objectType == typeof(Seat))
        {
            return (transform.position - position).magnitude >= 2 * _radius + _seatSeatDistance;
        }

        return false;
    }
}
