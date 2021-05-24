using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class MountainTile : CustomTile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    #region UNITY EDITOR
    #if UNITY_EDITOR
    [MenuItem("Assets/Create/MountainTile")]
    public static void CreateMountainTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save MountainTile", "New MountainTile", "Asset", "Save MountainTile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MountainTile>(), path);
    }
    #endif
    #endregion
}
