using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(), System.Serializable]
public class ColourSettings : ScriptableObject
{
    public GridGenerator.BiomeMode biomeMode;
    public BiomeColourSettings biomeColourSettings;

    [System.Serializable]
    public class BiomeColourSettings
    {
        public Biome biome;

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
            public GridGenerator.BiomeMode biome;

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
        }
    }
}
