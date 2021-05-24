using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class FireEffectForPointLight : MonoBehaviour
{
    public Light2D fire;
    public Color startColor;
    public int range = 20;
    public float changeInterval = 0.5f;
    float lastChange;
    float intensity;
    float startIntensity;

    void Start()
    {
        startColor = fire.color;
        intensity = fire.intensity;
        startIntensity = fire.intensity;
    }

    void Update()
    {
        if (Time.time - lastChange >= changeInterval)
        {
            lastChange = Time.time;

            int rndR = Random.Range(-range, range);
            int rndG = Random.Range(-range, range);
            int rndB = Random.Range(-range, range);
            Color color = new Color(
                (startColor.r + rndR/255f), 
                (startColor.g + rndG/255f), 
                (startColor.b + rndB/255f), 1f);
            fire.color = color;

            
        }

        if (Mathf.Abs(fire.intensity - intensity) > 0.02f)
        {
            float diff = intensity - fire.intensity;

            fire.intensity += Mathf.Sign(diff) * Time.deltaTime;
        }
        else
        {
            int rndI = Random.Range(-range, range);
            intensity = startIntensity + rndI/100f;
        }
    }
}
