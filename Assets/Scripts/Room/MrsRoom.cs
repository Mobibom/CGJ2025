using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrsRoom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventCenter.GetInstance().AddEventListener("消消乐游戏完成", OnMatchThreeGameFinished);
    }

    void OnMatchThreeGameFinished()
    {
        // 切换到死亡状态？

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
