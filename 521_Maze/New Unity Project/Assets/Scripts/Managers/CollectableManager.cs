using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : ManagerBase<CollectableManager>
{
    [SerializeField]
    private List<Transform> _collectableParents;

    [SerializeField]
    private GameObject _collectablePrefab;

    [SerializeField]
    private GameObject _keyPrefab;

    [SerializeField]
    private int _collectableQuantity = 3;

    private int _collectableInScene;

    void Start()
    {
        _collectableInScene = _collectableQuantity;
        var r = new System.Random();
        for(var i = 0; i < _collectableQuantity; i++)
        {
            var parentIndex = r.Next(_collectableParents.Count);
            var parent = _collectableParents[parentIndex];

            //remove parent index to avoid repeatitive
            _collectableParents.RemoveAt(parentIndex);
            var collectable = Instantiate(_collectablePrefab, parent.position, _collectablePrefab.transform.rotation);
            collectable.GetComponent<Collectable>().OnDestroy += CollectableDestroyed;
        }
    }

    private void CollectableDestroyed(GameObject colletable)
    {
        _collectableInScene--;

        // if collectable number is zero, create the key
        if(_collectableInScene == 0)
        {
            Instantiate(_keyPrefab, colletable.transform.position, Quaternion.identity);
        }
    }
}
