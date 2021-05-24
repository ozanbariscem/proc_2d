using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;

    public int seed = 0;
    public byte chunkAmountX = 4, chunkAmountY = 4;
    public byte chunkWidth = 255, chunkHeight = 255;

    public BiomeTypes biomeType;
    public ShapeSettings shapeSettings;
    public BiomeSettings biomeSettings;
    public TileSettings tileSettings;

    ShapeGenerator shapeGenerator;

    public Map map;

    public CustomTile tile;

    public void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Noise.New(seed);

        shapeGenerator = new ShapeGenerator();
        shapeGenerator.UpdateSettings(shapeSettings, Noise.noise);
        Biome biome = biomeSettings.biomes.SingleOrDefault((x)=>x.biome == biomeType);
        BiomeTiles tiles = tileSettings.tiles.SingleOrDefault((x=>x.biome == biomeType));
        map = Map.Generate(
            this.gameObject, shapeGenerator, biome, tiles,
            chunkAmountX, chunkAmountY, chunkWidth, chunkHeight);
        map.cellGraph = new PathCellGraph(map);

        CameraController.Instance.SetCamera(map.chunkAmountX * map.chunkWidth, map.chunkAmountY * map.chunkHeight);

        #region TESTING
        UnityEngine.Debug.Log("Testing code here.");
        TaskSystem taskSystem = new TaskSystem();
        CharacterController.Instance.Setup(taskSystem);

        CellObject obj = CellObjectData.CreateFromData(CellObjectData.objectDatas["container"]);
        Map.map.GetCell(10, 10).AddObjectToCell(obj);
        #endregion
    }
}
