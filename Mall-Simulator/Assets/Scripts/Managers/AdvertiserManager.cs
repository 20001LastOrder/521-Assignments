using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvertiserManager : ManagerBase<AdvertiserManager>
{
	// x position to generate the advertiser
    [SerializeField]
    private float _generateX;

	//Y range to generate the advertiser
    [SerializeField]
    private float _generateMinY;
    [SerializeField]
    private float _generateMaxY;

	//Layer of the advertiser
    [SerializeField]
    private int _advertiserLayer = 12;

	//initial maximum number of advertisers
	[SerializeField]
    private int _maxNumAdvertisers = 3;

	//advertiser observing distance
    [SerializeField]
    private float _observingDistance = 5;

    [SerializeField]
    private GameObject _advertiserPrefab;

    [SerializeField]
    private float _salePatchDistacne = 5;

	// frequency and prob the advertiser advertises
    [SerializeField]
    private float _advertiseFreq = 5;

    [SerializeField]
    private float _advertiseProb = 0.5f;

    private List<Advertiser> _advertisers;
    private List<Flyer> _flyers;

	// list of advertisers and flyers
    public List<Advertiser> Advertisers => _advertisers;
    public List<Flyer> Flyers => _flyers;

    public float ObservingDistance
    {
        set
        {
            _observingDistance = value;
        }
    }

	// update all advertisers when the properties below got updated 
	public float SalePatchDistance {
        get => _salePatchDistacne;
        set
        {
            _salePatchDistacne = value;
			foreach (var advertiser in _advertisers) {
				advertiser.UpdateSalePatchDistance();
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

	// Generate a random starting point to spawn the advertiser
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

	// start a corountine that continuing broadcast the information
	public void BroadcastAdvertiseOpportunity(Shopper targerCustomer) {
		StartCoroutine(BroadcastAdvertiseOpportunityCoroutine(targerCustomer));
	}

	//broadcast the opportunity for 2s
	private IEnumerator BroadcastAdvertiseOpportunityCoroutine(Shopper targerCustomer)
    {
		float counter = 0;

		while(counter < 2f) {
			var pos = targerCustomer.transform.position;
			foreach (var advertiser in _advertisers) {
				if (Vector3.Distance(advertiser.transform.position, pos) <= _observingDistance) {
					advertiser.SendToAdvertiseMission(targerCustomer);
				}
			}
			counter += 0.1f;
			yield return new WaitForSeconds(0.1f);
		}
    }

}
