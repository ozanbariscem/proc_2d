using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class SandTile : CustomTile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    #region UNITY EDITOR
    #if UNITY_EDITOR
    [MenuItem("Assets/Create/SandTile")]
    public static void CreateSandTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Sand Tile", "New Sand Tile", "Asset", "Save Sand Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SandTile>(), path);
    }
    #endif
    #endregion
}
