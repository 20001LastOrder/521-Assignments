using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : ManagerBase<GameFlowManager>
{
    [SerializeField]
    private GameObject _player;

    // UI elements
    [SerializeField]
    private GameObject _winPanel;

    [SerializeField]
    private GameObject _lostPanel;

    [SerializeField]
    private GameObject _aim;

    [SerializeField]
    private TimeCounter _timer;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_gameStatus == GameStage.InMaze || _gameStatus == GameStage.GameLose)
            {
                _player.GetComponent<FirstPersonController>().ResetPlayer();
                Instance.EnterMaze();
            }
        }
    }

    public enum GameStage
    {
        OutsideMaze,
        InMaze,
        GameFinished,
        GameLose
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

        //reset the UI and cursor
        _aim.SetActive(false);
        _lostPanel.SetActive(false);
        _timer.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GameWin()
    {
        if(_gameStatus == GameStage.InMaze)
        {
            _winPanel.SetActive(true);
            _aim.SetActive(false);
            _gameStatus = GameStage.GameFinished;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void GameLose()
    {
        if(_gameStatus == GameStage.InMaze)
        {
            _lostPanel.SetActive(true);
            _aim.SetActive(false);
            _gameStatus = GameStage.GameLose;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Restart game (used in OnClick)
    public void RestartGame()
    {
        Scene s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.name);
    }

    public void ResetTimer(int time)
    {
        _timer.HardSetTimer(time);
    }

    public void DecreaseTimer()
    {
        this.StartCoroutine(_timer.DecreaseTimer());
    }
}
