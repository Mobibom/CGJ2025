using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuaRongPuzzleMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        HuaRongPuzzleMgr.GetInstance().InitializeGame();
    }

    // Update is called once per frame
    void Update()
    {
        HuaRongPuzzleMgr.GetInstance().HandleMouseClick();
    }
}
