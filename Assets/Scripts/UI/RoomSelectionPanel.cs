/// <summary>
/// ����ѡ�񳡾��е� UI ���
/// </summary>
public class RoomSelectionPanel : BasePanel
{
    protected override void OnClick(string btnName)
    {
        base.OnClick(btnName);
        switch (btnName)
        {
            case "BackButton":
                // �����������
                UIManager.GetInstance().HideAllPanel(() => {
                    // �����������˵��������¼���GameManager ���������¼��������س���
                    EventCenter.GetInstance().EventTrigger<SceneStateData>("�����л�", new SceneStateData(Enum_SceneState.MainMenu));
                });
                break;
        }
    }
}
