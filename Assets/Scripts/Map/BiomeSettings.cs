using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(), System.Serializable]
public class BiomeSettings : ScriptableObject
{
    public List<Biome> biomes;
}
