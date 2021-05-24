using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class GrassTile : CustomTile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    #region UNITY EDITOR
#if UNITY_EDITOR
    [MenuItem("Assets/Create/GrassTile")]
    public static void CreateGrassTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Grass Tile", "New Grass Tile", "Asset", "Save Grass Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GrassTile>(), path);
    }
#endif
    #endregion
}
