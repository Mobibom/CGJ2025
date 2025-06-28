using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

/// <summary>
/// 主菜单面板类
/// </summary>
public class MainMenuPanel : BasePanel
{
    /// <summary>
    /// 完美像素相机组件
    /// </summary>
    private PixelPerfectCamera pixelPerfectCamera;

    /// <summary>
    /// 相机的初始 PPU
    /// </summary>
    private int cameraInitialPPU;

    private void Start()
    {
        pixelPerfectCamera = Camera.main.gameObject.GetComponent<PixelPerfectCamera>();
        cameraInitialPPU = pixelPerfectCamera.assetsPPU;
    }

    protected override void OnClick(string btnName)
    {
        base.OnClick(btnName);
        switch (btnName)
        {
            case "StartButton":
                //// 隐藏所有面板
                //UIManager.GetInstance().HideAllPanel(()=> {
                //    // 触发进入房间选择场景的事件，GameManager 会监听这个事件，并加载场景
                //    EventCenter.GetInstance().EventTrigger<SceneStateData>("场景切换", new SceneStateData(Enum_SceneState.RoomSelection));
                //});

                // 方案 1: 改相机 PPU，不建议，不是合适的方案，可能会存在像素抖动
                //UIManager.GetInstance().HideAllPanel(() =>
                //{
                //    // DOTween 控制相机逐渐靠近房子，由于使用 Pixel Perfect Camera，所以需要控制 PPU，而不是 orthographicSize
                //    var ppc = Camera.main.gameObject.GetComponent<PixelPerfectCamera>();
                //    DOTween.To(() => ppc.assetsPPU, x => ppc.assetsPPU = x, cameraInitialPPU * 2, 2.0f).SetEase(Ease.Linear).OnComplete(() =>
                //    {
                //        // 触发进入房间选择场景的事件，GameManager 会监听这个事件，并加载场景
                //        EventCenter.GetInstance().EventTrigger<SceneStateData>("场景切换", new SceneStateData(Enum_SceneState.RoomSelection));
                //    });
                //});

                // 方案 2: 改场景所有 sprites 的 transform.scale
                UIManager.GetInstance().HideAllPanel(() =>
                {
                    var allSprites = FindObjectsOfType<SpriteRenderer>();
                    var sequence = DOTween.Sequence();

                    foreach (var sprite in allSprites)
                    {
                        // 为每个 sprite 添加 DOScale 到 sequence 中，并行执行（Join）
                        sequence.Join(sprite.transform.DOScale(2.0f, 3.0f));
                    }

                    // 缩放结束后，屏幕逐渐变黑
                    // 这里显示 PrefacePanel
                    sequence.AppendCallback(() => {
                        foreach (var sprite in allSprites)
                        {
                            // 隐藏所有 sprite
                            sprite.gameObject.SetActive(false);
                        }
                        UIManager.GetInstance().ShowPanel<PrefacePanel>("MainMenu/PrefacePanel", E_UI_Layer.Top);
                    });
                });
                break;

            case "Game1":
                // 隐藏所有面板
                UIManager.GetInstance().HideAllPanel(() =>
                {
                    // 触发进入房间选择场景的事件，GameManager 会监听这个事件，并加载场景
                    EventCenter.GetInstance().EventTrigger<SceneStateData>("场景切换", new SceneStateData(Enum_SceneState.Game1));
                });

                break;

            case "HuaRongPuzzle":
                // 隐藏所有面板
                UIManager.GetInstance().HideAllPanel(() =>
                {
                    // 触发进入房间选择场景的事件，GameManager 会监听这个事件，并加载场景
                    EventCenter.GetInstance().EventTrigger<SceneStateData>("场景切换", new SceneStateData(Enum_SceneState.HuaRongPuzzleGame));
                });
                break;

            case "NumberGraph":
                // 隐藏所有面板
                Debug.Log("NumberGraph");
                UIManager.GetInstance().HideAllPanel(() =>
                {
                    // 触发进入房间选择场景的事件，GameManager 会监听这个事件，并加载场景
                    EventCenter.GetInstance().EventTrigger<SceneStateData>("场景切换", new SceneStateData(Enum_SceneState.NumberGraph));
                });
                break;

            case "AboutButton":
                // TODO: 显示队伍的关于信息
                break;

            case "SettingsButton":
                // TODO: 显示设置面板
                break;

            case "ExitGameButton":
                Application.Quit();
                break;
        }
    }
}
