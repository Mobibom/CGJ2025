using UnityEngine;

/// <summary>
/// ��Ϸѭ�������ڳ�ʼ����Ϸ������
/// </summary>
public class GameLoop : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        GameManager.GetInstance().Init();
    }
}