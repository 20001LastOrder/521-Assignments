using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : CircularObject
{
    [SerializeField]
    private int _seatNumberMax;

    [SerializeField]
    private int _seatNumberMin;

    [SerializeField]
    private float _seatSeatDistance;
    private float _tableRadius;

    [SerializeField]
    private float _tableSeatDistance = 0.1f;

    [SerializeField]
    private GameObject _seatPrefab;

    private float _seatRadius;

    private List<Seat> _seats;
    public float FullRadius => _tableRadius;
    public List<Seat> Seats => _seats;

    // Start is called before the first frame update
    protected void Awake()
    {
        var currentSize = GetComponent<SpriteRenderer>().bounds.size;
        var scale = transform.localScale;
        scale.x = scale.x * _radius * 2 / (currentSize.x);
        scale.y = scale.y * _radius * 2 / (currentSize.y);
        transform.localScale = scale;

        _seatRadius = _seatPrefab.GetComponent<Seat>().Radius;
        _tableRadius = _radius + 2*_seatRadius + _tableSeatDistance;
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
    }

    private Vector3 GetRandomSeatDistance(float seatRadius)
    {
        var centerPos = transform.position;

        // get directional vector of a random direction
        var randomDirection = new Vector3(Utils.RandomFloat(-1, 1), Utils.RandomFloat(-1, 1));
        randomDirection.Normalize();

        return centerPos+(seatRadius + _radius + _tableSeatDistance) * randomDirection;
    }

    private bool IsSatisfySeatSeatConstraint(Vector3 pos)
    {
        foreach (var seat in _seats)
        {
            if (!CheckSeatSeatConstraint(seat, pos))
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckSeatSeatConstraint(Seat seat, Vector3 position)
    {
        return (seat.transform.position - position).magnitude >= 2 * seat.Radius + _seatSeatDistance;
    }
}
