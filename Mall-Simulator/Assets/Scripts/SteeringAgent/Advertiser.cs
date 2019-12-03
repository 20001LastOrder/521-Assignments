using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Advertiser : SteeringAgent {


	[SerializeField]
	private float _advertiserDislikeDistance;

	// info used to randomly pick seek target
	[SerializeField]
	private float _wanderTargetXMin = 16;
	[SerializeField]
	private float _wanderTargetXMax = 17;
	[SerializeField]
	private float _wanderTargetChangeTime = 2;
	[SerializeField]
	private float _wanderTargetYMax= 8;

    [SerializeField]
    private int _salesTargetNumber = 3;

	// time used to make a sale success
    [SerializeField]
    private float _saleTime = 4;

	// time wait to sale smt
    [SerializeField]
    private float _salePatienceTime = 5;

    [SerializeField]
    private SpriteRenderer _render;
	[SerializeField]
	private GameObject _flyerPrefab;

	// counter used to general time count
	private float _counter;
	// counter specific for advertise
	private float _advertiseCounter;
    private int _salesDelivered;

	// persuit target (a shopper)
    private SteeringAgent _pursueTarget;
    private float _advertiseFreq = 5;
	private float _salePatchDistance = 3;
	private float _advertiseProb = 0.5f;
    private Color[] _advertiserColors = { Color.gray, Color.blue, Color.yellow};
 

    protected override void Start()
    {
        base.Start();
        _state = AdvertiserState.Wandering;
        _seekingTarget = GetNewSeekTarget();
        UpdateAdvertiseFreq();
        UpdateAdvertiseProb();
		UpdateSalePatchDistance();
        _render.color = _advertiserColors[_salesDelivered];
        this._velocity = new Vector3(-_maxSpeed, 0, 0);
    }

    public void UpdateAdvertiseFreq()
    {
        _advertiseFreq = AdvertiserManager.Instance.AdvertiseFreq;
    }

    public void UpdateAdvertiseProb()
    {
        _advertiseProb = AdvertiserManager.Instance.AdvertiseProb;
    }

	public void UpdateSalePatchDistance() {
		_salePatchDistance = AdvertiserManager.Instance.SalePatchDistance;
	}

    private enum AdvertiserState {
		Wandering,
        Advertise
	}
	private AdvertiserState _state;

	// action selection based on state
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
					return;
                }
                if(Vector3.Distance(transform.position, _pursueTarget.transform.position) < _salePatchDistance)
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
		var shouldAdvertise = Utils.RandomFloat() < _advertiseProb && (transform.position.y <= ShopManager.Instance.ShopExitY && transform.position.y >= -ShopManager.Instance.ShopExitY);
		if (shouldAdvertise) {
			Flyer flyer = Instantiate(_flyerPrefab, transform.position, Quaternion.identity).GetComponent<Flyer>();
            AdvertiserManager.Instance.Flyers.Add(flyer);
		}
	}

	// steering behaviour
	protected override void Steering() {

		switch (_state) {
			case AdvertiserState.Wandering:
                SteeringManager.Instance.Wandering(this, 0.01f);
                SteeringManager.Instance.Seek(this);
                foreach (var a in AdvertiserManager.Instance.Advertisers) {
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

	//send to pursuit a customer and try to sale
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
        if (_salesDelivered >= _salesTargetNumber)
        {
            DestroySelf();
        }
        else
        {
            _render.color = _advertiserColors[_salesDelivered];
        }
    }

    public void DestroySelf()
    {
		//remove this from advertiser list.
		AdvertiserManager.Instance.RemoveAdvertiser(this);
        Destroy(gameObject);
    }

	// transit back to wandering
    private void TransitToWandering()
    {
        _counter = 0;
        _advertiseCounter = 0;
        if(_salesDelivered >= _salesTargetNumber)
        {
            _render.color = _advertiserColors[2];
        }
        else
        {
            _render.color = _advertiserColors[_salesDelivered];
        }
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
