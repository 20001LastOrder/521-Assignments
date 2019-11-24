using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : ProceduralObject
{
    [SerializeField]
    private int _seatNumberMax;

    [SerializeField]
    private int _seatNumberMin;

    public static readonly float tableSeatDistance = 0.1f;

    public static readonly float tableTableDistance = 1;

    [SerializeField]
    private GameObject _seatPrefab;

    private float _seatRadius;
    private float _tableRadius;
    private List<Seat> _seats;
    public float TableRadius => _tableRadius;
    public List<Seat> Seats => _seats;

    // Start is called before the first frame update
    void Awake()
    {
        _tableRadius = GetComponent<SpriteRenderer>().bounds.size.x / 2;
        _seatRadius = _seatPrefab.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        _radius = _tableRadius + _seatRadius + tableSeatDistance;
    }

    private void Start()
    {
        var numSeats = Utils.RandomInt(_seatNumberMin, _seatNumberMax + 1);

        //generate Generate Seats
        GenerateSeats(numSeats);
    }

    private void GenerateSeats(int numSeats)
    {
        _seats = new List<Seat>();

        var i = 0;
        while (_seats.Count < numSeats && i < 1000)
        {
            i++;
            var pos = GetRandomSeatDistance(_seatRadius);
            if (IsSatisfySeatSeatConstraint(pos))
            {
                var seat = Instantiate(_seatPrefab, pos, Quaternion.identity).GetComponent<Seat>();
                seat.transform.parent = transform;
                _seats.Add(seat);
            }
        }
        Debug.Log("Table: " + i);
    }

    private Vector3 GetRandomSeatDistance(float seatRadius)
    {
        var centerPos = transform.position;

        // get directional vector of a random direction
        var randomDirection = new Vector3(Utils.RandomFloat(-1, 1), Utils.RandomFloat(-1, 1));
        randomDirection.Normalize();

        return centerPos+(seatRadius + _tableRadius + tableSeatDistance) * randomDirection;
    }

    private bool IsSatisfySeatSeatConstraint(Vector3 pos)
    {
        foreach (var seat in _seats)
        {
            if (!seat.IsPosSatisfyConstraint(pos, typeof(Seat)))
            {
                return false;
            }
        }

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool IsPosSatisfyConstraint(Vector3 position, Type objectType)
    {
        if (objectType == typeof(Table))
        {
            return (transform.position - position).magnitude >= 2 * _radius + tableTableDistance;
        }

        return false;
    }
}
