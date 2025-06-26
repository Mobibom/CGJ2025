using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using RoomData;
using UnityEditor;

public class RoomJsonReader : MonoBehaviour
{
    [Header("JSON 文件")]
    public TextAsset jsonFile;
    
    [Header("房间配置")]
    public List<RoomBlock> blocks = new List<RoomBlock>();
    
    [Header("基础材质")]
    public Material baseMaterial;
    
    [Header("子材质存储路径")]
    public string variantMatSavePath = "Assets/Materials/Variant";
    
    [Header("高亮设置")]
    [Range(0.01f, 5f)]
    public float highlightTransitionTime = 0.3f;
    public float hightStrength = 0.15f;

    [ContextMenu("⚙️ 生成房间")]
    public void GenerateRooms()
    {
        if (jsonFile == null)
        {
            Debug.LogError("❌ 没有指定 JSON 文件");
            return;
        }

        string jsonText = jsonFile.text;
        JObject root = JObject.Parse(jsonText);
        var skins = root["skins"]?["default"];

        if (skins == null)
        {
            Debug.LogError("❌ 无法读取 skins.default");
            return;
        }
        
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        foreach (RoomBlock block in blocks)
        {
            if (block.sprite == null)
            {
                Debug.LogWarning($"⚠️ Room '{block.name}' 没有配置 Sprite，跳过");
                continue;
            }
            
            if (baseMaterial == null)
            {
                Debug.LogError("❌ 没有指定基础材质");
                return;
            }
            
            var data = skins[block.name]?[block.name];
            if (data == null)
            {
                Debug.LogWarning($"⚠️ JSON 中没有找到房间：{block.name}");
                continue;
            }
            
            
            EnsureFolderExists(variantMatSavePath);
            
            float x = data["x"]?.Value<float>() ?? 0;
            float y = data["y"]?.Value<float>() ?? 0;

            GameObject go = new GameObject(block.name);
            go.transform.SetParent(this.transform, false);
            go.transform.localPosition = new Vector3(x / 100f, y / 100f, 0); // 按需缩放

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = block.sprite;
            sr.sortingOrder = block.orderInLayer;
            
            string variantPath = $"{variantMatSavePath}/{block.name}_Mat.mat";

            Material variantMat = AssetDatabase.LoadAssetAtPath<Material>(variantPath);
            
            if (variantMat == null)
            {
                variantMat = new Material(baseMaterial);
                AssetDatabase.CreateAsset(variantMat, variantPath);
                Debug.Log($"🆕 生成材质 variant: {variantPath}");
            }

            sr.material = variantMat;
            var highLighter = go.AddComponent<RoomHighlighter>();
            highLighter.hightLightStrength =  hightStrength;
            
            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true; 

        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ 房间生成完毕");
    }
    
    private void EnsureFolderExists(string fullPath)
    {
        string[] parts = fullPath.Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = parts[i];
            string parent = current;
            current += "/" + next;
            if (!AssetDatabase.IsValidFolder(current))
            {
                AssetDatabase.CreateFolder(parent, next);
            }
        }
    }
    
}