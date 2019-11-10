using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : IEquatable<WorldState>
{
    public SpiceVector CaravanStorage;

    public SpiceVector PlayerStorage;

    public Action LastAction;

    public WorldState PreviousState;

    public int steps = 0;

    public WorldState(SpiceVector caravanStorage, SpiceVector playerStorage)
    {
        CaravanStorage = caravanStorage;
        PlayerStorage = playerStorage;
    }

    public WorldState Clone()
    {
        return new WorldState(CaravanStorage.Clone(), PlayerStorage.Clone()); ;
    }

    public override int GetHashCode()
    {
        return CaravanStorage.Sum();
    }

    public override bool Equals(object obj)
    {
        return obj is WorldState && Equals((WorldState)obj);
    }


    public bool Equals(WorldState other)
    {
        return Reaches(other) && PlayerStorage.Equals(other.PlayerStorage);
    }

    public bool Reaches(WorldState other)
    {
        return CaravanStorage.Equals(other.CaravanStorage);
    }
}
