using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI logging manager
public class UIManager : ManagerBase<UIManager>
{
    // player inventory text
    [SerializeField]
    private Text _inventoryText;

	[SerializeField]
	private Text _thiefInventoryText;

	[SerializeField]
	private Text _playerActionText;

	[SerializeField]
	private Text _thiefActionText;

	[SerializeField]
	private int _numberOfActionsToLog = 8;

	[SerializeField]
	private Text _simulationSpeedText;

    // inicator (->) that indicate current player action
	[SerializeField]
	private RectTransform _currentPlayerActionIndicator;

	[SerializeField]
	private Scrollbar _playerActionTextScroll;

    [SerializeField]
    private GameObject _goalReachedLogo;

	[SerializeField]
	private float _indicatorMove = 17;

    [SerializeField]
    private float _indicatorOffset = 17;

    [SerializeField]
    private int _logsInVision = 5;

	private float _scrollDistance = 0.00854f;

	private List<string> _playerActionLogs;
	private Queue<string> _thiefActionLogs;
	private Vector3 _startIndicatorPos;

	private void Start() {
		_playerActionLogs = new List<string>(_numberOfActionsToLog);
		_thiefActionLogs = new Queue<string>(_numberOfActionsToLog);
		_startIndicatorPos = _currentPlayerActionIndicator.localPosition;
	}

    // log player and caravan state inventory
	public void LogStateInventory(SpiceVector playerInv, SpiceVector caravanInv)
    {
        var textFormat = "-------------------------\n";
        textFormat += "   |Tu|Sa|Ca|Ci|Cl|Pe|Su|\n";
        textFormat += LogSpiceVector("INV", playerInv);
        textFormat += LogSpiceVector("CAR", caravanInv);
        textFormat += "-------------------------";
        _inventoryText.text = textFormat;
	}

	public void LogThiefInventory(SpiceVector thiefInv) {
		var textFormat = "-------------------------\n";
		textFormat += "   |Tu|Sa|Ca|Ci|Cl|Pe|Su|\n";
		textFormat += LogSpiceVector("THF", thiefInv);
		textFormat += "-------------------------";
		_thiefInventoryText.text = textFormat;
	}

	public void AddNewPlayerActoinLog(string action) {
		_playerActionLogs.Add(action);
	}

	public void ShowPlayerActionLog() {
		string output = "";
		foreach (var actionLog in _playerActionLogs) {
			output += actionLog + "\n";
		}

        // resize the text box to fit the current plan actions
        var size = _playerActionText.rectTransform.sizeDelta;
        size.y = _indicatorMove * (_playerActionLogs.Count + 2);
        _playerActionText.rectTransform.sizeDelta = size;

        // reset the position of the indicator
        var position = _currentPlayerActionIndicator.localPosition;
        position.y = size.y / 2 - (3 * _indicatorMove) - _indicatorOffset;
        _currentPlayerActionIndicator.localPosition = position;

        // reset scroll distance for each action
        _scrollDistance = 1.0f / (_playerActionLogs.Count - _logsInVision);

        _playerActionText.text = output;
    }

	public void ClearPlayerActionLog() {
		_playerActionLogs = new List<string>();
		_playerActionText.text = "";
		_currentPlayerActionIndicator.localPosition = _startIndicatorPos;
		_playerActionTextScroll.value = 1;
	}

    // move the indicator and scroll to next action
	public void MoveToNextPlayerAction() {
        var position = _currentPlayerActionIndicator.localPosition;
		position.y -= _indicatorMove;
		_currentPlayerActionIndicator.localPosition = position;
		_playerActionTextScroll.value -= _scrollDistance;
	}

	public void LogThiefAction(string action) {
		PutActionToQueue(_thiefActionLogs, action);

		string output = "";
		int i = 0;
		foreach (var actionLog in _thiefActionLogs) {
			if (i == _thiefActionLogs.Count - 1) {
				output += "->" + actionLog + "\n";
			} else {
				output += "- " + actionLog + "\n";
			}
			i++;
		}
		_thiefActionText.text = output;
	}

	public void SetSimulationSpeedText(float speed) {
		_simulationSpeedText.text = speed + " x";
	}

    public void SetGameEnd()
    {
        _goalReachedLogo.SetActive(true);
    }

	private void PutActionToQueue(Queue<string> queue, string action) {
		queue.Enqueue(action);

		if (queue.Count > _numberOfActionsToLog) {
			queue.Dequeue();
		}
	}

    // log spice vector
	private string LogSpiceVector(string name, SpiceVector vector)
    {
        string output = name + "|";
        foreach(var spice in vector.Spices)
        {
            output += spice.ToString().PadLeft(2, ' ') + "|";
        }
        output += "\n";
        return output;
    }
}
