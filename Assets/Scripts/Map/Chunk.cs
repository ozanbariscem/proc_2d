using UnityEngine;
using UnityEngine.Tilemaps;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class Chunk
{
    public Map map;
    public byte x, y;
    public Cell[,] cells;
    public Tilemap terrain;
    public Tilemap nature;
    public Tilemap overlay;
    public byte width, height;

    public Chunk(Map map, byte x, byte y, byte width, byte height)
    {
        this.x = x; this.y = y;
        this.width = width; this.height = height;
        cells = new Cell[width, height];
        for (byte i = 0; i < width; i++)
        {
            for (byte j = 0; j < height; j++)
            {
                cells[i, j] = new Cell(i, j, TerrainTypes.DeepWater);
                cells[i, j].chunk = this;
                cells[i, j].map = map;
            }
        }
    }

    public Cell CellExists(byte x, byte y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
            return null;
        else
            return cells[x, y];
    }

    public GameObject SetChunkTransform(byte i, byte j)
    {
        GameObject grid = new GameObject($"Chunk({i}, {j})");
        grid.AddComponent<Grid>();

        terrain = NewTilemap("Terrain", grid.transform);
        nature = NewTilemap("Nature", grid.transform);
        overlay = NewTilemap("Overlay", grid.transform);
        return grid;
    }

    Tilemap NewTilemap(string name, Transform parent = null)
    {
        GameObject obj = new GameObject(name);

        if (parent != null)
            obj.transform.SetParent(parent);

        TilemapRenderer r = obj.AddComponent<TilemapRenderer>(); // This adds Tilemap as well
        if (name == "Overlay") r.sortingOrder = 100;
        return obj.GetComponent<Tilemap>();
    }

    public void PaintTile(Cell cell, BiomeTiles tiles)
    {
        switch (cell.terrainType)
        {
            case TerrainTypes.DeepWater:
                PaintTerrain(cell, tiles.terrainTiles.deepWater);
                break;
            case TerrainTypes.Water:
                PaintTerrain(cell, tiles.terrainTiles.water);
                break;
            case TerrainTypes.Sand:
                PaintTerrain(cell, tiles.terrainTiles.sand);
                break;
            case TerrainTypes.Dirt:
                PaintTerrain(cell, tiles.terrainTiles.dirt);
                break;
            case TerrainTypes.Grass:
                PaintTerrain(cell, tiles.terrainTiles.grass);
                break;
            case TerrainTypes.Forest:
                PaintTerrain(cell, tiles.terrainTiles.forest);
                break;
            case TerrainTypes.Jungle:
                PaintTerrain(cell, tiles.terrainTiles.jungle);
                break;
            case TerrainTypes.Mountain:
                PaintTerrain(cell, tiles.terrainTiles.mountain);
                break;
            case TerrainTypes.MountainTop:
                PaintTerrain(cell, tiles.terrainTiles.mountainTop);
                break;
            case TerrainTypes.Ice:
                PaintTerrain(cell, tiles.terrainTiles.ice);
                break;
            case TerrainTypes.Snow:
                PaintTerrain(cell, tiles.terrainTiles.snow);
                break;
            default:
                Debug.LogError($"Couldn't find tile with type: {cell.terrainType}. This function needs update or better implemantation.");
                break;
        }
    }

    public void PaintTerrain(Cell cell, CustomTile tile)
    {
        cell.PaintTerrain(tile);
    }

    public void PaintTilemap(Tilemap tilemap, Cell cell, CustomTile tile, float[] rgba = null)
    {
        tilemap.SetTile(cell.CellPosition, tile);

        if (rgba != null)
            tilemap.SetColor(cell.CellPosition, new Color(rgba[0], rgba[1], rgba[2], rgba[3]));
    }
}
