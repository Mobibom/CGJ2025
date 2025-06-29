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
        if (GameObject.Find("Letter(Clone)") == null)
        {
            var instance = ResMgr.GetInstance().Load<GameObject>("Prefab/Games/Letter");
            var letter = instance.GetComponentInChildren<Letter>(true);
            letter.SetFinishedCallback(() =>
            {
                Destroy(instance);
                GameFinished();
            });
        }
    }
    
    private void GameFinished()
    {
        // 查找“女儿”对象
        var priestObj = GameObject.Find("图层1(房间)");
        if (priestObj != null)
        {
            var sr = priestObj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // TODO: 切换到死亡状态女儿图片
                var newSprite = Resources.Load<Sprite>("Textures/Room/GrandmaRoom/祖母死亡");
                if (newSprite != null)
                {
                    sr.sprite = newSprite;
                }
                else
                {
                    Debug.LogWarning("未能加载祖母死亡图片");
                }
            }
            else
            {
                Debug.LogWarning("图层1(房间)没有SpriteRenderer组件");
            }
        }
        else
        {
            Debug.LogWarning("未找到图层1(房间)对象");
        }

        slc.LerpBToC(duration); 
    }
}

