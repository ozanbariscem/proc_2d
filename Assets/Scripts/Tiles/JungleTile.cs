using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class JungleTile : CustomTile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    #region UNITY EDITOR
    #if UNITY_EDITOR
    [MenuItem("Assets/Create/JungleTile")]
    public static void CreateJungleTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save JungleTile", "New JungleTile", "Asset", "Save JungleTile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<JungleTile>(), path);
    }
    #endif
    #endregion
}
