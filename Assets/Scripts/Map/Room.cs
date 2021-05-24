using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public List<Cell> cells;
    public Color color;

    public Room()
    {
        cells = new List<Cell>();
        color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
    }

    public void AddCell(Cell cell)
    {
        cells.Add(cell);
        if (cell.room != null)
            cell.room.RemoveCell(cell);
        cell.room = this;
    }

    public void RemoveCell(Cell cell)
    {
        cells.Remove(cell);
        cell.room = null;
    }

    public bool Contains(Cell cell)
    {
        return cell.room == this;
    }

    public void FloodFill(Cell cell)
    {
        if (cell.HasRoom) return;
        if (!cell.IsRoomable) return;

        AddCell(cell);
        Queue<Cell> q = new Queue<Cell>();
        q.Enqueue(cell);

        while (q.Count > 0)
        {
            Cell n = q.Dequeue();

            if (n.West != null && !n.West.HasRoom && n.West.IsRoomable)
            {
                AddCell(n.West);
                q.Enqueue(n.West);
            }
            if (n.East != null && !n.East.HasRoom && n.East.IsRoomable)
            {
                AddCell(n.East);
                q.Enqueue(n.East);
            }
            if (n.North != null && !n.North.HasRoom && n.North.IsRoomable)
            {
                AddCell(n.North);
                q.Enqueue(n.North);
            }
            if (n.South != null && !n.South.HasRoom && n.South.IsRoomable)
            {
                AddCell(n.South);
                q.Enqueue(n.South);
            }
        }
    }
}
