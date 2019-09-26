using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    [SerializeField]
    private int _playerLayer = 9;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == _playerLayer)
        {
            GameFlowManager.Instance.EnterMaze();
            gameObject.SetActive(false);
        }
    }

}
