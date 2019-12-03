using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopperManager : ManagerBase<ShopperManager>
{
	// Information to generate shoppers
    [SerializeField]
    private float _startX;
    [SerializeField]
    private float _endX;
    [SerializeField]
    private float _maxY;
    [SerializeField]
    private float _minY;

	// Shopper generation frequency
    [SerializeField]
    private float _updateFrequency = 20;

	// number of players generated at once
	[SerializeField]
	private float _playerGenerationRate;
	
    [SerializeField]
    private GameObject _shopperPrefab;

	// layer of shoppers
	[SerializeField]
	private int _shopperLayer = 13;

    private List<Shopper> _shoppers;

    private float counter = 20;

    void Start()
    {
		_shoppers = new List<Shopper>();
		counter = _updateFrequency;
	}

    // Update is called once per frame
    void Update()
    {
		// count and generate new shoppers
        counter += Time.deltaTime;
        if(counter > _updateFrequency)
        {
            counter = 0;
            for (var i = 0; i < _playerGenerationRate; i++)
            {
                InstantiateShopper();
            }
        }
	}

	// deregister shopper from shopper list
    public void DeregisterShopper(Shopper shopper)
    {
        _shoppers.Remove(shopper);
    }

    private void InstantiateShopper()
    {
        var shopper = Instantiate(_shopperPrefab, GetRandomStartingPoint(), Quaternion.identity).GetComponent<Shopper>();
        shopper.transform.parent = transform;
		shopper.gameObject.layer = _shopperLayer;
		_shoppers.Add(shopper);
    }

	//create random starting point for a shopper
    public Vector3 GetRandomStartingPoint()
    {
        return new Vector3(_startX, Utils.RandomFloat(_minY, _maxY), -1);
    }

	public void UpdatePlayerGenerationRate(float rate) {
		_playerGenerationRate = (int)rate;
	}
}
