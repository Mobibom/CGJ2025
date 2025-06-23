using UnityEngine;

public class SelectableDoor : MonoBehaviour
{
    [Header("�л�����ö��")]
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
        EventCenter.GetInstance().AddEventListener<KeyCode>("ĳ������", OnKeyDown);
    }

    private void OnKeyDown(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.Mouse0: // ������
                Debug.Log("Left mouse button pressed.");
                if (m_IsMouseEntered)
                {
                    // ���ţ����뷿��
                    // ���Դ���
                    UIManager.GetInstance().HideAllPanel(() => {
                        EventCenter.GetInstance().EventTrigger("�����л�", new SceneStateData(m_SwitchToScene, () => { Debug.Log("���뷿��"); }));
                    });
                }
                break;
        }
    }

    private void OnDestroy()
    {
        EventCenter.GetInstance().RemoveEventListener<KeyCode>("ĳ������", OnKeyDown);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
