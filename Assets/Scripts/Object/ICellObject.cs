using UnityEngine;
using System.Linq;

public abstract class ICellObject
{
    public CellObjectData data;
    public Cell[,] onCells;
    public ushort durability;
    public uint capacity;
    public Transform transform;

    /// <summary>
    /// Actual cell this object was placed on, basically onCells[0, 0]
    /// </summary>
    public Cell PlacedCell
    {
        get { return onCells[0, 0]; }
    }

    public abstract void OnCreate(bool bypass_lua = false);
    public abstract void OnBuild(Cell cell, bool bypass_lua = false);
    public abstract void OnDestroy(bool bypass_lua = false);
}
