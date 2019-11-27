using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class FoodCourtManager : ManagerBase<FoodCourtManager>
{
    [SerializeField]
    private float _foodCourtWidth;

    [SerializeField]
    private float _foodCourtHeight;

    [SerializeField]
    private int _tableNumberMax;

    [SerializeField]
    private int _tableNumberMin;

    [SerializeField]
    private int _planterNumberMax;

    [SerializeField]
    private int _planterNumberMin;

    [SerializeField]
    private float _tableTableDistance;

    [SerializeField]
    private float _tablePlanterDistance;

    [SerializeField]
    private float _planterPlanterDistance;

    [SerializeField]
    private GameObject _tablePrefab;

    [SerializeField]
    private GameObject _planterPrefab;

    private List<Table> _tables;
    private List<Planter> _planters;
    private List<Seat> _availableSeats;
    private float _planterRadius;

    public List<Table> Tables => _tables;
    public List<Planter> Planters => _planters;

    public Seat GetRandomAvailableSeat()
    {
        if(_availableSeats.Count == 0)
        {
            return null;
        }
        var index = Utils.RandomInt(0, _availableSeats.Count);
        Debug.Log(Utils.RandomInt(0, _availableSeats.Count));
        return _availableSeats[index];
    }

    public void RegisterAvailableSeat(Seat seat)
    {
        _availableSeats.Add(seat);
    }

    public void DeregisterAvailableSeat(Seat seat)
    {
        _availableSeats.Remove(seat);
    }

    private void Start()
    {
        _availableSeats = new List<Seat>();
        var numTables = Utils.RandomInt(_tableNumberMin, _tableNumberMax + 1);
        var numPlanters = Utils.RandomInt(_planterNumberMin, _planterNumberMax + 1);
        _planterRadius = _planterPrefab.GetComponent<Planter>().Radius;
        GenerateTables(numTables);
        GeneratePlanters(numPlanters);
    }

    private void GenerateTables(int numTables)
    {
        _tables = new List<Table>();

        var i = 0;
        while (_tables.Count < numTables && i < 5000)
        {
            i++;
            var pos = GenerateRandomTableLocation();
            if (IsSatisfyTableTableConstraint(pos))
            {
                var table = Instantiate(_tablePrefab, pos, Quaternion.identity).GetComponent<Table>();
                table.transform.parent = transform;
                _tables.Add(table);
            }
        }
    }

    private void GeneratePlanters(int numPlanters)
    {
        _planters = new List<Planter>();

        var i = 0;
        while (_planters.Count < numPlanters && i < 5000)
        {
            i++;
            var randomIndex = Utils.RandomInt(0, _tables.Count);
            var pos = GenerateRandomPlanterLocation(_tables[randomIndex]);
            if (IsSatisfyTablePlanterConstraint(pos) && IsSatisfyPlanterPlanterConstraint(pos))
            {
                var plant = Instantiate(_planterPrefab, pos, Quaternion.identity).GetComponent<Planter>();
                plant.transform.parent = transform;
                _planters.Add(plant);
            }
        }
    }

    private Vector3 GenerateRandomTableLocation()
    {
        if(_tables.Count == 0)
        {
            return new Vector3(-_foodCourtWidth, Utils.RandomFloat(-_foodCourtHeight, _foodCourtHeight), -1);
        }
        else
        {
            var pos = _tables.Last().transform.position;
            pos += new Vector3(2*_tables.Last().FullRadius + _tableTableDistance + _tableTableDistance * Utils.RandomFloat(), 0, 0);
            pos.y = Utils.RandomFloat(-_foodCourtHeight, _foodCourtHeight);
            return pos;
        }
    }

    private Vector3 GenerateRandomPlanterLocation(Table table)
    {
        var centerPos = table.transform.position;

        // get directional vector of a random direction
        var randomDirection = new Vector3(Utils.RandomFloat(-1, 1), Utils.RandomFloat(-1, 1));
        randomDirection.Normalize();
        var noise = _tablePlanterDistance * Utils.RandomFloat();

        return centerPos + (_planterRadius + table.FullRadius + _tablePlanterDistance + noise) * randomDirection;
    }

    private bool IsSatisfyTableTableConstraint(Vector3 pos)
    {
        return _tables.TrueForAll(table => CheckTableTableConstraint(table, pos));
    }

    private bool IsSatisfyTablePlanterConstraint(Vector3 pos)
    {
        return _tables.TrueForAll(table => CheckTablePlanterConstraint(table, pos));
    }

    private bool IsSatisfyPlanterPlanterConstraint(Vector3 pos)
    {
        return _planters.TrueForAll(planter => CheckPlanterPlanterConstraint(planter, pos));
    }

    private bool CheckTablePlanterConstraint(Table table, Vector3 position)
    {
        return (table.transform.position - position).magnitude >= table.FullRadius + _planterRadius + _tablePlanterDistance;
    }

    private bool CheckPlanterPlanterConstraint(Planter planter, Vector3 position)
    {
        return (planter.transform.position - position).magnitude >= planter.Radius + _planterRadius + _planterPlanterDistance;
    }

    private bool CheckTableTableConstraint(Table table, Vector3 position)
    {
        return (table.transform.position - position).magnitude >= 2 * table.FullRadius + _tableTableDistance;
    }


}
