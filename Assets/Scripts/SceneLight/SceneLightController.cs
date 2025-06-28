using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Reflection; // 确保导入 System.Reflection

public enum ENM_InitialLight
{
    ConfigA,
    ConfigB,
    ConfigC
}

// 确保这个类是存在的并且属性与你的需求匹配
[System.Serializable]


public class SceneLightController : MonoBehaviour
{
    
    [Header("Debug模式,勾上开启下方滑动")]
    public bool debug = false;
    
    [Header("初始光照")]
    public ENM_InitialLight initialLight = ENM_InitialLight.ConfigA;

    [Header("光源列表")]
    
    public List<Light2D> lights;

    [Header("三个光照配置")]
    public List<SceneLightData> configA;
    public List<SceneLightData> configB;
    public List<SceneLightData> configC;

    [Range(0f, 1f)]
    public float slider = 0f;
    
    [Header("切换时长")]
    public float duration = 1f;

    private float lerpTime;
    private float elapsedTime;
    private bool isLerping = false;
    private List<SceneLightData> fromData;
    private List<SceneLightData> toData;

    // 存储 UpdateMesh 方法的 MethodInfo
    //private MethodInfo updateMeshMethod; 

    void Awake()
    {
        
        if (lights.Count != configA.Count || lights.Count != configB.Count || lights.Count != configC.Count)
        {
            Debug.LogError("灯光与配置数量不一致");
            return;
        }

        InitializeConfigColors(configA);
        InitializeConfigColors(configB);
        InitializeConfigColors(configC);
        
        switch (initialLight)
        {
            case ENM_InitialLight.ConfigA:
                ApplyConfigs(configA);
                slider = 0f;
                break;
            case ENM_InitialLight.ConfigB:
                ApplyConfigs(configB);
                slider = 0.5f;
                break;
            case ENM_InitialLight.ConfigC:
                ApplyConfigs(configC);
                slider = 1f;
                break;
        }
    }

    private void InitializeConfigColors(List<SceneLightData> configs)
    {
        if (configs == null) return;

        for (int i = 0; i < configs.Count; i++)
        {
           
            if (configs[i] == null)
            {
                configs[i] = new SceneLightData(); 
            }
            
            if (configs[i].color.a < 0.001f)
            {
                configs[i].color.a = 1.0f;

            }
        }
    }

    public void SlideControl()
    {
        if (slider <= 0.5f)
        {
            float t = slider / 0.5f;
            LerpConfigs(configA, configB, t);
        }
        else
        {
            float t = (slider - 0.5f) / 0.5f;
            LerpConfigs(configB, configC, t);
        }
    }
    
    void Update()
    {
       
        //ApplyLightData(lights[0], configA[0]); 
        //ApplyConfigs(configA);
        //Debug.Log($"Light[0].intensity : {lights[0].intensity} and Check Light intensity : {checkLight.intensity}");

        
        if(debug)
            SlideControl();
        
        if (isLerping)
        {
            Lerping();
        }
    }

    public void Lerping()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / lerpTime);
        LerpConfigs(fromData, toData, t);

        if (t >= 1f)
            isLerping = false;
    }

    public void StartLerp(List<SceneLightData> from, List<SceneLightData> to, float duration)
    {
        if (from.Count != lights.Count || to.Count != lights.Count)
        {
            Debug.LogError("配置数量与灯光数量不一致");
            return;
        }

        fromData = from;
        toData = to;
        lerpTime = duration;
        elapsedTime = 0f;
        isLerping = true;
    }

    public void LerpAToB(float duration) => StartLerp(configA, configB, duration);
    public void LerpBToC(float duration) => StartLerp(configB, configC, duration);
    public void LerpCToA(float duration) => StartLerp(configC, configA, duration);
    
    public void SaveDataToConfigA() => SaveLightDataToConfig(configA);
    public void SaveDataToConfigB() => SaveLightDataToConfig(configB);
    public void SaveDataToConfigC() => SaveLightDataToConfig(configC);

    public void SaveLightDataToConfig(List<SceneLightData> config)
    {

        while (lights.Count > config.Count)
        {
            config.Add(new SceneLightData());
        }
        
        for (int i = 0; i < lights.Count; i++)
        {
            config[i].intensity = lights[i].intensity;
            config[i].color = lights[i].color;
            config[i].fallOff = lights[i].shapeLightFalloffSize;
            config[i].fallOffStrength = lights[i].falloffIntensity;
        }
    }

    private void LerpConfigs(List<SceneLightData> from, List<SceneLightData> to, float t)
    {
        for (int i = 0; i < lights.Count; i++)
        {
            if (lights[i] == null) continue;
            var lerped = SceneLightData.Lerp(from[i], to[i], t);
            ApplyLightData(lights[i], lerped);
        }
    }

    public void ApplyConfigs(List<SceneLightData> configs)
    {
        for (int i = 0; i < lights.Count; i++)
        {
            ApplyLightData(lights[i], configs[i]);
            
        }
    }

    private void ApplyLightData( Light2D light,  SceneLightData data)
    {
        if (light == null) return;

        if (Mathf.Abs(light.intensity - data.intensity) > 0.001f) // Use a small epsilon for float comparison
        {
            light.intensity = data.intensity;
        }

        // Only update color if it has changed significantly
        if (light.color != data.color)
        {
            light.color = data.color;
        }
        
        if (Mathf.Abs(light.falloffIntensity - data.fallOffStrength) > 0.001f) // Use a small epsilon for float comparison
        {
            light.falloffIntensity = data.fallOffStrength;
        }
        
        if (Mathf.Abs(light.shapeLightFalloffSize - data.fallOff) > 0.001f) // Use a small epsilon for float comparison
        {
            light.shapeLightFalloffSize = data.fallOff;
        }

        // if (light.lightType != Light2D.LightType.Global)
        // {
        //     if (Mathf.Abs(light.falloffIntensity - data.fallOffStrength) > 0.001f) // Use a small epsilon for float comparison
        //     {
        //         light.falloffIntensity = data.fallOffStrength;
        //     }
        //
        //     if (light.lightType == Light2D.LightType.Freeform)
        //     {
        //         if (Mathf.Abs(light.shapeLightFalloffSize - data.fallOff) > 0.001f) // Use a small epsilon for float comparison
        //         {
        //             light.shapeLightFalloffSize = data.fallOff;
        //         }
        //     }
        // }
    }
}