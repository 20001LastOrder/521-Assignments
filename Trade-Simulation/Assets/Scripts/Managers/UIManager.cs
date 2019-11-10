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

	private Queue<string> _playerActionLogs;
	private Queue<string> _thiefActionLogs;

	private void Start() {
		_playerActionLogs = new Queue<string>(_numberOfActionsToLog);
		_thiefActionLogs = new Queue<string>(_numberOfActionsToLog);
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

	public void LogPlayerAction(string action) {
		PutActionToQueue(_playerActionLogs, action);

		string output = "";
		int i = 0;
		foreach(var actionLog in _playerActionLogs) {
			if (i == _playerActionLogs.Count / 2) {
				output += "->" + actionLog + "\n";
			} else {
				output += "- " + actionLog + "\n";
			}
			i++;
		}
		_playerActionText.text = output;
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
