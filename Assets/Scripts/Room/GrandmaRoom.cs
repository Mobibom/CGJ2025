using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandmaRoom : MonoBehaviour
{
    
    
    public GameObject SceneLightMangager; // 确保在 Inspector 中赋值

    private SceneLightController slc;
    
    public float duration = 5.0f;
    
    
    void Start()
    {
        slc = SceneLightMangager.GetComponent<SceneLightController>();
        slc.LerpAToB(duration); 
    }
    
    // 小游戏成功后调用slc.LerpBToc(1.5f);
    
    public void OnFireplaceFinished()
    {
        Debug.Log("壁炉对话已结束");
        var instance = ResMgr.GetInstance().Load<GameObject>("Prefab/Games/Letter");
    }
}
