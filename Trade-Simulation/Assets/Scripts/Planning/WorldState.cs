using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Current world state
public class WorldState : IEquatable<WorldState>
{
    public SpiceVector CaravanStorage;

    public SpiceVector PlayerStorage;

    // the action take for previous state to the current state (only used in planning)
    public Action LastAction;
    // the previous state (only used in planning)
    public WorldState PreviousState;

    // forward cost for the state (only used in planning)
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

    // check if a state reachs this state (only check for caravan storage)
    public bool Reaches(WorldState other)
    {
        return CaravanStorage.Equals(other.CaravanStorage);
    }
}
