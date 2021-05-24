using UnityEngine;
using MoonSharp.Interpreter;
using System.Collections.Generic;

[MoonSharpUserData]
public class CellObject : ICellObject
{
    public CellObject() { }

    public override void OnCreate(bool bypass_lua = false)
    {
        CellObjectData.AddToCreatedObjects(this);

        if (!bypass_lua)
            data.script.Call(data.on_create_func, this);
    }

    public override void OnBuild(Cell cell, bool bypass_lua = false)
    {
        PlaceToCell(cell);
        CreateTransform();

        if (!bypass_lua)
            data.script.Call(data.on_build_func, this);
    }

    public override void OnDestroy(bool bypass_lua = false)
    {
        if (!bypass_lua)
            data.script.Call(data.on_destroy_func, this);

        DestroyTransform();
        RemoveFromCell();
    }

    private void PlaceToCell(Cell cell)
    {
        for (int i = 0; i < data.width; i++)
        {
            for (int j = 0; j < data.height; j++)
            {
                Cell onCell = cell.map.GetCell(cell.TrueX + i, cell.TrueY + j);

                if (onCell == null)
                {
                    Debug.LogWarning("Tried to place out of map bounds. In an ideal world this shouldn't be possible.");
                }
                else
                {
                    onCell.objectOnCell = this;
                    onCells[i, j] = onCell;
                }
            }
        }
    }

    private void RemoveFromCell()
    {
        for (int i = 0; i < data.width; i++)
        {
            for (int j = 0; j < data.height; j++)
            {
                onCells[i, j].objectOnCell = null;
                onCells[i, j] = null;
            }
        }
    }

    private void CreateTransform()
    {
        if (data.width == 1 && data.height == 1)
        {
            MapGenerator.Instance.tile.sprite = data.sprite_base;

            PlacedCell.chunk.nature.SetTile(PlacedCell.CellPosition, null);
            PlacedCell.chunk.nature.SetTile(PlacedCell.CellPosition, MapGenerator.Instance.tile);
        }
        else
        {
            GameObject obj = new GameObject(data.name);
            obj.transform.position = PlacedCell.TruePosition + new Vector3(data.width / 2f, data.height / 2f, 0);
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = data.sprite_base;
            sr.sortingOrder = 11;

            transform = obj.transform;
        }
    }

    private void DestroyTransform()
    {
        if (data.width == 1 && data.height == 1)
        {
            PlacedCell.chunk.nature.SetTile(PlacedCell.CellPosition, null);
        }
        else
        {
            GameObject.Destroy(transform);
        }
    }
}
