using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class WaterTile : CustomTile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    #region UNITY EDITOR
    #if UNITY_EDITOR
    [MenuItem("Assets/Create/WaterTile")]
    public static void CreateWaterTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Water Tile", "New Water Tile", "Asset", "Save Water Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<WaterTile>(), path);
    }
    #endif
    #endregion
}
