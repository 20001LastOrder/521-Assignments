using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinTester : MonoBehaviour
{
    [SerializeField]
    private LineRenderer line;

    [SerializeField]
    private int octaves = 1;

    public void Start()
    {
        PerlinNoiseGenerator p = new PerlinNoiseGenerator(0, 5, octaves, 0, 1);

        line.positionCount = 30;
        var increament = 5.0f / 30;
        var positions = new List<Vector3>();
        for(var i = 0; i < 30; i++)
        {
            var x = i * increament;
            var y = p.GetNoiseValue(x);
            positions.Add(new Vector3(x, y, 0));
        }

        line.SetPositions(positions.ToArray());
    }
}
