using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Detector to triger the game win
// Place at the exit of the maze
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
