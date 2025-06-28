using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigsawFragment : MonoBehaviour
{
    public Vector3 targetPosition;
    public float matchThreshold = 0.1f;
    private bool isChosen = false;
    private bool alreadyMatched = false;
    // Start is called before the first frame update
    void Start()
    {
        EventCenter.GetInstance().AddEventListener<KeyCode>("某键按下", OnKeyDown);
        EventCenter.GetInstance().AddEventListener<KeyCode>("某键抬起", OnKeyUp);
        EventCenter.GetInstance().AddEventListener<Vector3>("鼠标移动", OnMouseMove);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnKeyDown(KeyCode key)
    {
        if (alreadyMatched)
        {
            return;
        }
        switch (key)
        {
            case KeyCode.Mouse0:
                isChosen = true;
                // TODO: 处理选中事件, e.g. 显示选中特效
                break;
        }
    }

    void OnKeyUp(KeyCode key)
    {
        if (alreadyMatched)
        {
            return;
        }
        CheckMatch();
        isChosen = false;
    }

    void OnMouseMove(Vector3 pos)
    {
        if (alreadyMatched)
        {
            return;
        }
        if (isChosen)
        {
            transform.position = pos;
        }
    }

    void CheckMatch()
    {
        if ((transform.position - targetPosition).magnitude < matchThreshold)
        {
            alreadyMatched = true;
            transform.position = targetPosition;
        }
    }
}
