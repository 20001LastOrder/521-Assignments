using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : ManagerBase<UIManager>
{
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

	[SerializeField]
	private RectTransform _currentPlayerActionIndicator;

	[SerializeField]
	private Scrollbar _playerActionTextScroll;

	[SerializeField]
	private float _indicatorMove = 17;

	[SerializeField]
	private float _scrollDistance = 0.00854f;

	private List<string> _playerActionLogs;
	private Queue<string> _thiefActionLogs;
	private Vector3 _startIndicatorPos;

	private void Start() {
		_playerActionLogs = new List<string>(_numberOfActionsToLog);
		_thiefActionLogs = new Queue<string>(_numberOfActionsToLog);
		_startIndicatorPos = _currentPlayerActionIndicator.localPosition;
		Debug.Log(_startIndicatorPos);
	}

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
		_playerActionText.text = output;
	}

	public void ClearPlayerActionLog() {
		_playerActionLogs = new List<string>();
		_playerActionText.text = "";
		_currentPlayerActionIndicator.localPosition = _startIndicatorPos;
		_playerActionTextScroll.value = 1;
	}

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

	private void PutActionToQueue(Queue<string> queue, string action) {
		queue.Enqueue(action);

		if (queue.Count > _numberOfActionsToLog) {
			queue.Dequeue();
		}
	}

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
