using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinDetector : MonoBehaviour
{
    [SerializeField]
    private int _playerLayer = 9;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _playerLayer)
        {
            GameFlowManager.Instance.GameWin();
        }
    }
}
