using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// abstract class for any objects that can transfer spices
public abstract class SpiceTransferPoint : MonoBehaviour
{
    // trade location
    [SerializeField]
    private Transform _tradePoint;

    public Transform TradePoint
    {
        get => _tradePoint;
        protected set => _tradePoint = value;
    }

    // spice trade rule
    public SpiceVector TradeRule
    {
        get;
        set;
    }

    public abstract SpiceVector Transfer(SpiceVector deal, SpiceVector storage);

    public abstract bool IsDealAcceptable(SpiceVector deal);
}
