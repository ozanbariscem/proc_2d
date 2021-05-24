using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Biome
{
    [System.Serializable]
    public class Range
    {
        [Range(0.0f, 1f)]
        public float start;
        public Color color;
        public TerrainTypes type;
    }

    public List<Range> ranges;
    public BiomeTypes biome;

    public Range Evaluate(float value)
    {
        if (value < 0)
            return ranges[0];
        for (int i = ranges.Count - 1; i >= 0; i--)
        {
            if (value >= ranges[i].start)
            {
                return ranges[i];
            }
        }
        return null;
    }

    public Color EvaluateColor(float value)
    {
        Range range = Evaluate(value);

        if (range != null)
            return range.color;
        else
            return Color.magenta;
    }

    public static Biome FromOldBiomeClass(ColourSettings.BiomeColourSettings.Biome old)
    {
        Biome biome = new Biome();
        biome.biome = (BiomeTypes)old.biome;

        biome.ranges = new List<Range>();
        foreach (var range in old.ranges)
        {
            Range newRange = new Range();
            newRange.start = range.start;
            newRange.color = range.color;
            newRange.type = range.type;

            biome.ranges.Add(newRange);
        }
        return biome;
    }

    public static List<Biome> FromOldBiomeClass(List<ColourSettings.BiomeColourSettings.Biome> old)
    {
        List<Biome> biomes = new List<Biome>();

        foreach (var oldBiome in old)
        {
            biomes.Add(FromOldBiomeClass(oldBiome));
        }
    
        return biomes;
    }
}
