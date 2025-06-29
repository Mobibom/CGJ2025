using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectBase.Subtitle;

public class ChamberlaiRoom : MonoBehaviour
{
    public GameObject SceneLightMangager; // 确保在 Inspector 中赋值

    private SceneLightController slc;
    
    
    void Start()
    {
        slc = SceneLightMangager.GetComponent<SceneLightController>();
        slc.LerpAToB(1.5f); 
    }
    
    // 小游戏成功后调用slc.LerpBToc(1.5f);

    // Update is called once per frame
    void Update()
    {

    }
    
    public void OnBoxSubtitleFinished()
    {
        Debug.Log("box字幕已结束");
        var instance = ResMgr.GetInstance().Load<GameObject>("Prefab/Games/NumberGraph");

        var combinationLockGame = instance.GetComponentInChildren<NumberGraph>(true);
        if (combinationLockGame == null)
        {
            Debug.LogWarning("未在实例下找到 NumberGraph 组件");
            return;
        }

        combinationLockGame.SetFinishedCallback(() =>
        {
            Debug.Log("数字图游戏完成，触发下一步剧情");
            GameFinished();
            Destroy(instance);
        });
    }

    private void GameFinished()
    {
        // 查找“背景”对象
        var priestObj = GameObject.Find("Layer(background)");
        if (priestObj != null)
        {
            var sr = priestObj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // TODO: 切换图片
                var newSprite = Resources.Load<Sprite>("Textures/Room/PriestRoom/guanjia2");
                if (newSprite != null)
                {
                    sr.sprite = newSprite;
                }
                else
                {
                    Debug.LogWarning("未能加载图片");
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
