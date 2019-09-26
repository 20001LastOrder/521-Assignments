using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell
{
    public enum CellType
    {
        Tree,
        Empty
    }

    CellType _type;
    public int _id; 
    public CellType Type
    {
        get
        {
            return _type;
        }

        set
        {
            _type = value;
        }
    }

    public int Id => _id;

    public MazeCell(CellType type, int id)
    {
        _type = type;
        _id = id;
    }
}
