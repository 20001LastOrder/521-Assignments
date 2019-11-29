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
        GetComponent<CircleCollider2D>().radius = _flyerRadius;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryConsumeFlyer(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryConsumeFlyer(collision.gameObject);
    }

    private void TryConsumeFlyer(GameObject obj)
    {
        if (_isConsumed||!obj.tag.Equals(_shopperTag))
        {
            return;
        }

        Shopper shopper = obj.GetComponent<Shopper>();
        shopper.FlyerPlayer();
        AdvertiserManager.Instance.BroadCastAdvertiseOpportunity(shopper);
        _isConsumed = true;
        Destroy(this.gameObject);
    }
}

