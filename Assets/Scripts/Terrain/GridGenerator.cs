using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class GridGenerator : MonoBehaviour
{
    public enum RenderMode
    {
        Noise, Terrain, Texture
    }
    public enum BiomeMode
    {
        None, Arctic, Desert, Jungle
    }

    public bool autoGenerate = false;
    public bool setNewSeed = false;
    public bool paintNature = false;
    public string seed;

    public int worldSize = 100;

    public RenderMode renderMode;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;
    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;
    public BiomeSettings biomeSettings;

    public List<ColourSettings.BiomeColourSettings.Biome> biomes;
    ShapeGenerator shapeGenerator;
    [SerializeField, HideInInspector]
    public ColourGenerator colourGenerator;

    Noise noise;
    [SerializeField, HideInInspector]
    Tilemap terrainTilemap;
    [SerializeField]
    public NatureGenerator natureGenerator;

    public List<Tile> tiles;
    public InputField worldSizeInputField;

    public void OnValidate()
    {
        if (autoGenerate)
        {
            if (setNewSeed || seed == "")
            {
                seed = System.DateTime.Now.ToString();
                noise = new Noise(seed.GetHashCode());
            }
            if (noise == null)
                noise = new Noise(seed.GetHashCode());

            if (terrainTilemap == null)
            {
                GameObject terrain = new GameObject("Tilemap");
                terrain.transform.position = Vector3.zero;
                terrain.transform.parent = this.transform;
                terrainTilemap = terrain.AddComponent<Tilemap>();
                terrain.AddComponent<TilemapRenderer>();
            }

            OnShapeSettingsUpdated();
            OnColourSettingsUpdated();
            Generate();
            Camera.main.transform.position = new Vector3(worldSize / 2, worldSize / 2, -10);
            Camera.main.orthographicSize = worldSize / 2;
        }
    }

    public void OnShapeSettingsUpdated()
    {
        if (shapeGenerator == null)
        {
            shapeGenerator = new ShapeGenerator();
        }

        shapeGenerator.UpdateSettings(shapeSettings, noise);
    }

    public void OnColourSettingsUpdated()
    {
        foreach (var biome in biomes)
        {
            if (biome.biome == colourSettings.biomeMode)
            {
                colourSettings.biomeColourSettings.biome = biome;
                break;
            }
        }

        colourGenerator.UpdateSettings(colourSettings, noise);
    }

    public void Generate()
    {
        terrainTilemap.ClearAllTiles();
        natureGenerator.tilemap.ClearAllTiles();
        int prevSize = terrainTilemap.size.x;
        terrainTilemap.size = new Vector3Int(worldSize, worldSize, 0);
        for (int i = 0; i < prevSize || i < worldSize; i++)
        {
            for (int j = 0; j < prevSize || j < worldSize; j++)
            {
                // if this tile already exists we only need to change its color 
                // if not we need a new tile at this location
                // and if this tile is now out of our world size we should null it
                Vector3Int pos = new Vector3Int(i, j, 0);
                float value = shapeGenerator.CalculateUnscaledElevation(pos);
                value = shapeGenerator.GetScaledElevation(value);
                Tile tile;
                if ((tile = terrainTilemap.GetTile<Tile>(pos)) != null)
                {
                    if (i < worldSize && j < worldSize) // yes
                    {
                        Paint(tile, pos, value - 1);
                    }
                    else // no
                    {
                        terrainTilemap.SetTile(pos, null);
                    }
                }
                else
                {
                    tile = tiles[0];
                    tile.flags = TileFlags.None;
                    terrainTilemap.SetTile(pos, tile);
                    Paint(tile, pos, value - 1);
                }
            }
        }
    }

    void Paint(Tile tile, Vector3Int pos, float value)
    {
        Color color = Color.magenta;
        ColourSettings.BiomeColourSettings.Biome biome = colourSettings.biomeColourSettings.biome;
        ColourSettings.BiomeColourSettings.Biome.Range range = biome.Evaluate(value);
        switch (renderMode)
        {
            case RenderMode.Noise:
                color.r = value; color.g = value; color.b = value; color.a = 1f;
                terrainTilemap.SetColor(pos, color);
                break;
            case RenderMode.Terrain:
                color = range.color;
                terrainTilemap.SetColor(pos, color);
                break;
            case RenderMode.Texture:
                switch (range.type)
                {
                    case TerrainTypes.DeepWater:
                        tile = tiles[1];
                        break;
                    case TerrainTypes.Water:
                        tile = tiles[1];
                        break;
                    case TerrainTypes.Sand:
                        tile = tiles[2];
                        break;
                    case TerrainTypes.Grass:
                        tile = tiles[3];
                        break;
                    case TerrainTypes.Forest:
                        tile = tiles[3];
                        break;
                    case TerrainTypes.Mountain:
                        tile = tiles[4];
                        break;
                    case TerrainTypes.MountainTop:
                        tile = tiles[4];
                        break;
                    case TerrainTypes.Snow:
                        tile = tiles[5];
                        break;
                }
                terrainTilemap.SetTile(pos, null);
                terrainTilemap.SetTile(pos, tile);
                break;
        }

        if (paintNature)
        {
            natureGenerator.PaintTerrain(range.type, pos);
        }
    }

    void Start()
    {
        noise = new Noise(seed.GetHashCode());
    }

    public void SetSeed(string value)
    {
        this.seed = value;
        noise = new Noise(seed.GetHashCode());
    }

    public void SetWorldSize(string value)
    {
        int _value = -1;
        int.TryParse(value, out _value);

        if (_value < 100)
        {
            _value = 100;
        }
        else if (_value > 500)
        {
            _value = 500;
        }
        this.worldSize = _value;
    }

    public void SetBiome(int value)
    {
        colourSettings.biomeMode = (BiomeMode)value;
    }

    public void SetRenderMode(int value)
    {
        renderMode = (RenderMode)value;
    }

    public void CorrectWorldSizeInputField()
    {
        worldSizeInputField.text = worldSize.ToString();
    }

    public void GenerateButton()
    {
        if (noise == null)
            noise = new Noise(seed.GetHashCode());
        OnShapeSettingsUpdated();
        OnColourSettingsUpdated();
        Generate();
        Camera.main.transform.position = new Vector3(worldSize / 2, worldSize / 2, -10);
        Camera.main.orthographicSize = worldSize / 2;
    }
}
