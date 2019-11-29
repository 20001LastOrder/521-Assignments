using UnityEngine;

public class Shopper : SteeringAgent
{
    private static readonly float IN_SHOP_WAITING_TIME = 1.0f;
    private static readonly float EAT_TIME = 4.0f;

    [SerializeField]
    private float _shopperMinSpeed;

    [SerializeField]
    private float _shopperMaxSpeed;

    [SerializeField]
    private float _despawnX;

    [SerializeField]
    private Vector3 flowVelocity;

    [SerializeField]
    private float _waitingForSeatTime = 1;

    [SerializeField]
    private int _eaterLayer = 9;

    [SerializeField]
    private SpriteRenderer _render;

    [SerializeField]
    private float _flyeredTime = 2;

    private Seat _targetSeat;

    private Vector3 _positionBeforeSit;

    public enum ShopperStatus
    {
        TavellingToExitMall,
        TravellingToShop,
        InShop,
        TravellingToEat,
        Seating,
        FinishedEat,
        Flyered,
        WaitingForSeat
    }

    private ShopperStatus _state;
    private ShopperStatus _historyState;
    private Color _historyColor;
    private float _counter;
    private float _flyeredCounter;

    protected override void ActionSelection()
    {
        switch (_state)
        {
            case ShopperStatus.TavellingToExitMall:
                if (transform.position.x > _despawnX)
                {
                    Destroy(this.gameObject);
                }
                break;
            case ShopperStatus.TravellingToShop:
                if(Vector3.Distance(transform.position, _seekingTarget.Value) < 0.1f)
                {
                    _counter = 0;
                    ResetSteering();
                    _state = ShopperStatus.InShop;
                }
                break;
            case ShopperStatus.InShop:
                _counter += Time.deltaTime;
                if(_counter > IN_SHOP_WAITING_TIME)
                {
                    // orange color
                    _render.color = Color.magenta;
                    _counter = 0;
					ResetSteering();
					_targetSeat = FoodCourtManager.Instance.GetRandomAvailableSeat();
                    if(_targetSeat != null)
                    {
                        _seekingTarget = _targetSeat.transform.position;
                        _state = ShopperStatus.TravellingToEat;
                    }
                    else
                    {
                        _state = ShopperStatus.WaitingForSeat;
                    }
					
				}
                break;
            case ShopperStatus.TravellingToEat:
                if (Vector3.Distance(transform.position, _seekingTarget.Value) < 0.5f && _targetSeat.IsAvailable)
                {
                    // disable collision between seat and player
                    gameObject.layer = 9;
                    ResetSteering();
                    _counter = 0;
                    _positionBeforeSit = transform.position;
                    _targetSeat.IsAvailable = false;
                    _state = ShopperStatus.Seating;
                }else if (_targetSeat == null || !_targetSeat.IsAvailable)
                {
                    _state = ShopperStatus.WaitingForSeat;
                }
                break;
            case ShopperStatus.WaitingForSeat:
                _counter += Time.deltaTime;
                if(_counter > _waitingForSeatTime)
                {
                    _counter = 0;
                    _targetSeat = FoodCourtManager.Instance.GetRandomAvailableSeat();
                    if(_targetSeat != null && _targetSeat.IsAvailable)
                    {
                        _seekingTarget = _targetSeat.transform.position;
                        _state = ShopperStatus.TravellingToEat;
                    }
                }

                break;
            case ShopperStatus.Seating:
                _counter += Time.deltaTime;
                if (_counter > EAT_TIME)
                {
                    _render.color = Color.green;
                    _counter = 0;
                    _seekingTarget = _positionBeforeSit;
                    _state = ShopperStatus.FinishedEat;
                }
                break;
            case ShopperStatus.FinishedEat:
                if (Vector3.Distance(transform.position, _seekingTarget.Value) < 0.1f)
                {
                    // enable collision between seat and player
                    _targetSeat.IsAvailable = true;
                    gameObject.layer = 13;
                    _state = ShopperStatus.TavellingToExitMall;
                    ResetSteering();
                    _seekingTarget = null;
                }
                break;
            case ShopperStatus.Flyered:
                _flyeredCounter += Time.deltaTime;
                // when the flyered is finished, go back to whatever state before
                if(_flyeredCounter > _flyeredTime)
                {
                    _flyeredCounter = 0;
                    _state = _historyState;
                    _render.color = _historyColor;
                }
                break;
        }
    }

    public void FlyerPlayer()
    {
        if(_state == ShopperStatus.Flyered)
        {
            return;
        }
        _flyeredCounter = 0;
        _historyState = _state;
        _historyColor = _render.color;
        _render.color = Color.black;
        _state = ShopperStatus.Flyered;
        ResetSteering();
    }

    protected override void Steering()
    {
        //no steering in flyered
        switch (_state)
        {
            case ShopperStatus.TavellingToExitMall:
                SteeringManager.Instance.FlowFieldFollowing(this, flowVelocity);
                SteeringManager.Instance.ObstacleAvoidance(this);
                break;
            case ShopperStatus.TravellingToEat:
            case ShopperStatus.TravellingToShop:
                SteeringManager.Instance.Seek(this);
                SteeringManager.Instance.ObstacleAvoidance(this);
                break;
            case ShopperStatus.FinishedEat:
            case ShopperStatus.Seating:
                SteeringManager.Instance.Seek(this);
                break;
            case ShopperStatus.WaitingForSeat:
                SteeringManager.Instance.ObstacleAvoidance(this);
                break;
        }
    }

    protected void OnDestroy()
    {
        ShopperManager.Instance.DeregisterShopper(this);
    }

    protected override void Start()
    {
        base.Start();
        flowVelocity = flowVelocity.normalized * _maxSpeed;

        //toss a coin and decide what the shopper should do
        if(Utils.RandomFloat() < 0.5f)
        {
            _state = ShopperStatus.TavellingToExitMall;
        }
        else
        {
            _seekingTarget = ShopManager.Instance.GetRandomShopLocation();
            _state = ShopperStatus.TravellingToShop;
        }

        this._maxSpeed = Utils.RandomFloat(_shopperMinSpeed, _shopperMaxSpeed);
    }
}
