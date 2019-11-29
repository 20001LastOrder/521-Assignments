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
    private float _counter;

    [SerializeField]
    private int _eaterLayer = 9;

    [SerializeField]
    private SpriteRenderer _render;

    private Seat _targetSeat;

    private Vector3 _positionBeforeSit;

    public enum ShopperStatus
    {
        TavellingToExitMall,
        TravellingToShop,
        InShop,
        TravellingToEat,
        Seating,
        FinishedEat
    }

    private ShopperStatus _status; 

    protected override void ActionSelection()
    {
        switch (_status)
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
                    _status = ShopperStatus.InShop;
                }
                break;
            case ShopperStatus.InShop:
                _counter += Time.deltaTime;
                if(_counter > IN_SHOP_WAITING_TIME)
                {
                    _render.color = Color.red;
                    _counter = 0;
					ResetSteering();
					_targetSeat = FoodCourtManager.Instance.GetRandomAvailableSeat();
					_seekingTarget = _targetSeat.transform.position;
					_status = ShopperStatus.TravellingToEat;
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
                    _status = ShopperStatus.Seating;
                }else if (!_targetSeat.IsAvailable)
                {
                    _targetSeat = FoodCourtManager.Instance.GetRandomAvailableSeat();
                    _seekingTarget = _targetSeat.transform.position;
                }
                break;
            case ShopperStatus.Seating:
                _counter += Time.deltaTime;
                if (_counter > EAT_TIME)
                {
                    _render.color = Color.green;
                    _counter = 0;
                    _seekingTarget = _positionBeforeSit;
                    _status = ShopperStatus.FinishedEat;
                }
                break;
            case ShopperStatus.FinishedEat:
                if (Vector3.Distance(transform.position, _seekingTarget.Value) < 0.1f)
                {
                    // enable collision between seat and player
                    _targetSeat.IsAvailable = true;
                    gameObject.layer = 13;
                    _status = ShopperStatus.TavellingToExitMall;
                    ResetSteering();
                    _seekingTarget = null;
                }
                break;
        }
    }

    protected override void Steering()
    {
        //no steering in shop and seat
        switch (_status)
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
            _status = ShopperStatus.TavellingToExitMall;
        }
        else
        {
            _seekingTarget = ShopManager.Instance.GetRandomShopLocation();
            _status = ShopperStatus.TravellingToShop;
        }

        this._maxSpeed = Utils.RandomFloat(_shopperMinSpeed, _shopperMaxSpeed);
    }
}
