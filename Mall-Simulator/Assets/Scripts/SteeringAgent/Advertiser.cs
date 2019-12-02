using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Advertiser : SteeringAgent {
	[SerializeField]
	private float _advertiserDislikeDistance;

	[SerializeField]
	private float _wanderTargetXMin = 16;
	[SerializeField]
	private float _wanderTargetXMax = 17;
	[SerializeField]
	private float _wanderTargetChangeTime = 5;
	[SerializeField]
	private float _wanderTargetYMax= 8;



    [SerializeField]
    private int _salesTargetNumber = 3;

    [SerializeField]
    private float _saleDistance = 3;

    [SerializeField]
    private float _saleTime = 4;

    [SerializeField]
    private float _salePatienceTime = 5;

    [SerializeField]
    private SpriteRenderer _render;

	[SerializeField]
	private GameObject _flyerPrefab;

	private float _counter;
	private float _advertiseCounter;
    private float _salesDelivered;
    private SteeringAgent _pursueTarget;
    private float _advertiseFreq = 5;
    private float _advertiseProb = 0.5f;

    public float AdvertiserFreq
    {
        set
        {
            _advertiseFreq = value;
        }
    }

    public float AdvertiseProb
    {
        set
        {
            _advertiseProb = value;
        }
    }

    protected override void Start()
    {
        base.Start();
        _state = AdvertiserState.Wandering;
        _seekingTarget = GetNewSeekTarget();
        UpdateAdvertiseFreq();
        UpdateAdvertiseProb();
    }

    public void UpdateAdvertiseFreq()
    {
        _advertiseFreq = AdvertiserManager.Instance.AdvertiseFreq;
    }

    public void UpdateAdvertiseProb()
    {
        _advertiseProb = AdvertiserManager.Instance.AdvertiseProb;
    }

    private enum AdvertiserState {
		Wandering,
        Advertise
	}
	private AdvertiserState _state;

	protected override void ActionSelection() {
		switch (_state) {
			case AdvertiserState.Wandering:
				_counter += Time.deltaTime; 
				if(_counter > _wanderTargetChangeTime) {
					_counter = 0;
					_seekingTarget = GetNewSeekTarget();
				}

				_advertiseCounter += Time.deltaTime;
				if(_advertiseCounter > _advertiseFreq) {
					_advertiseCounter = 0;
					CheckToPutFlyer();
				}

				break;
            case AdvertiserState.Advertise:
                _counter += Time.deltaTime;
                if(_pursueTarget == null)
                {
                    TransitToWandering();
                }
                if(Vector3.Distance(transform.position, _pursueTarget.transform.position) < _saleDistance)
                {
                    _advertiseCounter += Time.deltaTime;
                }
                else
                {
                    _advertiseCounter = 0;
                }

                if(_advertiseCounter > _saleTime)
                {
                    DeliverSale();
                    TransitToWandering();
                }

                if(_counter > _salePatienceTime)
                {
                    TransitToWandering();
                }
                break;
		}
	}

	private void CheckToPutFlyer() {
		var shouldAdvertise = Utils.RandomFloat() < _advertiseProb;
		if (shouldAdvertise) {
			Flyer flyer = Instantiate(_flyerPrefab, transform.position, Quaternion.identity).GetComponent<Flyer>();
            AdvertiserManager.Instance.Flyers.Add(flyer);
		}
	}

	protected override void Steering() {

		switch (_state) {
			case AdvertiserState.Wandering:
				SteeringManager.Instance.Seek(this);
				foreach(var a in AdvertiserManager.Instance.Advertisers) {
					if(Vector3.Distance(transform.position, a.transform.position) < _advertiserDislikeDistance && a != this) {
						SteeringManager.Instance.Flee(this, a);
					}
				}
				SteeringManager.Instance.ObstacleAvoidance(this);
				break;
            case AdvertiserState.Advertise:
                SteeringManager.Instance.Pursue(this, _pursueTarget);
                SteeringManager.Instance.ObstacleAvoidance(this);
                break;
        }
	}

    public void SendToAdvertiseMission(Shopper targer)
    {
        if(_state == AdvertiserState.Advertise)
        {
            return;
        }
        _counter = 0;
        _advertiseCounter = 0;
        _state = AdvertiserState.Advertise;
        _render.color = Color.red;
        _pursueTarget = targer;
    }

    public void DeliverSale()
    {
        _salesDelivered += 1;
        Debug.Log(_salesDelivered);
        if(_salesDelivered >= _salesTargetNumber)
        {
            //remove this from advertiser list.
            DestroySelf();
        }
    }

    public void DestroySelf()
    {
        AdvertiserManager.Instance.RemoveAdvertiser(this);
        Destroy(gameObject);
    }

    private void TransitToWandering()
    {
        _counter = 0;
        _advertiseCounter = 0;
        _render.color = Color.blue;
        _seekingTarget = GetNewSeekTarget();
        _state = AdvertiserState.Wandering;
    }

    private Vector3 GetNewSeekTarget() {
		// Decide whether we set target on the left or on the right
		var multiplier = Utils.RandomFloat() > 0.5 ? -1 : 1;
		var x = multiplier * Utils.RandomFloat(_wanderTargetXMin, _wanderTargetXMax);
		var y = Utils.RandomFloat(-_wanderTargetYMax, _wanderTargetYMax);
		var destination =  new Vector3(x, y, -1);
		return destination;
	}

}
