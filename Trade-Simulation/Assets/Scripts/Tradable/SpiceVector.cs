using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Spice vector storing the 7 spices with some basic operations for vector (add, minus, etc)
[Serializable]
public class SpiceVector : IEquatable<SpiceVector>
{
    public static readonly string[] SpiceNames = { "Tu", "Sa", "Ca", "Ci", "Cl", "Pe", "Su" };

    // Init Spices with default name and Order
    public List<int> Spices = new List<int>(7);

    public SpiceVector()
    {
        Spices = new List<int>(7) { 0,0,0,0,0,0,0};
    }
    
    public void Add(SpiceVector other)
    {
        for(var i = 0; i < Spices.Count; i++)
        {
            Spices[i] += other.Spices[i];
        }
    }

    public void Minus(SpiceVector other)
    {
        for (var i = 0; i < Spices.Count; i++)
        {
            Spices[i] -= other.Spices[i];
        }
    }

    public SpiceVector Clone()
    {
        var cl = new SpiceVector();
        for (var i = 0; i < Spices.Count; i++)
        {
            cl.Spices[i] = Spices[i];
        }
        return cl;
    }

    public int Sum()
    {
        var sum = 0;
        for (var i = 0; i < Spices.Count; i++)
        {
            sum += Spices[i];
        }
        return sum;
    }

    public bool Equals(SpiceVector other)
    {
        for (var i = 0; i < Spices.Count; i++)
        {
            if(Spices[i] != other.Spices[i])
            {
                return false;
            }
        }
        return true;
    }
}