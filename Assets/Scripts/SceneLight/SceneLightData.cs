using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneLightData
{
    public Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public float intensity = 1f;
    
    // public float falloffIntensity = 1f;
    // public float innerAngle = 20f;
    // public float outerAngle = 45f;
    // public float radius = 5f;
    // public int segments = 10;

    public static SceneLightData Lerp(SceneLightData a, SceneLightData b, float t)
    {
        return new SceneLightData
        {
            color = Color.Lerp(a.color, b.color, t),
            intensity = Mathf.Lerp(a.intensity, b.intensity, t),
            // falloffIntensity = Mathf.Lerp(a.falloffIntensity, b.falloffIntensity, t),
            // innerAngle = Mathf.Lerp(a.innerAngle, b.innerAngle, t),
            // outerAngle = Mathf.Lerp(a.outerAngle, b.outerAngle, t),
            // radius = Mathf.Lerp(a.radius, b.radius, t),
            // segments = Mathf.RoundToInt(Mathf.Lerp(a.segments, b.segments, t))
        };
    }
}

