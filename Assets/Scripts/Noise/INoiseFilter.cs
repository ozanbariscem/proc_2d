using UnityEngine;

public interface INoiseFilter
{
    float Evaluate(Vector3 point);
    Noise noise {get; set;}
}
