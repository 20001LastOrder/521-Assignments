using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpiceTransferPoint : MonoBehaviour
{
    [SerializeField]
    private Transform _tradePoint;

    public Transform TradePoint
    {
        get => _tradePoint;
        protected set => _tradePoint = value;
    }

    public SpiceVector TradeRule
    {
        get;
        set;
    }

    public abstract SpiceVector Transfer(SpiceVector deal, SpiceVector storage);

    public abstract bool IsDealAcceptable(SpiceVector deal);
}
