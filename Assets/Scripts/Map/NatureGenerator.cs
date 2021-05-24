using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NatureGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile tree;
    public Tile stone;

    void SetTilemap()
    {
        GameObject child = transform.Find("Tilemap").gameObject;
        if (child == null)
        {
            child = new GameObject("Tilemap");
            child.transform.parent = this.transform;
            tilemap = child.AddComponent<Tilemap>();
            return;
        }
        tilemap = child.GetComponent<Tilemap>();
    }

    public void PaintTerrain(TerrainTypes terrainType, Vector3Int pos)
    {
        if (tilemap == null)
            SetTilemap();
        switch (terrainType)
        {
            case TerrainTypes.Forest:
                Tile treeTile = tree;
                tilemap.SetTile(pos, tree);
                break;
        }
    }
}
