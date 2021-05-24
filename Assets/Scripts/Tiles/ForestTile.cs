using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class ForestTile : CustomTile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    #region UNITY EDITOR
    #if UNITY_EDITOR
    [MenuItem("Assets/Create/ForestTile")]
    public static void CreateForestTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Forest Tile", "New Forest Tile", "Asset", "Save Forest Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ForestTile>(), path);
    }
    #endif
    #endregion
}
