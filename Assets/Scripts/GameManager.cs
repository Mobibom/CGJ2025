using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ����״̬��ö��
/// </summary>
public enum Enum_SceneState
{
    MainMenu,
    RoomSelection,
    DemoRoom,
    Room1,
    Room2,
}

public class SceneStateData
{
    public Enum_SceneState state;

    public UnityAction callBack;

    public SceneStateData(Enum_SceneState state, UnityAction callBack = null)
    {
        this.state = state;
        this.callBack = callBack;
    }
}

/// <summary>
/// ��Ϸ�����������߼������
/// </summary>
public class GameManager : BaseManager<GameManager>
{
    public Enum_SceneState SceneState { get; set; }

    public GameManager()
    {
        MonoMgr.GetInstance().AddUpdateListener(Update);
        InputMgr.GetInstance().StartOrEndCheck(true);
        EventCenter.GetInstance().AddEventListener<KeyCode>("ĳ������", OnKeyDown);
    }

    private void OnKeyDown(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.Escape:
                break;

            case KeyCode.B:
                break;
        }
    }

    private void Update()
    {
        switch (SceneState)
        {
            case Enum_SceneState.MainMenu:
                MainMenuSceneUpdate();
                break;
        }
    }

    private void MainMenuSceneUpdate()
    {
    }

    public void Init()
    {
        Debug.Log("Init");
        SceneState = Enum_SceneState.MainMenu;
        MonoMgr.GetInstance().AddUpdateListener(Update);

        EventCenter.GetInstance().AddEventListener<SceneStateData>("�����л�", OnSceneStateChanged);

        OnMainMenuSceneLoaded();
    }

    private void OnMainMenuSceneLoaded()
    {
        Debug.Log("���˵������������");
        SceneState = Enum_SceneState.MainMenu;
        // MusicMgr.GetInstance().PlayBkMusic("");
        UIManager.GetInstance().ShowPanel<MainMenuPanel>("MainMenu/MainMenuPanel", E_UI_Layer.Top);
    }

    private void OnRoomSelectionSceneLoaded()
    {
        Debug.Log("����ѡ�񳡾��������");
        SceneState = Enum_SceneState.RoomSelection;
        UIManager.GetInstance().ShowPanel<RoomSelectionPanel>("Room/RoomSelectionPanel", E_UI_Layer.Top);
    }

    private void OnDemoRoomSceneLoaded()
    {
        Debug.Log("DemoRoom �����������");
        SceneState = Enum_SceneState.DemoRoom;
        UIManager.GetInstance().ShowPanel<RoomPanel>("Room/RoomPanel", E_UI_Layer.Top);
    }

    private void OnSceneStateChanged(SceneStateData data)
    {
        if (data.state == SceneState)
            return;

        // TODO: ��Ӹ���ĳ���״̬����
        switch (data.state)
        {
            case Enum_SceneState.MainMenu:
                data.callBack += OnMainMenuSceneLoaded;
                ScenesMgr.GetInstance().LoadScene("MainMenuScene", data.callBack);
                break;

            case Enum_SceneState.RoomSelection:
                // ���뷿��ѡ�񳡾�
                data.callBack += OnRoomSelectionSceneLoaded;
                ScenesMgr.GetInstance().LoadScene("RoomSelectionScene", data.callBack);
                break;

            case Enum_SceneState.DemoRoom:
                data.callBack += OnDemoRoomSceneLoaded;
                ScenesMgr.GetInstance().LoadScene("DemoRoomScene", data.callBack);
                break;

            default:
                Debug.LogError("SceneState Error");
                break;
        }
    }
}