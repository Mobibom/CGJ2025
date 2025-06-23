using UnityEngine;

public class SelectableDoor : MonoBehaviour
{
    [Header("切换场景枚举")]
    [SerializeField]
    private Enum_SceneState m_SwitchToScene;

    private bool m_IsMouseEntered = false;

    public void OnMouseEnter()
    {
        m_IsMouseEntered = true;
    }

    public void OnMouseExit()
    {
        m_IsMouseEntered = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        EventCenter.GetInstance().AddEventListener<KeyCode>("某键按下", OnKeyDown);
    }

    private void OnKeyDown(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.Mouse0: // 鼠标左键
                Debug.Log("Left mouse button pressed.");
                if (m_IsMouseEntered)
                {
                    // 打开门，进入房间
                    // 测试代码
                    UIManager.GetInstance().HideAllPanel(() => {
                        EventCenter.GetInstance().EventTrigger("场景切换", new SceneStateData(m_SwitchToScene, () => { Debug.Log("进入房间"); }));
                    });
                }
                break;
        }
    }

    private void OnDestroy()
    {
        EventCenter.GetInstance().RemoveEventListener<KeyCode>("某键按下", OnKeyDown);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
