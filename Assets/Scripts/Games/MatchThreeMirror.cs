using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchThreeMirror : MonoBehaviour
{
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
    }

    private void OnMouseDown()
    {
        Debug.Log("MatchThreeMirror clicked!");
        UIManager.GetInstance().ShowPanel<MatchThreeGamePanel>("Games/MatchThreeGamePanel", E_UI_Layer.Top);
    }
}
