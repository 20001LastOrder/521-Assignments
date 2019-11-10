using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : ManagerBase<UIManager>
{
    [SerializeField]
    private Text _inventoryText;

    public void LogInventory(SpiceVector playerInv, SpiceVector caravanInv)
    {
        var textFormat = "-------------------------\n";
        textFormat += "   |Tu|Sa|Ca|Ci|Cl|Pe|Su|\n";
        textFormat += LogSpiceVector("INV", playerInv);
        textFormat += LogSpiceVector("CAR", caravanInv);
        textFormat += "-------------------------";
        _inventoryText.text = textFormat;
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
