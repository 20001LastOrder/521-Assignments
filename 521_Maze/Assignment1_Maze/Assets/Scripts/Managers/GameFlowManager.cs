using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Manager for UI and game flow
public class GameFlowManager : ManagerBase<GameFlowManager>
{
    [SerializeField]
    private GameObject _player;

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
        // lock the cursor and make the cursor disappear 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    private void Update()
    {
        //Check if the escape key is pressed and reset the maze
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_gameStatus == GameStage.InMaze || _gameStatus == GameStage.GameLose)
            {
                ResetMaze();
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

    /*
     * Enter maze, move player to the first grid and then close the entrance while opening the exit. 
     * Setup some UI elements as well
     */
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

    public void ResetMaze()
    {
        _player.GetComponent<FirstPersonController>().ResetPlayer();
        Instance.EnterMaze();
    }

    /*
     * Set game status to win and show the relavent UI
     */
    public void GameWin()
    {
        if(_gameStatus == GameStage.InMaze)
        {
            _winPanel.SetActive(true);
            _aim.SetActive(false);
            _timer.gameObject.SetActive(false);
            _gameStatus = GameStage.GameFinished;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            //pose the time
            _player.GetComponent<FirstPersonController>().frozenPlayer();
            Time.timeScale = 0;
        }
    }

    // Set game status to lose and show the relavent UI
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

    // Restart the timer in UI
    public void ResetTimer(int time)
    {
        _timer.HardSetTimer(time);
    }

    // Decrease the timer with a new coroutine to player the decrease music
    public void DecreaseTimer()
    {
        StartCoroutine(_timer.DecreaseTimer());
    }
}
