using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Games;
using ProjectBase.Subtitle;

public class PriestRoom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
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
                        // 这里可以添加剧情结束后的逻辑
                    });
            }
            else
            {
                Debug.Log("填色游戏未完成，重新开始");
            }

            Destroy(instance);
        });
    }
}