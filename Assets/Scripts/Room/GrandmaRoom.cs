using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandmaRoom : MonoBehaviour
{
    public void OnFireplaceFinished()
    {
        Debug.Log("壁炉对话已结束");
        var instance = ResMgr.GetInstance().Load<GameObject>("Prefab/Games/Letter");
    }
}
