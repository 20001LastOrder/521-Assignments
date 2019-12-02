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
    private int _maxNumAdvertisers = 3;

    [SerializeField]
    private float _salePatchDistance = 5;

    [SerializeField]
    private GameObject _advertiserPrefab;

    [SerializeField]
    private float _flyerObservingDistance = 5;

    [SerializeField]
    private float _advertiseFreq = 5;

    [SerializeField]
    private float _advertiseProb = 0.5f;

    private List<Advertiser> _advertisers;
    private List<Flyer> _flyers;


    public List<Advertiser> Advertisers => _advertisers;
    public List<Flyer> Flyers => _flyers;

    public float SalePatchDistance
    {
        set
        {
            _salePatchDistance = value;
        }
    }

    public float FlyerObservingDistance {
        get => _flyerObservingDistance;
        set
        {
            _flyerObservingDistance = value;
            foreach(var flyer in _flyers)
            {
                flyer.UpdateFlyerObservingDistance();
            }
        }
    }

    public float AdvertiseFreq
    {
        get => _advertiseFreq;
        set
        {
            _advertiseFreq = value;
            foreach(var advertiser in _advertisers)
            {
                advertiser.UpdateAdvertiseFreq();
            }
        }
    }

    public float AdvertiseProb
    {
        get => _advertiseProb;
        set
        {
            _advertiseProb = value;
            foreach (var advertiser in _advertisers)
            {
                advertiser.UpdateAdvertiseProb();
            }
        }
    }

    public int MaxNumAdvertisers
    {
        set
        {
            var currentNumberAdvertisers = _maxNumAdvertisers;
            _maxNumAdvertisers = value;
            if (value >= currentNumberAdvertisers)
            {
                for (var i = 0; i < value - currentNumberAdvertisers; i++)
                {
                    InstantiateAdvertiser();
                }
            }
            else
            {
                for (var i = 0; i < currentNumberAdvertisers - value; i++)
                {
                    _advertisers[0].DestroySelf();
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _flyers = new List<Flyer>();
		_advertisers = new List<Advertiser>();
		for (var i = 0; i < _maxNumAdvertisers; i++) {
			InstantiateAdvertiser();
		}
	}

	private void InstantiateAdvertiser() {
		var advertiser = Instantiate(_advertiserPrefab, GetRandomStartingPoint(), Quaternion.identity).GetComponent<Advertiser>();
		advertiser.transform.parent = transform;
		advertiser.gameObject.layer = _advertiserLayer;
		_advertisers.Add(advertiser);
	}

	public Vector3 GetRandomStartingPoint() {
		return new Vector3(_generateX, Utils.RandomFloat(_generateMinY, _generateMaxY), -1);
	}

    public void RemoveAdvertiser(Advertiser a)
    {
        _advertisers.Remove(a);
        if(_advertisers.Count < _maxNumAdvertisers)
        {
            InstantiateAdvertiser();
        }
    }

    public void BroadcastAdvertiseOpportunity(Shopper targerCustomer)
    {
        var pos = targerCustomer.transform.position;
        foreach(var advertiser in _advertisers)
        {
            if(Vector3.Distance(advertiser.transform.position, pos) <= _salePatchDistance)
            {
                advertiser.SendToAdvertiseMission(targerCustomer);
            }
        }
    }

}
