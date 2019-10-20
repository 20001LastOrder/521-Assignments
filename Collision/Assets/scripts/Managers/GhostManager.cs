using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : ManagerBase<GhostManager>
{

    [SerializeField]
    private GameObject ghostPrefab;

    [SerializeField]
    private Vector3 spawnButtomLeft;

    [SerializeField]
    private Vector3 spawnTopRight;

    [SerializeField]
    private int numGhost = 4;

    // Start is called before the first frame update
    void Start()
    {
        for(var i = 0; i < numGhost; i++)
        {
            CreateGhost();
        }
    }

    private void CreateGhost()
    {
        var spawnPosition = GetRandomSpawnLocation();
        Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnLocation()
    {
        var width = spawnTopRight.x - spawnButtomLeft.x;
        var height = spawnTopRight.y - spawnButtomLeft.y;

        return new Vector3(spawnButtomLeft.x + width * Utils.RandomFloat(), spawnButtomLeft.y + height * Utils.RandomFloat());
    }
}
