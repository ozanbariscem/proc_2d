using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class MountainTopTile : CustomTile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    #region UNITY EDITOR
    #if UNITY_EDITOR
    [MenuItem("Assets/Create/MountainTopTile")]
    public static void CreateMountainTopTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save MountainTopTile", "New MountainTopTile", "Asset", "Save MountainTopTile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<MountainTopTile>(), path);
    }
    #endif
    #endregion
}
