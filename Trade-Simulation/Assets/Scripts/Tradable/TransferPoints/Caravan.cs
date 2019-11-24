using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Caravan gives / take what specified in TradeRule
public class Caravan : SpiceTransferPoint
{
    public SpiceVector Storage;

    void Awake()
    {
        Storage = new SpiceVector();
    }

    // check if the caravan storage can satisfy the deal
    public override bool IsDealAcceptable(SpiceVector deal)
    {
        for(var i = 0; i < deal.Spices.Count; i++)
        {
            if(Storage.Spices[i] < deal.Spices[i])
            {
                return false;
            }
        }

        return true;
    }

    // transfer spices
    public override SpiceVector Transfer(SpiceVector deal, SpiceVector storage)
    {
        if (!IsDealAcceptable(deal))
        {
            throw new System.Exception("No Enough Storage");
        }

        Storage.Minus(deal);
        storage.Add(deal);
        return storage;
    }
}
