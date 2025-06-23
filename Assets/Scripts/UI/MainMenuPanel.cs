using UnityEngine;

/// <summary>
/// 主菜单面板类
/// </summary>
public class MainMenuPanel : BasePanel
{
    protected override void OnClick(string btnName)
    {
        base.OnClick(btnName);
        switch (btnName)
        {
            case "StartButton":
                // 隐藏所有面板
                UIManager.GetInstance().HideAllPanel(()=> {
                    // 触发进入房间选择场景的事件，GameManager 会监听这个事件，并加载场景
                    EventCenter.GetInstance().EventTrigger<SceneStateData>("场景切换", new SceneStateData(Enum_SceneState.RoomSelection));
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
