using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class DirtTile : CustomTile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    #region UNITY EDITOR
    #if UNITY_EDITOR
    [MenuItem("Assets/Create/DirtTile")]
    public static void CreateDirtTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Dirt Tile", "New Dirt Tile", "Asset", "Save Dirt Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<DirtTile>(), path);
    }
    #endif
    #endregion
}
