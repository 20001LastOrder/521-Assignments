using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvertiserManager : ManagerBase<AdvertiserManager>
{
	[SerializeField]
	private float _generateX;

	[SerializeField]
	private float _generateMinY;
	[SerializeField]
	private float _generateMaxY;
	[SerializeField]
	private int _advertiserLayer = 12;

	[SerializeField]
	private GameObject _advertiserPrefab;

	private List<Advertiser> _advertisers;
	public List<Advertiser> Advertisers => _advertisers;

    // Start is called before the first frame update
    void Start()
    {
		_advertisers = new List<Advertiser>();
		for (var i = 0; i < 3; i++) {
			InstantiateShopper();
		}
	}

	private void InstantiateShopper() {
		var advertiser = Instantiate(_advertiserPrefab, GetRandomStartingPoint(), Quaternion.identity).GetComponent<Advertiser>();
		advertiser.transform.parent = transform;
		advertiser.gameObject.layer = _advertiserLayer;
		_advertisers.Add(advertiser);
	}

	public Vector3 GetRandomStartingPoint() {
		return new Vector3(_generateX, Utils.RandomFloat(_generateMinY, _generateMaxY), -1);
	}

}
