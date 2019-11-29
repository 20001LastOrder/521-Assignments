using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : ManagerBase<UIManager>
{
    public void OnPlayerSpawnRateChange(float value) {
		ShopperManager.Instance.UpdatePlayerGenerationRate(value);
	}
}
