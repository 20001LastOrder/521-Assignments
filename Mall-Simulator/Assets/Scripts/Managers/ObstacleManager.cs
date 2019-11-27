using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : ManagerBase<ObstacleManager>
{
    private List<CircularObject> _obstacles;

    protected override void Awake()
    {
        base.Awake();
        _obstacles = new List<CircularObject>();
    }

    public void RegisterObstacle(CircularObject obj)
    {
        _obstacles.Add(obj);
    }

    public void DeregisterObstacle(CircularObject obj)
    {
        _obstacles.Remove(obj);
    }

    public List<CircularObject> Obstacles => _obstacles;
}
