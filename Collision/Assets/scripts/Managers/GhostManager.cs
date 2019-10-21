using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : ManagerBase<GhostManager>
{

    [SerializeField]
    private GameObject ghostPrefab;

    [SerializeField]
    private List<Transform> ghostSpawnPoints;

    [SerializeField]
    private int numGhost = 4;

    [SerializeField]
    private float ghostChangeMovementHeight = 2;

    private List<Ghost> ghosts;

    private System.Random r = new System.Random();

    public float GhostChangeMovementHeight => ghostChangeMovementHeight;



    // Start is called before the first frame update
    void Start()
    {
        ghosts = new List<Ghost>();

        var ghostParents = new List<Transform>(ghostSpawnPoints);

        for (var i = 0; i < numGhost; i++)
        {
            // draw next position 
            var parentIndex = r.Next(ghostParents.Count);
            CreateGhost(ghostParents[parentIndex]);

            //remove to avoid repetitive positions
            ghostParents.RemoveAt(parentIndex);
        }
    }

    private void RespawnGhost(Ghost g)
    {
        ghosts.Remove(g);

        var parentIndex = r.Next(ghostSpawnPoints.Count);
        CreateGhost(ghostSpawnPoints[parentIndex]);
    }

    private void CreateGhost(Transform parent)
    {
        var ghost = Instantiate(ghostPrefab, parent).GetComponent<Ghost>();
        ghost.transform.parent = null;
        ghost.OnDestroy += RespawnGhost;
        ghosts.Add(ghost);
    }

    public bool DoesCollideWithGhosts(Vector3 position, Vector3 velocity, float radius)
    {
        var hasCollision = false;

        foreach (var ghost in ghosts)
        {
            if(ghost != null)
            {
                hasCollision = hasCollision || CollisionWithSingleGhost(position, velocity, radius, ghost);
            }
        }

        return hasCollision;
    }

    private bool CollisionWithSingleGhost(Vector3 position, Vector3 velocity, float radius, Ghost ghost)
    {
        var transformsOfGhost = ghost.Points;

        // if there are no points, no collision (might happen when the ghost is being destroyed)
        if(transformsOfGhost == null)
        {
            return false;
        }

        for(var i = 0; i < transformsOfGhost.Count; i++)
        {
            var next = (i + 1) % transformsOfGhost.Count;
            if (EnvironmentManager.Instance.LinePointCollisionDetection(transformsOfGhost[i].CurrentPosition, transformsOfGhost[next].CurrentPosition, position, radius)){
                // TODO: change Ghost behaviour
                var acc = velocity / Time.deltaTime / 2;
                transformsOfGhost[i].AddInstantaneousAcc(acc);
                transformsOfGhost[next].AddInstantaneousAcc(acc);

                return true;
            }

        }

        return false;
    }
}
