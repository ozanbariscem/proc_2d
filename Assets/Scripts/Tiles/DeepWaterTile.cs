using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class DeepWaterTile : CustomTile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    #region UNITY EDITOR
    #if UNITY_EDITOR
    [MenuItem("Assets/Create/DeepWaterTile")]
    public static void CreateDeepWaterTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Deep Water Tile", "New Deep Water Tile", "Asset", "Save Deep Water Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<DeepWaterTile>(), path);
    }
    #endif
    #endregion
}

