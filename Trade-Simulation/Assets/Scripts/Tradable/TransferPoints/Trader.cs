using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trader : SpiceTransferPoint
{
    [SerializeField]
    private TextMesh _text;
    
    // The deal must be exactly as the tradeRule
    public override bool IsDealAcceptable(SpiceVector deal)
    {
        for(var i = 0; i < deal.Spices.Count; i++)
        {
            if(deal.Spices[i] != TradeRule.Spices[i])
            {
                return false;
            }
        }
        return true;
    }

    public override SpiceVector Transfer(SpiceVector deal, SpiceVector storage)
    {
        if (!IsDealAcceptable(deal))
        {
            throw new System.Exception("Deal not accepted by the trader");
        }
        storage.Add(TradeRule);
        return storage;
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public string Name => _text.text;

    public SpiceVector RequiredSpices()
    {
        var getVector = TradeRule.Clone();
        for (var i = 0; i < getVector.Spices.Count; i++)
        {
            if (getVector.Spices[i] >= 0)
            {
                getVector.Spices[i] = 0;
            }
            else
            {
                getVector.Spices[i] *= -1;
            }
        }
        return getVector;
    }
}
