using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
        var go = ResMgr.GetInstance().Load<GameObject>("Prefab/Games/FillColorGame");
        if (go != null)
        {
            var instance = Instantiate(go);
            var canvasGroup = instance.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = instance.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 1f); // 使用DOTween实现1秒渐显
        }
        else
        {
            Debug.LogWarning("未能加载 FillColorGame 预制体");
        }
    }
}