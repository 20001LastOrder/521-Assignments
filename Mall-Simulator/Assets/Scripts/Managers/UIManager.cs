using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : ManagerBase<UIManager>
{
    public void OnPlayerSpawnRateChange(float value) {
		ShopperManager.Instance.UpdatePlayerGenerationRate(value);
	}

    public void OnMaxNumberOfAdvertiserChange(float value)
    {
        AdvertiserManager.Instance.MaxNumAdvertisers = (int)value;
    }

    public void OnObservingDistanceChange(float value)
    {
		AdvertiserManager.Instance.ObservingDistance = value;
	}

    public void OnSalePatchDistanceChange(float value)
    {
        AdvertiserManager.Instance.SalePatchDistance = value;
    }

    public void OnAdvertisingRateChange(float value)
    {
        // to avoid numerical error
        if(value < 0.0001)
        {
            value = 0.0001f;
        }
        AdvertiserManager.Instance.AdvertiseFreq = 1 / value;
    }

    public void OnProbabilityChange(float value)
    {
        AdvertiserManager.Instance.AdvertiseProb = value;
    }
}
