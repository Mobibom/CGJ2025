using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 场景状态的枚举
/// </summary>
public enum Enum_SceneState
{
    MainMenu,
    RoomSelection,
    DemoRoom,
    Room1,
    Room2,

    Game1,

    NumberGraph,
    HuaRongPuzzleGame,
    MatchThreeGame,

    Test,
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
/// 游戏管理器，主逻辑的入口
/// </summary>
public class GameManager : BaseManager<GameManager>
{
    public Enum_SceneState SceneState { get; set; }

    private bool isInit = false;

    public GameManager()
    {
        MonoMgr.GetInstance().AddUpdateListener(Update);
        InputMgr.GetInstance().StartOrEndCheck(true);
        EventCenter.GetInstance().AddEventListener<KeyCode>("某键按下", OnKeyDown);
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
        if (isInit)
        {
            return;
        }

        Debug.Log("Init");
        // 下面这段注释掉，影响debug的时候使用，
        // 可以在GameManger.prefab里设置
        //SceneState = Enum_SceneState.MainMenu;
        MonoMgr.GetInstance().AddUpdateListener(Update);

        EventCenter.GetInstance().AddEventListener<SceneStateData>("场景切换", OnSceneStateChanged);
        
        if(SceneState == Enum_SceneState.MainMenu)
            OnMainMenuSceneLoaded();

        isInit = true;
    }

    private void OnMainMenuSceneLoaded()
    {
        Debug.Log("主菜单场景加载完成");
        SceneState = Enum_SceneState.MainMenu;
        // MusicMgr.GetInstance().PlayBkMusic("");
        UIManager.GetInstance().ShowPanel<MainMenuPanel>("MainMenu/MainMenuPanel", E_UI_Layer.Top);
    }

    private void OnRoomSelectionSceneLoaded()
    {
        Debug.Log("房间选择场景加载完成");
        SceneState = Enum_SceneState.RoomSelection;
        UIManager.GetInstance().ShowPanel<RoomSelectionPanel>("Room/RoomSelectionPanel", E_UI_Layer.Top);
    }

    private void OnDemoRoomSceneLoaded()
    {
        Debug.Log("DemoRoom 场景加载完成");
        SceneState = Enum_SceneState.DemoRoom;
        UIManager.GetInstance().ShowPanel<RoomPanel>("Room/RoomPanel", E_UI_Layer.Top);
    }

    private void OnGame1SceneLoaded()
    {
        Debug.Log("Game1 场景加载完成");
        SceneState = Enum_SceneState.Game1;
    }

    private void OnNumberGraphSceneLoaded()
    {
        Debug.Log("NumberGraph 场景加载完成");
        SceneState = Enum_SceneState.NumberGraph;
    }

    private void OnMatchThreeGameSceneLoaded()
    {
        Debug.Log("MatchThreeGame 场景加载完成");
        SceneState = Enum_SceneState.MatchThreeGame;
        UIManager.GetInstance().ShowPanel<MatchThreeGamePanel>("Games/MatchThreeGamePanel", E_UI_Layer.Top);
    }

    private void OnHuaRongPuzzleSceneLoaded()
    {
        Debug.Log("HuaRongPuzzle 场景加载完成");
        SceneState = Enum_SceneState.HuaRongPuzzleGame;
    }

    private void OnTestSceneLoaded()
    {
        Debug.Log("Test 场景加载完成");
        SceneState = Enum_SceneState.Test;
    }

    private void OnSceneStateChanged(SceneStateData data)
    {
        if (data.state == SceneState)
            return;

        // TODO: 添加更多的场景状态处理
        switch (data.state)
        {
            case Enum_SceneState.MainMenu:
                data.callBack += OnMainMenuSceneLoaded;
                ScenesMgr.GetInstance().LoadScene("MainMenuScene", data.callBack);
                break;

            case Enum_SceneState.RoomSelection:
                // 进入房间选择场景
                data.callBack += OnRoomSelectionSceneLoaded;
                ScenesMgr.GetInstance().LoadScene("RoomSelectionScene", data.callBack);
                break;

            case Enum_SceneState.DemoRoom:
                data.callBack += OnDemoRoomSceneLoaded;
                ScenesMgr.GetInstance().LoadScene("DemoRoomScene", data.callBack);
                break;

            case Enum_SceneState.Game1:
                data.callBack += OnGame1SceneLoaded;
                ScenesMgr.GetInstance().LoadScene("FillColorGameScene", data.callBack);
                break;

            case Enum_SceneState.HuaRongPuzzleGame:
                data.callBack += OnHuaRongPuzzleSceneLoaded;
                ScenesMgr.GetInstance().LoadScene("HuaRongPuzzleScene", data.callBack);
                break;

            case Enum_SceneState.NumberGraph:
                data.callBack += OnNumberGraphSceneLoaded;
                ScenesMgr.GetInstance().LoadScene("NumberGraphScene", data.callBack);
                break;

            case Enum_SceneState.MatchThreeGame:
                data.callBack += OnMatchThreeGameSceneLoaded;
                ScenesMgr.GetInstance().LoadScene("MatchThreeGameScene", data.callBack);
                break;

            case Enum_SceneState.Test:
                data.callBack += OnTestSceneLoaded;
                // TODO: 修改为测试场景的名称
                ScenesMgr.GetInstance().LoadScene("CombinationLock", data.callBack);
                break;

            default:
                Debug.LogError("SceneState Error");
                break;
        }
    }
}