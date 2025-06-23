/// <summary>
/// 房间中的 UI 面板
/// </summary>
public class RoomPanel : BasePanel
{
    protected override void OnClick(string btnName)
    {
        base.OnClick(btnName);
        switch (btnName)
        {
            case "BackButton":
                // 隐藏所有面板
                UIManager.GetInstance().HideAllPanel(() => {
                    // 触发进入房间选择场景的事件，GameManager 会监听这个事件，并加载场景
                    EventCenter.GetInstance().EventTrigger<SceneStateData>("场景切换", new SceneStateData(Enum_SceneState.RoomSelection));
                });
                break;
        }
    }
}
