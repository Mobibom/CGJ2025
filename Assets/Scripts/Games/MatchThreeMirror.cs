using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchThreeMirror : MonoBehaviour
{
    private bool isGameFinished = false;

    void Start()
    {
        // 如果没有 EventSystem，则创建一个
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // 生成一个 SelectionDot
        GameObject dot = ResMgr.GetInstance().Load<GameObject>("Prefab/SelectionDot");
        if (dot != null)
        {
            dot.transform.SetParent(transform, false);
            dot.transform.localPosition = new Vector3(0, 0, -0.2f);
        }
        else
        {
            Debug.LogError("Dot prefab not found!");
        }

        EventCenter.GetInstance().AddEventListener("消消乐游戏完成", OnGameFinished);
    }

    private void OnDestroy()
    {
        // 移除事件监听
        EventCenter.GetInstance().RemoveEventListener("消消乐游戏完成", OnGameFinished);
    }

    private void OnGameFinished()
    {
        isGameFinished = true;
    }

    public void ShowGame()
    {
        if (isGameFinished)
        {
            return;
        }

        UIManager.GetInstance().ShowPanel<MatchThreeGamePanel>("Games/MatchThreeGamePanel", E_UI_Layer.Top);
    }
}
