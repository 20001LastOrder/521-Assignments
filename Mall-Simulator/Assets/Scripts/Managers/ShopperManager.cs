using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopperManager : ManagerBase<ShopperManager>
{
    [SerializeField]
    private float _startX;
    [SerializeField]
    private float _endX;
    [SerializeField]
    private float _maxY;
    [SerializeField]
    private float _minY;

    [SerializeField]
    private float updateFrequency = 20;

    [SerializeField]
    private GameObject _shopperPrefab;

    private List<Shopper> _shoppers;

    private float counter = 20;

    void Start()
    {
		_shoppers = new List<Shopper>();
		counter = 0;
		for (var i = 0; i < 100; i++) {
			InstantiateShopper();
		}
	}

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if(counter > updateFrequency)
        {
            counter = 0;
            for (var i = 0; i < 10; i++)
            {
                //InstantiateShopper();
            }
        }
	}

    public void DeregisterShopper(Shopper shopper)
    {
        _shoppers.Remove(shopper);
    }

    private void InstantiateShopper()
    {
        var shopper = Instantiate(_shopperPrefab, GetRandomStartingPoint(), Quaternion.identity).GetComponent<Shopper>();
        shopper.transform.parent = transform;
        _shoppers.Add(shopper);
    }

    public Vector3 GetDestination(Vector3 currentPosition)
    {
        return new Vector3(_endX, currentPosition.y, -1);
    }

    public Vector3 GetRandomStartingPoint()
    {
        return new Vector3(_startX, Utils.RandomFloat(_minY, _maxY), -1);
    }
}
