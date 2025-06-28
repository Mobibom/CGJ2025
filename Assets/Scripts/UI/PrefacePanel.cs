using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PrefacePanel : BasePanel
{
    public override void ShowMe()
    {
        base.ShowMe();

        var sequence = DOTween.Sequence();

        var blackImage = GetControl<Image>("BlackImage");

        // 设置黑色背景的初始透明度为 0.0f
        blackImage.color = new Color(blackImage.color.r, blackImage.color.g, blackImage.color.b, 0.0f);

        // 设置黑色背景的透明度从 0.0f 渐变到 1.0f
        sequence.Append(blackImage.DOFade(1.0f, 3.0f));

        // 播放序言字幕
        var prefaceTextGroup = transform.Find("PrefaceTextGroup");
        var prefaceTexts = prefaceTextGroup.GetComponentsInChildren<Text>();
        foreach (var text in prefaceTexts)
        {
            // 设置每个字幕的初始透明度为 0.0f
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
            // 将字幕淡入
            sequence.Append(text.DOFade(1.0f, 1.0f));
            // 等待字幕显示时间
            sequence.AppendInterval(1.0f);
            //// 将字幕淡出
            //sequence.Append(text.DOFade(0.0f, 1.0f));
        }

        sequence.AppendCallback(() =>
        {
            // 隐藏所有精灵
            var allSprites = FindObjectsOfType<SpriteRenderer>();
            foreach (var sprite in allSprites)
            {
                sprite.gameObject.SetActive(false);
            }
        });

        sequence.AppendInterval(1.0f);

        // 字幕播完以后
        sequence.AppendCallback(() =>
        {
            // 触发进入房间选择场景的事件，GameManager 会监听这个事件，并加载场景
            EventCenter.GetInstance().EventTrigger<SceneStateData>("场景切换", new SceneStateData(Enum_SceneState.RoomSelection));

            // 隐藏所有面板
            UIManager.GetInstance().HideAllPanel(null);
        });
    }
}
