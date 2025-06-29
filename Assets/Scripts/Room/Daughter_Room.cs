using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectBase.Subtitle;

public class Daughter_Room : MonoBehaviour
{
    
    public GameObject SceneLightMangager; // 确保在 Inspector 中赋值
    public Camera MainCamera; // 确保在 Inspector 中赋值

    private SceneLightController slc;
    // Start is called before the first frame update
    void Start()
    {
        slc = SceneLightMangager.GetComponent<SceneLightController>();
        slc.LerpAToB(1.5f); 
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void OnDiraySubtitleFinished()
    {
        Debug.Log("日记本字幕已结束");
        var instance = ResMgr.GetInstance().Load<GameObject>("Prefab/Games/CombinationLock");
        MainCamera.gameObject.transform.position = new Vector3(400, 300, -10);

        var combinationLockGame = instance.GetComponentInChildren<CombinationLock>(true);
        if (combinationLockGame == null)
        {
            Debug.LogWarning("未在实例下找到 CombinationLock 组件");
            return;
        }

        combinationLockGame.SetFinishedCallback(() =>
        {
            Debug.Log("组合锁游戏完成，触发下一步剧情");
            MainCamera.gameObject.transform.position = new Vector3(0, 0, -10);
            Destroy(instance);
            SubtitleMgr.GetInstance().ShowSubtitle(SubtitleType.Bubble, null,
                new List<DialogueEntry>
                {
                    new()
                    {
                        content = "镜子说我们是倒影。",
                        avatar = null
                    },
                    new()
                    {
                        content = "她咳血那夜，我吞下她的名字。",
                        avatar = null
                    },
                    new()
                    {
                        content = "现在我的眼睛成了她的墓穴——闭上时能听见双重心跳。",
                        avatar = null
                    },
                    new()
                    {
                        content = "是谁在门缝藏了黑猫的牙齿……",
                        avatar = null
                    },
                }, transform, Vector2.zero, () =>
                {
                    Debug.Log("剧情结束回调");
                    GameFinished();
                });
            Destroy(instance);
        });
    }

    private void GameFinished()
    {
        // 查找“女儿”对象
        var priestObj = GameObject.Find("Layer(background)");
        if (priestObj != null)
        {
            var sr = priestObj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // TODO: 切换到死亡状态女儿图片
                var newSprite = Resources.Load<Sprite>("Textures/Room/DaughterRoom/daugtersroom2");
                if (newSprite != null)
                {
                    sr.sprite = newSprite;
                }
                else
                {
                    Debug.LogWarning("未能加载死亡状态女儿图片");
                }
            }
            else
            {
                Debug.LogWarning("Layer(background)没有SpriteRenderer组件");
            }
        }
        else
        {
            Debug.LogWarning("未找到Layer(background)对象");
        }
        
        slc.LerpBToC(1.5f); 
    }
    
    
}
