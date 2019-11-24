using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manage Generation of traders
public class TraderManager : ManagerBase<TraderManager>
{
    [SerializeField]
    private List<SpiceVector> _tradersInfomation = new List<SpiceVector>() { new SpiceVector(), new SpiceVector(), new SpiceVector(), new SpiceVector(), new SpiceVector(), new SpiceVector(), new SpiceVector(), new SpiceVector() };

    [SerializeField]
    private List<Trader> _traders;

    public List<Trader> Traders => _traders;

    protected override void Awake()
    {
        base.Awake();
        var r = new System.Random();

        var tem_traders = new List<Trader>(_traders);

        // random assign id and duties to each trader
        var spiceIndex = 0;
        while(tem_traders.Count > 0)
        {
            var index = r.Next(tem_traders.Count);
            var trader = tem_traders[index];
            trader.TradeRule = _tradersInfomation[spiceIndex];
            trader.SetText((spiceIndex+1).ToString());
            tem_traders.RemoveAt(index);
            spiceIndex += 1;
        }
    }
}

