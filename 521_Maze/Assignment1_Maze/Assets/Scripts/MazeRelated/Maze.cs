using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Create maze with some reserved part (keeping empty)
public class Maze
{
    // provide movement in the maze
    public static readonly Dictionary<char, Tuple<int, int>> MAZE_MOVES = new Dictionary<char, Tuple<int, int>>
    {
        { 'E', new Tuple<int, int>(1, 0)},
        { 'W', new Tuple<int, int>(-1, 0)},
        { 'S', new Tuple<int, int>(0, -1)},
        { 'N', new Tuple<int, int>(0, 1)}
    };

    private readonly MazeCell[,] _cells;
    private readonly List<int> _path;

    private readonly int _mazeLength;
    public int MazeLength => _mazeLength;
    public MazeCell[,] Cells => _cells;

    // use only one random to make sure that the sequence does not repeat
    private static readonly System.Random r = new System.Random();

    public Maze(int mazeLength, List<int> initialPath, float mazeRatio)
    {
        _cells = InitializeMazeCells(mazeLength);
        _path = new List<int>(initialPath);
        this._mazeLength = mazeLength;

        foreach (var cellId in _path)
        {
            var index = IdToIndex(cellId, mazeLength);
            _cells[index.Item1, index.Item2].Type = MazeCell.CellType.Empty;
        }
        BuildMaze(mazeRatio);
    }

    /*
     * Build up the maze using (Modified) Wilson's Algorithm
     */
    private void BuildMaze(float mazeRatio)
    {
        var ids = new List<int>(System.Linq.Enumerable.Range(0, _mazeLength * _mazeLength - 1));
        var walkableCellIds = new HashSet<int>(_path);

        ids.RemoveAll((int id) => walkableCellIds.Contains(id));

        while (walkableCellIds.Count < mazeRatio * (_mazeLength * _mazeLength))
        {
            var path = new List<int>();
            var id = ids[r.Next(ids.Count)];
            var lastMovement = ' ';

            path.Add(id);
            // random walk until it hits the boundary of the walkbale maze
            while (true)
            {
                var neighbours = FindNeighbours(id, _mazeLength);
                //finish finding if the condition meets
                if (neighbours.FindAll((neighbour) => walkableCellIds.Contains(neighbour)).Count > 0) break;

                var index = IdToIndex(id, _mazeLength);
                //otherwise start wondering
                var movements = Maze.PossibleMovements(index, _mazeLength);
                //remove opposite movement if path started
                if (path.Count > 1)
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
                id = IndexToId(newIndex, _mazeLength);


                // check if current id is in path, remove all elements after the id
                if (path.Contains(id))
                {
                    int idIndex = path.IndexOf(id);
                    path.RemoveRange(idIndex + 1, path.Count - idIndex - 1);
                    // do not need to consider last move this time
                    continue;
                }

                // if not, add the id
                path.Add(id);
            }

            // add path to the walkable cell set, and change the representation of them
            foreach(var cellId in path)
            {
                walkableCellIds.Add(cellId);
                var index = IdToIndex(cellId, _mazeLength);
                _cells[index.Item1, index.Item2].Type = MazeCell.CellType.Empty;
            }

        }
    }

    public static Tuple<int, int> IdToIndex(int id, int mazeSize)
    {
        Tuple<int, int> index = new Tuple<int, int>(id % mazeSize, id / mazeSize);
        return index;
    }

    public static int IndexToId(Tuple<int, int> index, int mazeSize)
    {
        return index.Item2 * mazeSize + index.Item1;
    }

    public static char OpposeMovement(char move)
    {
        if(move == 'W')
        {
            return 'E';
        }else if(move == 'E')
        {
            return 'W';
        }else if(move == 'N')
        {
            return 'S';
        }else if(move == 'S')
        {
            return 'N';
        }
        throw new ArgumentException("Bad Movements");
    }

    // check the possible movements by the place of the grid in the maze
    public static List<char> PossibleMovements(Tuple<int, int> index, int mazeSize)
    {
        List<char> movements = new List<char>(new char[] { 'W', 'S', 'N', 'E' });

        //left boundary
        if (index.Item1 == 0)
        {
            movements.Remove('W');
        }

        //buttom boundary
        if(index.Item2 == 0)
        {
            movements.Remove('S');
        }

        //right boundary
        if(index.Item1 == mazeSize - 1)
        {
            movements.Remove('E');
        }

        //top boundary
        if(index.Item2 == mazeSize - 1)
        {
            movements.Remove('N');
        }

        return movements;
    }

    // Find neightbours given a grid id 
    public static List<int> FindNeighbours(int id, int mazeSize)
    {
        List<int> neighbours = new List<int>();

        //buttom neighbour
        if(!(id < mazeSize))
        {
            neighbours.Add(id - mazeSize);
        }

        // top neighbour
        if(!(id >= mazeSize * (mazeSize - 1)))
        {
            neighbours.Add(id + mazeSize);
        }

        //left neighbour
        if(id % mazeSize != 0)
        {
            neighbours.Add(id - 1);
        }

        //right neighbour
        if(id % mazeSize != mazeSize - 1)
        {
            neighbours.Add(id + 1);
        }

        return neighbours;
    }

    // At the beginning, initialize all the maze cells to tree
    public static MazeCell[, ] InitializeMazeCells(int mazeSize)
    {
        var mazeCells = new MazeCell[mazeSize, mazeSize];
        for (var i = 0; i < mazeSize; i++)
        {
            for (var j = 0; j < mazeSize; j++)
            {
                // j is x coordinate, i is y coordinate
                int id = Maze.IndexToId(new System.Tuple<int, int>(j, i), mazeSize);

                mazeCells[j, i] = new MazeCell(MazeCell.CellType.Tree, id);
            }
        }

        return mazeCells;
    }

    public static List<int> FindBoundaryIds(int mazeLength)
    {
        var boundaryIds = new List<int>();
        for(var i = 0; i < mazeLength * mazeLength; i++)
        {
            if (i < mazeLength || i >= (mazeLength-1) * mazeLength || i % mazeLength == 0 || i % mazeLength == mazeLength - 1)
            {
                boundaryIds.Add(i);
            }
        }
        return boundaryIds;
    }
}
