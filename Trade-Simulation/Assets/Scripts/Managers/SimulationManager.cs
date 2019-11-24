using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// manage simulate speed & restart
public class SimulationManager : ManagerBase<SimulationManager>
{
	[SerializeField]
	private float _minSimulationSpeed = 0.25f;

	[SerializeField]
	private float _maxSimulationSpeed = 20;

	private float _currentSimulationSpeed = 1;

    private bool _isGoalReached = false;

    // if the goal is reached, pause the game and set game end logo
    public bool IsGoalReached
    {
        get => _isGoalReached;
        set
        {
            if (value)
            {
                Time.timeScale = 0;
                UIManager.Instance.SetGameEnd();
            }
            _isGoalReached = value;
        }
    }

    void Update()
    {
        // pauseand/continue, restart when goal found
		if (Input.GetKeyDown(KeyCode.Space)) {
            if (_isGoalReached)
            {
                Time.timeScale = 1;
                Scene s = SceneManager.GetActiveScene();
                SceneManager.LoadScene(s.name);
                return;
            }


            if (Time.timeScale > 0) {
				_currentSimulationSpeed = Time.timeScale;
				UIManager.Instance.SetSimulationSpeedText(0);
				Time.timeScale = 0;
			} else {
				SetCurrentSimulationSpeed();
			}
        // speed up
		}else if (Input.GetKeyDown(KeyCode.Equals)) {
			if(Time.timeScale <= 0) {
				return;
			}

			_currentSimulationSpeed = Time.timeScale * 2;
			if(_currentSimulationSpeed <= _maxSimulationSpeed) {

				SetCurrentSimulationSpeed();
			}
        // speed down
		} else if (Input.GetKeyDown(KeyCode.Minus)) {
			if (Time.timeScale <= 0) {
				return;
			}

			_currentSimulationSpeed = Time.timeScale / 2;
			if (_currentSimulationSpeed >= _minSimulationSpeed) {
				SetCurrentSimulationSpeed();
			}
		}
	}

	private void SetCurrentSimulationSpeed() {
		UIManager.Instance.SetSimulationSpeedText(_currentSimulationSpeed);
		Time.timeScale = _currentSimulationSpeed;
	}
}
