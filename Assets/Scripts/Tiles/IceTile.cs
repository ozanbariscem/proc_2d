using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class IceTile : CustomTile
{
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    #region UNITY EDITOR
    #if UNITY_EDITOR
    [MenuItem("Assets/Create/IceTile")]
    public static void CreateIceTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save IceTile", "New IceTile", "Asset", "Save IceTile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<IceTile>(), path);
    }
    #endif
    #endregion
}
