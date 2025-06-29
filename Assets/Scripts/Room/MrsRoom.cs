using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrsRoom : MonoBehaviour
{
    [SerializeField]
    private GameObject normalScene;

    [SerializeField]
    private GameObject deadScene;

    // Start is called before the first frame update
    void Start()
    {
        EventCenter.GetInstance().AddEventListener("消消乐游戏通过", OnMatchThreeGameFinished);
    }

    private void OnDestroy()
    {
        // 移除事件监听
        EventCenter.GetInstance().RemoveEventListener("消消乐游戏通过", OnMatchThreeGameFinished);
    }

    void OnMatchThreeGameFinished()
    {
        // 切换到死亡状态？
        normalScene.SetActive(false);
        deadScene.SetActive(true);
    }
}
