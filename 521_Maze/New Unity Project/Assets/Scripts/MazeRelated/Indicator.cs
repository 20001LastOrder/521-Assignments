using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    [SerializeField]
    private int _playerLayer = 9;

    private LinkedList<Tuple<int, int>> _solutionPath;

    public void Init(List<int> solutionPath, Tuple<int, int> exitIndex)
    {
        _solutionPath = new LinkedList<Tuple<int, int>>();
        //clone solution path
        solutionPath.ForEach(x =>
        {
            var originalPosition = Maze.IdToIndex(x, MazeManager.MAZE_LENGTH);

            //consider the boundary
            _solutionPath.AddLast(new Tuple<int, int>(originalPosition.Item1 + 1, originalPosition.Item2 + 1));
        });

        //add exit Index at the boundary
        _solutionPath.AddLast(exitIndex);

        //remove the first grid, as player is already there
        _solutionPath.RemoveFirst();
        //set indicator to the next place of the solution path
        var position = _solutionPath.First.Value;
        transform.position = new Vector3(position.Item1, 0.5f, position.Item2);
    }

    public void UpdateIndicator(Tuple<int, int> currentPosition, Tuple<int, int> nextPosition)
    {

        // get the next solution position, default to (0, 0) if there is no grid anymore (meanning the player
        // is at the exit)
        var position = _solutionPath.Count==0? new Tuple<int, int>(-1, -1) : _solutionPath.First.Value;
        Vector3 nextIndicatorPosition;
        // compare with next position considering boundary
        if(nextPosition.Equals(position))
        {
            if (_solutionPath.Count == 0) return;
            // player is on the solution path
            _solutionPath.RemoveFirst();
            var nextSoluionPosition = _solutionPath.First.Value;
            nextIndicatorPosition = new Vector3(nextSoluionPosition.Item1, 0.5f, nextSoluionPosition.Item2);
        }
        else
        {
            //player is not in the solution path, add currentPosition to the solution path
            // _solutionPath.AddFirst(currentPosition);
            // nextIndicatorPosition = new Vector3(currentPosition.Item1, 0.5f, currentPosition.Item2);
            nextIndicatorPosition = transform.position;
        }


        transform.position = nextIndicatorPosition;
    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.layer != _playerLayer)
        {
            return;
        }

        if(GameFlowManager.Instance.GameStatus == GameFlowManager.GameStage.OutsideMaze)
        {
            GameFlowManager.Instance.EnterMaze();
        }
    }

}
