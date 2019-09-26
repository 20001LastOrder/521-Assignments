using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeTemplate
{
    private readonly MazeCell[,] _cells;

    private readonly int _mazeLength;

    private readonly List<int> _path;

    public int MazeLength => _mazeLength;

    //public getters
    public List<int> Path => _path;

    public MazeCell[,] Cells => _cells;

    public MazeTemplate(int mazeLength)
    {
        _mazeLength = mazeLength;
        _path = new List<int>();

        // initiate maze (default with all trees)
        _cells = Maze.InitializeMazeCells(mazeLength);
    }



    public List<int> CeatePath(int minLength, int maxLength)
    {

        //empty path
        _path.Clear();

        //get boundary id
        var boundaryIds = Maze.FindBoundaryIds(_mazeLength);
        Debug.Log(boundaryIds.Count);

        System.Random r = new System.Random();
        // start with a random boundary point
        int currentId = boundaryIds[r.Next(boundaryIds.Count)];

        _path.Add(currentId);

        char lastMovement = ' ';
        bool shouldStop = false;
        while (!shouldStop)
        {
            var index = Maze.IdToIndex(currentId, _mazeLength);

            //if element is in left boundary
            var movements = Maze.PossibleMovements(index, _mazeLength);
            //remove opposite movement if path started
            if(_path.Count > 1)
            {
                movements.Remove(Maze.OpposeMovement(lastMovement));
            }

            var nextMovement = movements[r.Next(movements.Count)];
            Maze.MAZE_MOVES.TryGetValue(nextMovement, out Tuple<int, int> displacement);

            // throw exception if no match displacement for the movement
            if (displacement == null)
            {
                throw new ArgumentException("No Match Displacement For the Value");
            }

            var newIndex = new Tuple<int, int>(index.Item1 + displacement.Item1, index.Item2 + displacement.Item2);
            lastMovement = nextMovement;
            currentId = Maze.IndexToId(newIndex, _mazeLength);

            var neighbours = Maze.FindNeighbours(currentId, _mazeLength);
            var inPathCount = 0;
            foreach(var neighbour in neighbours)
            {
                if (_path.Contains(neighbour))
                {
                    inPathCount++;
                }
            }

            // check if current id is in path, remove all elements after the id
            if (_path.Contains(currentId))
            {
                int idIndex = _path.IndexOf(currentId);
                _path.RemoveRange(idIndex + 1, _path.Count - idIndex - 1);
                // do not need to consider last move this time
                continue;
            }
            //if the path is larger than the maximum length we want
            else if(_path.Count >= maxLength)
            {
                // randomly remove the last several path
                int numberToRemove = r.Next(maxLength);
                _path.RemoveRange(_path.Count - numberToRemove, numberToRemove);
                currentId = _path[_path.Count - 1];
                continue;
            }
            // if there is a loop formed by the path
            else if(inPathCount > 1)
            {
                _path.RemoveAt(_path.Count - 1);
                currentId = _path[_path.Count - 1];
                continue;
            }


            // if not, add the id and check finish condition
            _path.Add(currentId);
            if (_path.Count > minLength && boundaryIds.Contains(currentId))
            {
                shouldStop = true;
                break;
            }
        }

        // change representation
        foreach (var id in _path)
        {
            var index = Maze.IdToIndex(id, _mazeLength);
            _cells[index.Item1, index.Item2].Type = MazeCell.CellType.Empty;
        }


        return _path;
    }
}
