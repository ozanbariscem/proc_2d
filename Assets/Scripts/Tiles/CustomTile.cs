using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTile : Tile
{
    public Sprite[] sprites;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = this.sprite;
        var t = tileData.transform;
        // t.SetTRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f * rotation), Vector3.one);
        tileData.transform = transform;
    }
}
