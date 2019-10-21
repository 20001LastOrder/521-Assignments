using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Constraint
{
    protected int iterationPerFrame;

    protected Constraint(Ghost ghost, int iterationPerFrame)
    {
        SetupConstraint(ghost);
        this.iterationPerFrame = iterationPerFrame;
    }

    protected abstract void SetupConstraint(Ghost ghost);

    public abstract void ResolveConstriant(Ghost ghost, float fixFactor);
}
