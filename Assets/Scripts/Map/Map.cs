using UnityEngine;
using System.Collections.Generic;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Map
{
    public static Map map;

    public GameObject obj;
    public Chunk[,] chunks;
    public List<Room> rooms;
    public PathCellGraph cellGraph;

    public byte chunkAmountX, chunkAmountY;
    public byte chunkWidth, chunkHeight;

    public Map(GameObject parent, byte chunkAmountX, byte chunkAmountY, byte chunkWidth, byte chunkHeight)
    {
        obj = parent;
        chunks = new Chunk[chunkAmountX, chunkAmountY];
        this.chunkAmountX = chunkAmountX; this.chunkAmountY = chunkAmountY;
        this.chunkWidth = chunkHeight; this.chunkHeight = chunkHeight;

        map = this;
    }

    public Cell GetCell(int x, int y)
    {
        byte chunkLocX = (byte)(x / chunkWidth);
        byte chunkLocY = (byte)(y / chunkHeight);
        
        Chunk chunk = ChunkExists(chunkLocX, chunkLocY);
        if (chunk != null)
        {
            byte cellLocX = (byte)(x % chunkWidth);
            byte cellLocY = (byte)(y % chunkHeight);

            Cell cell = chunk.CellExists(cellLocX, cellLocY);
            return cell;
        }
        return null;
    }

    public Chunk ChunkExists(byte x, byte y)
    {
        if (x < 0 || y < 0 || x >= chunkAmountX || y >= chunkAmountY)
            return null;
        else
            return chunks[x, y];
    }

    public void DeveloperOverlayRender(DeveloperMode.OverlayRenderMode mode)
    {
        switch (mode)
        {
            case DeveloperMode.OverlayRenderMode.none:
                foreach (var chunk in chunks)
                {
                    foreach (var cell in chunk.cells)
                    {
                        chunk.PaintTilemap(chunk.overlay, cell, null);
                    }
                }
                break;
            case DeveloperMode.OverlayRenderMode.is_walkable:
                MapGenerator.Instance.tile.sprite = MapGenerator.Instance.tile.sprites[0];
                foreach (var chunk in chunks)
                {
                    foreach (var cell in chunk.cells)
                    {
                        chunk.PaintTilemap(chunk.overlay, cell, MapGenerator.Instance.tile, 
                            cell.PathfindingCost == 0 ? new float[4]{ 0, 0, 0, .8f } : new float[4]{ 1, 1, 1, .8f } );
                    }
                }
                break;
            case DeveloperMode.OverlayRenderMode.room:
                MapGenerator.Instance.tile.sprite = MapGenerator.Instance.tile.sprites[0];
                foreach (var room in rooms)
                {
                    foreach (var cell in room.cells)
                    {
                        cell.chunk.PaintTilemap(cell.chunk.overlay, cell, MapGenerator.Instance.tile, 
                            new float[4] {room.color.r, room.color.g, room.color.b, .8f} );
                    }
                }
                break;
        }
    }

    public void FloodFill()
    {
        rooms = new List<Room>();

        for (byte i = 0; i < chunkAmountX; i++)
        {
            for (byte j = 0; j < chunkAmountY; j++)
            {
                for (byte x = 0; x < chunkWidth; x++)
                {
                    for (byte y = 0; y < chunkHeight; y++)
                    {
                        if (!chunks[i, j].cells[x, y].HasRoom && chunks[i, j].cells[x, y].IsRoomable)
                        {
                            Room room = new Room();
                            room.FloodFill(chunks[i, j].cells[x, y]);
                            if (room.cells.Count > 0)
                            {
                                rooms.Add(room);
                                Debug.Log($"Room with cell count: {room.cells.Count}, added to map.rooms.");
                            }
                        }
                    }
                }
            }
        }
    }

    public static Map Generate(GameObject parent, ShapeGenerator shapeGenerator, Biome biome, BiomeTiles tiles, byte chunkAmountX, byte chunkAmountY, byte chunkWidth, byte chunkHeight)
    {
        Map map = new Map(parent, chunkAmountX, chunkAmountY, chunkWidth, chunkHeight);

        for (byte i = 0; i < chunkAmountX; i++)
        {
            for (byte j = 0; j < chunkAmountY; j++)
            {
                map.chunks[i, j] = new Chunk(map, i, j, chunkWidth, chunkHeight);
                GameObject grid = map.chunks[i, j].SetChunkTransform(i, j);
                grid.transform.SetParent(map.obj.transform);
                grid.transform.localPosition = new Vector3(i * chunkWidth, j * chunkHeight, 0); 

                for (byte x = 0; x < chunkWidth; x++)
                {
                    for (byte y = 0; y < chunkHeight; y++)
                    {
                        Vector3 position = new Vector3((i * chunkWidth) + x, (j * chunkWidth) + y, 0);
                        float value = shapeGenerator.GetScaledElevationFromPosition(position) - 1; // We need something better here
                        TerrainTypes terrainType = biome.Evaluate(value).type;
                        map.chunks[i, j].cells[x, y].SetTerrain(terrainType);
                    }
                }
            }
        }

        for (byte i = 0; i < chunkAmountX; i++)
        {
            for (byte j = 0; j < chunkAmountY; j++)
            {
                for (byte x = 0; x < chunkWidth; x++)
                {
                    for (byte y = 0; y < chunkHeight; y++)
                    {
                        map.chunks[i, j].PaintTile(map.chunks[i, j].cells[x, y], tiles);
                        map.chunks[i, j].cells[x, y].SetNature(map.chunks[i, j].cells[x, y].terrainType);
                    }
                }
            }
        }

        map.FloodFill();
        return map;
    }
}
