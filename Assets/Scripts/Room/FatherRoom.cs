using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherRoom : MonoBehaviour
{
    
    public GameObject SceneLightMangager; // 确保在 Inspector 中赋值
    public Camera MainCamera; // 确保在 Inspector 中赋值

    private SceneLightController slc;
    
    public float duration = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        slc = SceneLightMangager.GetComponent<SceneLightController>();
        slc.LerpAToB(duration); 
    }
    
    
    
    public void OnPaintingConversationFinished()
    {
        Debug.Log("已点击画作");
        Vector2 position = new Vector2(-3, -3);
        EventCenter.GetInstance().EventTrigger("初始化华容道", position);
    }
}
