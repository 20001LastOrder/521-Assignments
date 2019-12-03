using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer : MonoBehaviour
{
    [SerializeField]
    private string _shopperTag = "shopper";


    private bool _isConsumed = false;

	// when the player bounced into the flyer
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryConsumeFlyer(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryConsumeFlyer(collision.gameObject);
    }
	
	// one flyer can be consumed only once
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

