using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : ManagerBase<ShopManager>
{
    [SerializeField]
    private List<Shop> _shops;

	// The exit coordinate for upper shops. (lower is just - this number)
    [SerializeField]
    private float _shopExitY = 6;

	public float ShopExitY => _shopExitY;

    public List<Shop> Shops => _shops;

    public Vector3 GetRandomShopLocation()
    {
        var shop = _shops[Utils.RandomInt(0, _shops.Count)];
        var randomX = Utils.RandomFloat(shop.LowerLeftCorner.x, shop.UpperRightCorner.x);
        var randomY = Utils.RandomFloat(shop.LowerLeftCorner.y, shop.UpperRightCorner.y);
        return new Vector3(randomX, randomY, -1);
    }
}
