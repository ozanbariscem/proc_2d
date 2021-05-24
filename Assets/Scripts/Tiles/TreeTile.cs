using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class TreeTile : CustomTile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {   
        int rndI = Random.Range(0, sprites.Length);
        tileData.sprite = sprites[rndI];
        tileData.color = Color.green;
        
        // base.GetTileData(position, tilemap, ref tileData);
    }

    #region UNITY EDITOR
    #if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a RoadTile Asset
    [MenuItem("Assets/Create/TreeTile")]
    public static void CreateRoadTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Tree Tile", "New Tree Tile", "Asset", "Save Tree Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TreeTile>(), path);
    }
    #endif
    #endregion
}