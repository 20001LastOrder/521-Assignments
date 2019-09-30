using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    [SerializeField]
    private Text _timerText;

    [SerializeField]
    private int _timerDecreaseInterval = 3;

    [SerializeField]
    private float _timerDecreaseTime = 1.0f;

    private readonly string CONNECTOR = ":";

    private int _currentTime;


    public void HardSetTimer(int time)
    {
        _timerText.text = FormatNumber(time) + CONNECTOR + FormatNumber(0);
        _currentTime = time;
    }

    public IEnumerator DecreaseTimer()
    {
        _currentTime--;
        int minute = 60;
        float elapsedTime = ((float)_timerDecreaseInterval) / 60 * _timerDecreaseTime;
        while(minute > 0)
        {
            _timerText.text = FormatNumber(_currentTime) + CONNECTOR + FormatNumber(minute);
            minute -= _timerDecreaseInterval;
            yield return new WaitForSeconds(elapsedTime);
        }
        _timerText.text = FormatNumber(_currentTime) + CONNECTOR + FormatNumber(0);
    }
    private string FormatNumber(int num)
    {
        return num.ToString("D2");
    }
}
