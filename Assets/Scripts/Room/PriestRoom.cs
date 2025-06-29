using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Games;
using ProjectBase.Subtitle;
// using UnityEngine.Events; // PriestRoom 不需要直接使用 UnityEvent，除非它也有自己的事件

public class PriestRoom : MonoBehaviour
{
    public GameObject SceneLightMangager; // 确保在 Inspector 中赋值

    private SceneLightController slc;
    
    public float duration = 5.0f;
    void Start()
    {
        slc = SceneLightMangager.GetComponent<SceneLightController>();
        slc.LerpAToB(duration); 
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void GameFinished()
    {
        // 查找“图层3.5（牧师）”对象
        var priestObj = GameObject.Find("图层3.5（牧师）");
        if (priestObj != null)
        {
            var sr = priestObj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                var newSprite = Resources.Load<Sprite>("Textures/Room/PriestRoom/4-3(死亡状态牧师)");
                if (newSprite != null)
                {
                    sr.sprite = newSprite;
                }
                else
                {
                    Debug.LogWarning("未能加载死亡状态牧师图片");
                }
            }
            else
            {
                Debug.LogWarning("图层3.5（牧师）没有SpriteRenderer组件");
            }
        }
        else
        {
            Debug.LogWarning("未找到图层3.5（牧师）对象");
        }
        
        
        slc.LerpBToC(duration);
    }

    public void OnCrucifixSubtitleFinished()
    {
        Debug.Log("十字架字幕已结束");
        var instance = ResMgr.GetInstance().Load<GameObject>("Prefab/Games/FillColorGame");
        // 在实例下查找 FillColorGame 组件
        var fillColorGame = instance.GetComponentInChildren<FillColorGame>(true);
        if (fillColorGame == null)
        {
            Debug.LogWarning("未在实例下找到 FillColorGame 组件");
            return;
        }

        fillColorGame.SetFinishedCallback(() =>
        {
            if (fillColorGame.IsGameWin())
            {
                Debug.Log("填色游戏完成，触发下一步剧情");
                SubtitleMgr.GetInstance().ShowSubtitle(SubtitleType.Bubble, null,
                    new List<DialogueEntry>
                    {
                        new()
                        {
                            name = "神父",
                            content = "谢谢您大人，让我得以喘息",
                            avatar = null
                        },
                    }, transform, Vector2.zero, () =>
                    {
                        Debug.Log("剧情结束回调");
                        GameFinished();
                    });
            }
            else
            {
                SubtitleMgr.GetInstance().ShowSubtitle(SubtitleType.Bubble, null,
                    new List<DialogueEntry>
                    {
                        new()
                        {
                            name = "神父",
                            content = "失败",
                            avatar = null
                        },
                    }, transform, Vector2.zero, () =>
                    {
                        Debug.Log("剧情结束回调");
                        // 这里可以添加剧情结束后的逻辑
                    });
                Debug.Log("填色游戏未完成，重新开始");
            }

            Destroy(instance);
        });
    }
}