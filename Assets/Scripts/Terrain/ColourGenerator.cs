using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColourGenerator
{
    ColourSettings settings;

    public void UpdateSettings(ColourSettings settings, Noise noise)
    {
        this.settings = settings;
    }
}
