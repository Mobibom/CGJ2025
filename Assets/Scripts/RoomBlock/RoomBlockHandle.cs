using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RoomBlockHandle : MonoBehaviour
{
    
    [Header("切换场景枚举")]
    [SerializeField]
    public Enum_SceneState m_SwitchToScene;

    [Header("窗户")] public GameObject windowLight;

    
    [Header("高亮渐变时间（秒）")]
    public float transitionTime = 0.2f;
    
    [Header("高亮强度")]
    public float hightLightStrength = 0.2f;

    private Material _mat;
    private float _targetValue = 0f;
    private float _currentValue = 0f;

    private float _transitionVelocity;
    
    private bool m_IsMouseEntered = false;
    

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
                    
                    //EventCenter.GetInstance().EventTrigger("场景切换", new SceneStateData(m_SwitchToScene, () => { Debug.Log("进入房间"); }));
                }
                break;
        }
    }

    private void OnDestroy()
    {
        EventCenter.GetInstance().RemoveEventListener<KeyCode>("某键按下", OnKeyDown);
    }
    

    private void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        // _mat = sr.material;
        // if (_mat.HasProperty("_HightLightStrength"))
        // {
        //     _currentValue = _mat.GetFloat("_HightLightStrength");
        // }
        
        windowLight.SetActive(false);
    }

    private void OnMouseEnter()
    {
        m_IsMouseEntered = true;
        // hightLightStrength = 0.15f;
        // _targetValue = hightLightStrength;
        windowLight.SetActive(true);
    }

    // private void OnMouseDown()
    // {
    //     EventCenter.GetInstance().EventTrigger("某键按下", KeyCode.Mouse0);
    // }

    private void OnMouseExit()
    {
        m_IsMouseEntered = false;
        // _targetValue = 0f;
        windowLight.SetActive(false);
    }

    private void Update()
    {
        // if (_mat == null || !_mat.HasProperty("_HightLightStrength"))
        //     return;
        //
        // // 平滑插值（基于时间）
        // _currentValue = Mathf.MoveTowards(_currentValue, _targetValue, Time.deltaTime / transitionTime);
        // _mat.SetFloat("_HightLightStrength", _currentValue);
    }
}