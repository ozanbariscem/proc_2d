using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(), System.Serializable]
public class TileSettings : ScriptableObject
{
    public List<BiomeTiles> tiles;
}
