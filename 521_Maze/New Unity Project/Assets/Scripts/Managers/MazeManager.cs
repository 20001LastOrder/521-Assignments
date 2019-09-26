using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeManager : ManagerBase<MazeManager>
{
    [SerializeField]
    private GameObject _mazeWall;
    [SerializeField]
    private GameObject _mazeBoundary;
    [SerializeField]
    private GameObject _indicatorPrefab;

    [SerializeField]
    private int _showLength = 3;

    private static readonly int MAZE_LENGTH = 8;
    private static readonly int MAX_LEVEL = 15;
    private static readonly float MAZE_RATIO = 0.55f; 

    private bool[, ] _mazeRepresentations;
    private GameObject[,] _mazeWalls;
    private int _currentTime = 0;
    private List<int> _solutionPath;
    private List<Maze> _mazes;
    private GameObject _indicator;

    // Start is called before the first frame update
    void Start()
    {
        _mazeRepresentations = new bool[MAZE_LENGTH + 2, MAZE_LENGTH + 2];
        _mazes = new List<Maze>();
        _mazeWalls = new GameObject[MAZE_LENGTH + 2, MAZE_LENGTH + 2];
        // create maze path template
        MazeTemplate m = new MazeTemplate(MAZE_LENGTH);
        m.CeatePath(14, 16);
        _solutionPath = m.Path;

        for (var i = 0; i < 16; i++)
        {
            // for each time dimension, show _showLength part of the actual solution
            var min = (i + _showLength) > m.Path.Count ? Math.Max(m.Path.Count - _showLength, 0) : i;
            var max = (i + _showLength) > m.Path.Count ? m.Path.Count : i + _showLength;
            Debug.Log(min);
            Debug.Log(m.Path.Count);
            Maze maze = new Maze(MAZE_LENGTH, m.Path.GetRange(min, _showLength), MAZE_RATIO);
            _mazes.Add(maze);
        }

        ResetMazeRepresentations();
        CreateMazeOnRepresentations(_mazes[0]);
        PrintMaze();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_currentTime < MAX_LEVEL)
            {
                _currentTime += 1;
                CreateMazeOnRepresentations(_mazes[_currentTime]);
            }
            PrintMaze();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_currentTime > 0)
            {
                _currentTime -= 1;
                CreateMazeOnRepresentations(_mazes[_currentTime]);
            }
            PrintMaze();
        }
    }

    private void ResetMazeRepresentations()
    {
        for(var i = 0; i < _mazeRepresentations.GetLength(1); i++)
        {
            for(var j = 0; j < _mazeRepresentations.GetLength(0); j++)
            {
                _mazeRepresentations[j, i] = true;
            }
        }
    }

    private void OpenEntrance(Tuple<int, int> indexInOriginalMaze)
    {
        // left
        if(indexInOriginalMaze.Item1 == 0)
        {
            _mazeRepresentations[indexInOriginalMaze.Item1, indexInOriginalMaze.Item2 + 1] = false;
        }
        //right
        else if(indexInOriginalMaze.Item1 == MAZE_LENGTH - 1)
        {
            _mazeRepresentations[indexInOriginalMaze.Item1 + 2, indexInOriginalMaze.Item2 + 1] = false;
        }
        //buttom
        else if (indexInOriginalMaze.Item2 == 0)
        {
            _mazeRepresentations[indexInOriginalMaze.Item1 + 1, indexInOriginalMaze.Item2] = false;
        }
        //top
        else if(indexInOriginalMaze.Item2 == MAZE_LENGTH - 1)
        {
            _mazeRepresentations[indexInOriginalMaze.Item1 + 1, indexInOriginalMaze.Item2 + 2] = false;
        }
    }

    private void CreateMazeOnRepresentations(Maze maze)
    {
        var cells = maze.Cells;
        for(var i = 0; i < cells.GetLength(1); i++)
        {
            for(var j = 0; j < cells.GetLength(0); j++)
            {
                if(cells[j, i].Type == MazeCell.CellType.Tree)
                {
                    _mazeRepresentations[j + 1, i + 1] = true;
                }
                else if (cells[j,i].Type == MazeCell.CellType.Empty)
                {
                    _mazeRepresentations[j + 1, i + 1] = false;
                }
            }
        }
    }

    private void PrintMaze()
    {
        var mazeLength = _mazeRepresentations.GetLength(0);
        var boundaries = new HashSet<int>(Maze.FindBoundaryIds(mazeLength));
        for (var i = 0; i < _mazeRepresentations.GetLength(1); i++)
        {
            for (var j = 0; j < _mazeRepresentations.GetLength(0); j++)
            {
                if(_mazeRepresentations[j, i] && _mazeWalls[j, i] == null)
                {
                    if (boundaries.Contains(Maze.IndexToId(new Tuple<int, int>(j, i), mazeLength)))
                    {
                        _mazeWalls[j, i] = Instantiate(_mazeBoundary, new Vector3(j, 0f, i), Quaternion.identity, this.transform);
                    }
                    else
                    {
                        _mazeWalls[j, i] = Instantiate(_mazeWall, new Vector3(j, 0f, i), Quaternion.identity, this.transform);
                    }
                }
                else if(!_mazeRepresentations[j, i])
                {
                    if(_mazeWalls[j, i] != null)
                    {
                        Destroy(_mazeWalls[j, i]);
                        _mazeWalls[j, i] = null;
                    }
                }
            }
        }
    }

    public void OpenEntranceToTheMaze()
    {
        ResetMazeRepresentations();
        _currentTime = 0;
        var entranceIndex = Maze.IdToIndex(_solutionPath[0], MAZE_LENGTH);
        OpenEntrance(entranceIndex);
        CreateMazeOnRepresentations(_mazes[_currentTime]);
        PrintMaze();
        //instantiate the indicator in the entrance of the maze, +1 is because of the outside boundary
        _indicator = Instantiate(_indicatorPrefab, new Vector3(entranceIndex.Item1 + 1, 0.5f, entranceIndex.Item2 + 1), Quaternion.identity);
    }

    public void OpenExit()
    {
        ResetMazeRepresentations();
        _currentTime = 0;
        var entranceIndex = Maze.IdToIndex(_solutionPath[_solutionPath.Count - 1], MAZE_LENGTH);
        OpenEntrance(entranceIndex);
        CreateMazeOnRepresentations(_mazes[_currentTime]);
        PrintMaze();
    }

    public bool CanMove(Tuple<int, int> index)
    {
        return !_mazeRepresentations[index.Item1, index.Item2];
    }

    public Tuple<int, int> EntranceIndex()
    {
        var index = Maze.IdToIndex(_solutionPath[0],MAZE_LENGTH);
        return new Tuple<int, int>(index.Item1 + 1, index.Item2 + 1);
    }

    public void ForwardTime()
    {
        if (_currentTime < MAX_LEVEL)
        {
            _currentTime += 1;
            CreateMazeOnRepresentations(_mazes[_currentTime]);
        }
        PrintMaze();
    }
}
