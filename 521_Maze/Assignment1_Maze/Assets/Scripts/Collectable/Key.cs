using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Key for opening the maze entrance
public class Key : MonoBehaviour
{
    [SerializeField]
    private int _playerLayer = 9;

    [SerializeField]
    private int _projectile = 10;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == _playerLayer || collision.gameObject.layer == _projectile)
        {
            MazeManager.Instance.OpenEntranceToTheMaze();
            Destroy(gameObject);
        }
    }
}
