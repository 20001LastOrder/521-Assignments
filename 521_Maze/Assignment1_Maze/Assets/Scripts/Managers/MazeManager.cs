﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manager for maze creation and flow
public class MazeManager : ManagerBase<MazeManager>
{
    [SerializeField]
    private GameObject _mazeWall;
    [SerializeField]
    private GameObject _mazeBoundary;
    [SerializeField]
    private GameObject _indicatorPrefab;
    [SerializeField]
    private GameObject _detectorPrefab;

    // the maximum solution length that must be reserved a time.
    [SerializeField]
    private int _showLength = 3;

    public static readonly int MAZE_LENGTH = 8;
    public static readonly int MAX_LEVEL = 15;
    private static readonly float MAZE_RATIO = 0.55f; 

    private bool[, ] _mazeRepresentations;
    private GameObject[,] _mazeWalls;
    private List<int> _solutionPath;
    private List<Maze> _mazes;
    private Indicator _indicator;
    private GameObject _detector;

    private int _currentTime = 0;
    public int CurrentTime => _currentTime;

    void Start()
    {
        // the maze is surrounded by a boundary (in total 9 * 9)
        _mazeRepresentations = new bool[MAZE_LENGTH + 2, MAZE_LENGTH + 2];
        _mazes = new List<Maze>();
        _mazeWalls = new GameObject[MAZE_LENGTH + 2, MAZE_LENGTH + 2];
        
        // create maze path template
        MazeTemplate m = new MazeTemplate(MAZE_LENGTH);

        // draw a path
        var random = new System.Random();
        var pathLength = random.Next(13, 16);

        m.CeatePath(pathLength, pathLength);
        _solutionPath = m.Path;

        //create maze for each time dimension
        for (var i = 0; i < 16; i++)
        {
            // for each time dimension, show _showLength part of the actual solution
            var min = (i + _showLength) > m.Path.Count ? Math.Max(m.Path.Count - _showLength, 0) : i;
            Maze maze = new Maze(MAZE_LENGTH, m.Path.GetRange(min, _showLength), MAZE_RATIO);
            _mazes.Add(maze);
        }

        ResetMazeRepresentations();
        CreateMazeOnRepresentations(_mazes[0]);
        CreateMaze();
    }

    void Update()
    {
        // For debug, manually change the maze level
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_currentTime < MAX_LEVEL)
            {
                _currentTime += 1;
                CreateMazeOnRepresentations(_mazes[_currentTime]);
            }
            CreateMaze();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_currentTime > 0)
            {
                _currentTime -= 1;
                CreateMazeOnRepresentations(_mazes[_currentTime]);
            }
            CreateMaze();
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

    // Open an entrance at the boundary of the maze
    private Tuple<int, int> OpenEntrance(Tuple<int, int> indexInOriginalMaze)
    {
        var entranceIndex = new Tuple<int, int>(0, 0);
        // left
        if(indexInOriginalMaze.Item1 == 0)
        {
            entranceIndex = new Tuple<int, int>(indexInOriginalMaze.Item1, indexInOriginalMaze.Item2 + 1);
        }
        //right
        else if(indexInOriginalMaze.Item1 == MAZE_LENGTH - 1)
        {
            entranceIndex = new Tuple<int, int>(indexInOriginalMaze.Item1 + 2, indexInOriginalMaze.Item2 + 1);
        }
        //buttom
        else if (indexInOriginalMaze.Item2 == 0)
        {
            entranceIndex = new Tuple<int, int>(indexInOriginalMaze.Item1 + 1, indexInOriginalMaze.Item2);
        }
        //top
        else if(indexInOriginalMaze.Item2 == MAZE_LENGTH - 1)
        {
            entranceIndex = new Tuple<int, int>(indexInOriginalMaze.Item1 + 1, indexInOriginalMaze.Item2 + 2);
        }
        _mazeRepresentations[entranceIndex.Item1, entranceIndex.Item2] = false;
        return entranceIndex;
    }

    // Create actual maze representation
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

    // create maze in the actual game
    private void CreateMaze()
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

    // Open the entrance to the maze
    public void OpenEntranceToTheMaze()
    {
        ResetMazeRepresentations();
        _currentTime = 0;
        var entranceIndex = Maze.IdToIndex(_solutionPath[0], MAZE_LENGTH);
        OpenEntrance(entranceIndex);
        CreateMazeOnRepresentations(_mazes[_currentTime]);
        CreateMaze();
        //instantiate the indicator in the entrance of the maze, +1 is because of the outside boundary
        _indicator = Instantiate(_indicatorPrefab, new Vector3(entranceIndex.Item1 + 1, 0.5f, entranceIndex.Item2 + 1), Quaternion.identity).GetComponent<Indicator>();
    }

    public void OpenExit()
    {
        ResetMazeRepresentations();
        _currentTime = 0;
        var exitIndex = Maze.IdToIndex(_solutionPath[_solutionPath.Count - 1], MAZE_LENGTH);
        var exitBoundaryIndex = OpenEntrance(exitIndex);
        CreateMazeOnRepresentations(_mazes[_currentTime]);
        CreateMaze();

        //initialize the indicator
        _indicator.Init(_solutionPath, exitBoundaryIndex);

        //init timer (+1 as timer starts from 1)
        GameFlowManager.Instance.ResetTimer(MAX_LEVEL + 1);

        //instantiate the win game detector a the boundary exit of the maze
        if(_detector == null)
        {
            _detector = Instantiate(_detectorPrefab, new Vector3(exitBoundaryIndex.Item1, 0.5f, exitBoundaryIndex.Item2), Quaternion.identity);
        }
    }

    // Check if a grid is empty or a tree
    public bool CanMove(Tuple<int, int> index)
    {
        return !_mazeRepresentations[index.Item1, index.Item2];
    }

    // get the index of the entrance
    public Tuple<int, int> EntranceIndex()
    {
        var index = Maze.IdToIndex(_solutionPath[0],MAZE_LENGTH);
        return new Tuple<int, int>(index.Item1 + 1, index.Item2 + 1);
    }

    // forward maze in time dimension
    public void ForwardTime()
    {   
        //decrease timer
        GameFlowManager.Instance.DecreaseTimer();
        if (_currentTime < MAX_LEVEL)
        {
            _currentTime += 1;
            CreateMazeOnRepresentations(_mazes[_currentTime]);
            CreateMaze();
        }
        else if(GameFlowManager.Instance.GameStatus != GameFlowManager.GameStage.GameFinished)
        {
            GameFlowManager.Instance.ResetMaze();
        }
    }

    // show the next solution guidance for the Maze
    public void NextGuidance(Tuple<int, int> currentPosition, Tuple<int, int> nextPosition)
    {
        _indicator.UpdateIndicator(currentPosition, nextPosition);
    }
}
