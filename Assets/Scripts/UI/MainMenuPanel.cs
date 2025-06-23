using UnityEngine;

/// <summary>
/// ���˵������
/// </summary>
public class MainMenuPanel : BasePanel
{
    protected override void OnClick(string btnName)
    {
        base.OnClick(btnName);
        switch (btnName)
        {
            case "StartButton":
                // �����������
                UIManager.GetInstance().HideAllPanel(()=> {
                    // �������뷿��ѡ�񳡾����¼���GameManager ���������¼��������س���
                    EventCenter.GetInstance().EventTrigger<SceneStateData>("�����л�", new SceneStateData(Enum_SceneState.RoomSelection));
                });
                break;

            case "AboutButton":
                // TODO: ��ʾ����Ĺ�����Ϣ
                break;

            case "SettingsButton":
                // TODO: ��ʾ�������
                break;

            case "ExitGameButton":
                Application.Quit();
                break;
        }
    }
}
