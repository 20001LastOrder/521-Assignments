using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer : MonoBehaviour
{
    [SerializeField]
    private string _shopperTag = "shopper";

    [SerializeField]
    private float _flyerRadius = 5;

    private bool _isConsumed = false;
    private void Start()
    {
        GetComponent<CircleCollider2D>().radius = AdvertiserManager.Instance.FlyerObservingDistance;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryConsumeFlyer(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryConsumeFlyer(collision.gameObject);
    }

    public void UpdateFlyerObservingDistance()
    {
        GetComponent<CircleCollider2D>().radius = AdvertiserManager.Instance.FlyerObservingDistance;
    }

    private void TryConsumeFlyer(GameObject obj)
    {
        if (_isConsumed||!obj.tag.Equals(_shopperTag))
        {
            return;
        }

        Shopper shopper = obj.GetComponent<Shopper>();
        shopper.FlyerPlayer();
        AdvertiserManager.Instance.BroadcastAdvertiseOpportunity(shopper);
        AdvertiserManager.Instance.Flyers.Remove(this);
        _isConsumed = true;
        Destroy(this.gameObject);
    }
}

