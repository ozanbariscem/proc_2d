using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShapeGenerator
{
    ShapeSettings settings;
    INoiseFilter[] noiseFilters;
    public MinMax elevationMinMax;

    public void UpdateSettings(ShapeSettings settings, Noise noise)
    {
        this.settings = settings;
        this.noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
            noiseFilters[i].noise = noise;
        }
        elevationMinMax = new MinMax();
    }

    public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;

        if (noiseFilters.Length > 0)
        {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (settings.noiseLayers[0].enabled)
            {
                elevation = firstLayerValue;
            }
        }

        for (int i = 1; i < noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enabled)
            {
                float mask = (settings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }
        
        return elevation;
    }

    public float GetScaledElevation(float unscaledElevation)
    {
        float elevation = Mathf.Max(0, unscaledElevation);
        elevation = settings.planetRadius * (1+elevation);
        elevationMinMax.AddValue(elevation);
        return elevation;
    }

    public float GetScaledElevationFromPosition(Vector3 pos)
    {
        return GetScaledElevation(CalculateUnscaledElevation(pos));
    }
}
