using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class SnowTile : CustomTile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    #region UNITY EDITOR
    #if UNITY_EDITOR
    [MenuItem("Assets/Create/SnowTile")]
    public static void CreateSnowTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save SnowTile", "New SnowTile", "Asset", "Save SnowTile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SnowTile>(), path);
    }
    #endif
    #endregion
}
