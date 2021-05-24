using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCellGraph
{
    public Dictionary<Cell, PathNode<Cell>> nodes;

    public PathCellGraph(Map map)
    {
        nodes = new Dictionary<Cell, PathNode<Cell>>();

        int chunkAmountX = map.chunks.GetLength(0);
        int chunkAmountY = map.chunks.GetLength(1);
        int chunkSizeX = map.chunks[0, 0].cells.GetLength(0);
        int chunkSizeY = map.chunks[0, 0].cells.GetLength(1);

        for (int i = 0; i < chunkAmountX; i++)
        {
            for (int j = 0; j < chunkAmountY; j++)
            {
                for (int x = 0; x < chunkSizeX; x++)
                {
                    for (int y = 0; y < chunkSizeY; y++)
                    {
                        Cell cell = map.chunks[i, j].cells[x, y];

                        PathNode<Cell> node = new PathNode<Cell>() { data = cell };
                        nodes.Add(cell, node);
                    }
                }
            }
        }

        foreach (var cell in nodes.Keys)
        {
            GenerateEdgesByCell(cell);
        }
    }

    void GenerateEdgesByCell(Cell cell)
    {
        if (cell == null) return;

        PathNode<Cell> node = nodes[cell];
        List<PathEdge<Cell>> edges = new List<PathEdge<Cell>>();

        Cell[] neighbours = cell.GetNeighbours();

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i] != null && neighbours[i].PathfindingCost > 0 && !HasClippingCorner(cell, neighbours[i]))
            {
                PathEdge<Cell> edge = new PathEdge<Cell>();
                edge.cost = neighbours[i].PathfindingCost;
                edge.node = nodes[neighbours[i]];

                edges.Add(edge);
            }
        }

        node.edges = edges.ToArray();
    }

    public void RegenerateGraphAtCell(Cell cell)
    {
        if (cell == null) return;

        GenerateEdgesByCell(cell);
        foreach (Cell neighbour in cell.GetNeighbours())
        {
            GenerateEdgesByCell(neighbour);
        }
    }

    bool HasClippingCorner(Cell cell, Cell neighbour)
    {
        int diffX = cell.TrueX - neighbour.TrueX;
        int diffY = cell.TrueY - neighbour.TrueY;
    
        if (Mathf.Abs(diffX) + Mathf.Abs(diffY) == 2)
        {
            Cell neighbourEW = cell.map.GetCell(cell.TrueX - diffX, cell.TrueY);
            Cell neighbourNS = cell.map.GetCell(cell.TrueX, cell.TrueY - diffY);

            return neighbourEW.PathfindingCost == 0f || neighbourNS.PathfindingCost == 0f;
        }

        return false;
    }
}
