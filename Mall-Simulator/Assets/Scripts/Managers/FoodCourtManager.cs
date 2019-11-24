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
    private GameObject _tablePrefab;

    [SerializeField]
    private int _tableNumberMax;

    [SerializeField]
    private int _tableNumberMin;

    private List<Table> _tables;

    public List<Table> Tables => _tables;

    public void Start()
    {
        var numTables = Utils.RandomInt(_tableNumberMin, _tableNumberMax + 1);
        GenerateTables(numTables);
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
        Debug.Log("Food: "+ i);
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
            pos += new Vector3(2*_tables.Last().Radius + Table.tableTableDistance + 2*Utils.RandomFloat(), 0, 0);
            pos.y = Utils.RandomFloat(-_foodCourtHeight, _foodCourtHeight);
            return pos;
        }
    }

    private bool IsSatisfyTableTableConstraint(Vector3 pos)
    {
        foreach (var table in _tables)
        {
            if (!table.IsPosSatisfyConstraint(pos, typeof(Table)))
            {
                return false;
            }
        }

        return true;
    }

}
