using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : ManagerBase<GameFlowManager>
{
    [SerializeField]
    private GameObject _player;

    public enum GameStage
    {
        OutsideMaze,
        InMaze
    }

    private GameStage _gameStatus = GameStage.OutsideMaze;
    public GameStage GameStatus => _gameStatus;

    public void EnterMaze()
    {
        var entranceCoord = MazeManager.Instance.EntranceIndex();
        Vector3 newPosition = new Vector3(entranceCoord.Item1, _player.transform.position.y, entranceCoord.Item2);
        _player.transform.position = newPosition;
        MazeManager.Instance.OpenExit();
        _gameStatus = GameStage.InMaze;
    }
}
