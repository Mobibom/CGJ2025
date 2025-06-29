using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // 引入 UnityEvent 命名空间

public class RoomLightSwitchDemoScript : MonoBehaviour
{
    public GameObject SceneLightMangager; // 确保在 Inspector 中赋值

    private SceneLightController slc;
    
    // Start is called before the first frame update
    void Start()
    {
        // 确保 SceneLightMangager 已经赋值，并且其上挂载了 SceneLightController
        if (SceneLightMangager != null)
        {
            slc = SceneLightMangager.GetComponent<SceneLightController>();
            if (slc == null)
            {
                Debug.LogError("RoomLight: 在 SceneLightMangager 上未找到 SceneLightController 组件！");
            }
            else
            {
                slc.LerpAToB(1.5f); // 场景加载后调用 LerpAToB
            }
        }
        else
        {
            Debug.LogError("RoomLight: SceneLightMangager 未赋值！请在 Inspector 中拖拽赋值。");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 公共方法，用于外部调用以触发 onFinished 事件
    public void ChangeToC()
    {
        Debug.Log("RoomLight: 切换至C");
        if (slc == null)
        {
            Debug.LogError("RoomLight: 在 SceneLightMangager 上未找到 SceneLightController 组件！");
        }
        else
        {
            slc.LerpBToC(1.5f); // 场景加载后调用 LerpAToB
        }
    }


}