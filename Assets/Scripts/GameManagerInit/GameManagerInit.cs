using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerInit : MonoBehaviour
{
    [SerializeField]
    public Enum_SceneState m_GameState = Enum_SceneState.MainMenu;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("初始化 GameManager");
        GameManager gm =  GameManager.GetInstance();
        gm.SceneState = m_GameState;
        gm.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
