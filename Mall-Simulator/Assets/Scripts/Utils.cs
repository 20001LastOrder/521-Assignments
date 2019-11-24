using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// Utility Methods
public static class Utils
{
    private static System.Random r = new System.Random();

    public static float RandomFloat()
    {
        return (float)r.NextDouble();
    }

    public static float RandomFloat(float min, float max)
    {
        return (float)(min + (max-min) * r.NextDouble());
    }

    public static int RandomInt()
    {
        return r.Next();
    }

    public static int RandomInt(int min, int max)
    {
        return r.Next(min, max);
    }

    public static Vector3 FindMax(this List<Vector3> list, Func<Vector3, float> comparator)
    {
        Vector3 max = list[0];

        for(var i = 1; i < list.Count; i++)
        {
            if(comparator(max) < comparator(list[i]))
            {
                max = list[i];
            }
        }

        return max;
    }
}
