using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectBase.Subtitle;

public class ChamberlaiRoom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void OnBoxSubtitleFinished()
    {
        Debug.Log("box字幕已结束");
        var instance = ResMgr.GetInstance().Load<GameObject>("Prefab/Games/CombinationLock");

        var combinationLockGame = instance.GetComponentInChildren<CombinationLock>(true);
        if (combinationLockGame == null)
        {
            Debug.LogWarning("未在实例下找到 CombinationLock 组件");
            return;
        }

        combinationLockGame.SetFinishedCallback(() =>
        {
            Debug.Log("组合锁游戏完成，触发下一步剧情");
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
        var priestObj = GameObject.Find("Layer(daughter)");
        if (priestObj != null)
        {
            var sr = priestObj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // TODO: 切换到死亡状态女儿图片
                var newSprite = Resources.Load<Sprite>("Textures/Room/PriestRoom/4-3(死亡状态牧师)");
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
                Debug.LogWarning("Layer(Daughter)没有SpriteRenderer组件");
            }
        }
        else
        {
            Debug.LogWarning("未找到Layer(Daughter)对象");
        }
    }
}
