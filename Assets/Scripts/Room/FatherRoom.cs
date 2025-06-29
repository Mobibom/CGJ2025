using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherRoom : MonoBehaviour
{
    public void OnPaintingConversationFinished()
    {
        Debug.Log("已点击画作");
        Vector2 position = new Vector2(-3, -3);
        EventCenter.GetInstance().EventTrigger("初始化华容道", position);
    }
}
