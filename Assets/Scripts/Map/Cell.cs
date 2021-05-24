using System;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Cell
{
    public Map map;
    public Chunk chunk;
    public Room room;

    public byte x, y;

    public CellObject objectOnCell;
    public List<Item> itemsOnCell;
    public Cell[] neighbours;
    
    public Cell North { get { return map.GetCell(TrueX, TrueY + 1); } }
    public Cell South { get { return map.GetCell(TrueX, TrueY - 1); } }
    public Cell East { get { return map.GetCell(TrueX + 1, TrueY); } }
    public Cell West { get { return map.GetCell(TrueX - 1, TrueY); } }

    public int TrueX { get { return (chunk.x * chunk.width) + x; } }
    public int TrueY { get { return (chunk.y * chunk.height) + y; } }

    public bool IsRoomable { get { return PathfindingCost != 0; } }
    public bool HasRoom { get { return room != null; } }

    public Vector3Int TruePosition
    {
        get
        {
            return new Vector3Int(TrueX, TrueY, 0);
        }
    }
    public Vector3Int CellPosition
    {
        get
        {
            return new Vector3Int(x, y, 0);
        }
    }
    public TerrainTypes terrainType;

    public float PathfindingCost
    {
        get
        {
            float value = 0;
            switch (terrainType)
            {
                case TerrainTypes.DeepWater:
                    value = 0;
                    break;
                case TerrainTypes.Water:
                    value = 0;
                    break;
                case TerrainTypes.Sand:
                    value = 2;
                    break;
                case TerrainTypes.Grass:
                    value = 1;
                    break;
                case TerrainTypes.Dirt:
                    value = 1;
                    break;
                case TerrainTypes.Forest:
                case TerrainTypes.Jungle:
                    value = 1;
                    break;
                case TerrainTypes.Mountain:
                case TerrainTypes.MountainTop:
                case TerrainTypes.Ice:
                case TerrainTypes.Snow:
                    value = 2;
                    break;
            }

            if (objectOnCell != null)
            {
                if (objectOnCell.data.PathfindingCost == 0) return 0;
                else value += objectOnCell.data.PathfindingCost;
            }
            return value;
        }
    }
    public string Info
    {
        get
        {
            return $"({x}, {y}), {terrainType}";
        }
    }

    public Cell(byte x, byte y, TerrainTypes terrainType)
    {
        this.x = x; this.y = y;
        this.terrainType = terrainType;
    }

    public Cell[] GetNeighbours()
    {
        Cell[] neighbours = new Cell[8];

        int x = TrueX; int y = TrueY;
        neighbours[0] = map.GetCell(x, y + 1);
        neighbours[1] = map.GetCell(x + 1, y);
        neighbours[2] = map.GetCell(x, y - 1);
        neighbours[3] = map.GetCell(x - 1, y);

        neighbours[4] = map.GetCell(x + 1, y + 1);
        neighbours[5] = map.GetCell(x + 1, y - 1);
        neighbours[6] = map.GetCell(x - 1, y - 1);
        neighbours[7] = map.GetCell(x - 1, y + 1);
        return neighbours;
    }

    /// <summary>
    /// Assumes the whole map is set, dont call this while generating the map.
    /// </summary>
    /// <returns>Neighbour array</returns>
    public Cell[] SetNeighbours()
    {
        if (neighbours == null)
            neighbours = GetNeighbours();
        return neighbours;
    }

    public void SetTerrain(TerrainTypes type)
    {
        this.terrainType = type;
    }

    public void SetNature(TerrainTypes type)
    {
        switch (type)
        {
            case TerrainTypes.DeepWater:
                break;
            case TerrainTypes.Water:
                break;
            case TerrainTypes.Sand:
                break;
            case TerrainTypes.Dirt:
                break;
            case TerrainTypes.Grass:
                break;
            case TerrainTypes.Forest:
            case TerrainTypes.Jungle:
                AddObjectToCell(CellObjectData.CreateFromData(CellObjectData.objectDatas["forest_tree"]));
                break;
            case TerrainTypes.Mountain:
            case TerrainTypes.MountainTop:
            case TerrainTypes.Ice:
            case TerrainTypes.Snow:
                AddObjectToCell(CellObjectData.CreateFromData(CellObjectData.objectDatas["stone"]));
                break;
            default:
                Debug.LogError($"Couldn't find tile with type: {type.ToString()}. This function needs update or better implemantation.");
                break;
        }
    }

    public bool AddObjectToCell(CellObject obj, bool bypass_lua = false)
    {
        if (objectOnCell != null)
        {
            Debug.Log("Cell is not empty.");
            return false;
        }
        else
        {
            obj.OnBuild(this, bypass_lua);
            ModifyPathfindingGraph(obj.onCells);
            return true;
        }
    }

    public bool RemoveObjectOnCell(bool bypass_lua = false)
    {
        if (objectOnCell == null)
        {
            Debug.Log("Cell is empty.");
            return false;
        }
        else
        {
            Cell[,] wasOnCells = objectOnCell.onCells.Clone() as Cell[,];
            objectOnCell.OnDestroy(bypass_lua);
            ModifyPathfindingGraph(wasOnCells);
            return true;
        }
    }

    public bool AddItemToCell(Item item)
    {
        if (itemsOnCell == null)
            itemsOnCell = new List<Item>();
        itemsOnCell.Add(item);
        return true;
    }

    public bool RemoveItemFromCell(Item item)
    {
        if (itemsOnCell == null)
        {
            itemsOnCell = new List<Item>();
            return false;
        }
        else
        {
            return itemsOnCell.Remove(item);
        }
    }

    public void ModifyPathfindingGraph(Cell[,] cells)
    {
        if (this.map.cellGraph != null)
        {
            foreach (var cell in cells)
            {
                this.map.cellGraph.RegenerateGraphAtCell(cell);
            }
        }
    }

    public float FlatDistanceTo(Cell target)
    {
        return Mathf.Sqrt(Mathf.Pow(Mathf.Abs(target.TrueX - TrueX), 2) + Mathf.Pow(Mathf.Abs(target.TrueY - TrueY), 2));
    }

    public CellObject ClosestContainer()
    {
        if (!CellObjectData.createdObjects.ContainsKey("container")) return null;

        CellObject closest = CellObjectData.createdObjects["container"][0];
        foreach (var con in CellObjectData.createdObjects["container"])
        {
            if (FlatDistanceTo(con.PlacedCell) < FlatDistanceTo(closest.PlacedCell))
                closest = con;
        }
        return closest;
    }

    public Item ClosestItemOfTypeWithAmount(string tag, ushort amount)
    {
        if (!ItemData.createdItems.ContainsKey(tag)) return null;

        Item closest = ItemData.createdItems[tag][0];
        foreach (var _item in ItemData.createdItems[tag])
        {
            if (_item.amount >= amount && FlatDistanceTo(_item.onCell) < FlatDistanceTo(this))
            {
                closest = _item;
            }
        }
        return closest;
    }

    #region Painting
    public int[] NumberOfNeighboursWithTerrainType(Cell[] neighbours)
    {
        int[] totalNeighbours = NumberOfDirectNeighboursWithTerrainType(neighbours);
        int[] diagonalNeighbours = NumberOfDiagonalNeighboursWithTerrainType(neighbours);
        for (int i = 0; i < totalNeighbours.Length; i++)
        {
            totalNeighbours[i] += diagonalNeighbours[i];
        }
        return totalNeighbours;
    }

    public int[] NumberOfDirectNeighboursWithTerrainType(Cell[] neighbours)
    {
        int terrainTypeAmount = Enum.GetValues(typeof(TerrainTypes)).Length;
        int[] directNeighbours = new int[terrainTypeAmount];
        for (int i = 0; i < 4; i++)
        {
            if (neighbours[i] != null)
            {
                directNeighbours[(int)neighbours[i].terrainType]++;
            }
        }
        return directNeighbours;
    }

    public int[] NumberOfDiagonalNeighboursWithTerrainType(Cell[] neighbours)
    {
        int terrainTypeAmount = Enum.GetValues(typeof(TerrainTypes)).Length;
        int[] diagonalNeighbours = new int[terrainTypeAmount];
        for (int i = 0; i < 4; i++)
        {
            if (neighbours[4 + i] != null)
            {
                diagonalNeighbours[(int)neighbours[i].terrainType]++;
            }
        }
        return diagonalNeighbours;
    }

    public bool IsCornerToTerrainType(Cell[] neighbours, TerrainTypes type, int[] numberOfDirectNeighboursWithTerrainType = null, int[] numberOfDiagonalNeighboursWithTerrainType = null)
    {
        if (numberOfDirectNeighboursWithTerrainType == null)
            numberOfDirectNeighboursWithTerrainType = NumberOfDirectNeighboursWithTerrainType(neighbours);
        if (numberOfDiagonalNeighboursWithTerrainType == null)
            numberOfDiagonalNeighboursWithTerrainType = NumberOfDiagonalNeighboursWithTerrainType(neighbours);

        return
            numberOfDiagonalNeighboursWithTerrainType[(int)type] >= 1 &&
            numberOfDirectNeighboursWithTerrainType[(int)type] >= 2;
    }

    public int CornerIndexToType(Cell[] neighbours, TerrainTypes type)
    {
        // Works clockwise
        for (int i = 0; i < 4; i++)
        {
            if (neighbours[0 + i] == null || neighbours[4 + i] == null || neighbours[1 + i] == null) continue;
            if (neighbours[0 + i].terrainType == type &&
                neighbours[4 + i].terrainType == type &&
                neighbours[1 + i].terrainType == type)
            {
                return i;
            }
        }
        return -1;
    }

    public void PaintTerrain(CustomTile tile)
    {
        int typeAmount = Enum.GetValues(typeof(TerrainTypes)).Length;

        Cell[] neighbours = GetNeighbours();
        int[] numberOfDirectNeighboursWithTerrainType = NumberOfDirectNeighboursWithTerrainType(neighbours);
        int[] numberOfDiagonalNeighboursWithTerrainType = NumberOfDiagonalNeighboursWithTerrainType(neighbours);

        tile.sprite = tile.sprites[0];
        for (int i = 0; i < typeAmount; i++)
        {
            TerrainTypes type = (TerrainTypes)i;
            if (type == this.terrainType) continue; // This is a cool way to only paint edges if you only set sprite in if conditions

            if (IsCornerToTerrainType(neighbours, type, numberOfDirectNeighboursWithTerrainType, numberOfDiagonalNeighboursWithTerrainType))
            {
                int rotation = CornerIndexToType(neighbours, type);
                tile.sprite = tile.sprites[i + 1]; // Because 0 is itself;

                var t = tile.transform;
                t.SetTRS(Vector3.zero, Quaternion.Euler(0f, 0f, -90f * rotation), Vector3.one);
                tile.transform = t;
                break;
            }
        }
        chunk.terrain.SetTile(CellPosition, null);
        chunk.terrain.SetTile(CellPosition, tile);
    }
    #endregion
}
