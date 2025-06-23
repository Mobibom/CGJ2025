/// <summary>
/// �����е� UI ���
/// </summary>
public class RoomPanel : BasePanel
{
    protected override void OnClick(string btnName)
    {
        base.OnClick(btnName);
        switch (btnName)
        {
            case "BackButton":
                // �����������
                UIManager.GetInstance().HideAllPanel(() => {
                    // �������뷿��ѡ�񳡾����¼���GameManager ���������¼��������س���
                    EventCenter.GetInstance().EventTrigger<SceneStateData>("�����л�", new SceneStateData(Enum_SceneState.RoomSelection));
                });
                break;
        }
    }
}
