using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour
{
    // Start is called before the first frame update

    private JigsawFragment chosenFrag;

    public float leftBorder;
    public float rightBorder;
    public float topBorder;
    public float bottomBorder;
    public Action onFinish;
    void Start()
    {
        EventCenter.GetInstance().AddEventListener<KeyCode>("某键按下", OnKeyDown);
        EventCenter.GetInstance().AddEventListener<KeyCode>("某键抬起", OnKeyUp);
        EventCenter.GetInstance().AddEventListener<Vector3>("鼠标移动", OnMouseMove);

        foreach (Transform child in transform)
        {
            child.transform.position = new Vector3(UnityEngine.Random.Range(-3, 3), UnityEngine.Random.Range(-3, 3), child.transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetFinishedCallback(Action callback)
    {
        // 设置完成回调
        if (callback != null)
        {
            onFinish = callback;
        }
    }

    void OnKeyDown(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Mouse0:
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (!hit.transform.gameObject.GetComponent<JigsawFragment>().getMatchStatus())
                    {
                        chosenFrag = hit.transform.gameObject.GetComponent<JigsawFragment>();
                    }
                }
                break;
        }
    }

    void OnKeyUp(KeyCode key)
    {
        if (chosenFrag != null)
        {
            chosenFrag.CheckMatch();
            chosenFrag = null;
            isFinished();
        }
    }

    void OnMouseMove(Vector3 pos)
    {
        if (chosenFrag != null)
        {
            Vector3 movePos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y,
                    Camera.main.nearClipPlane + 0.1f));
            if (movePos.x > rightBorder)
            {
                movePos.x = rightBorder;
            }
            if (movePos.x < leftBorder)
            {
                movePos.x = leftBorder;
            }
            if (movePos.y > bottomBorder)
            {
                movePos.y = bottomBorder;
            }
            if (movePos.y < topBorder)
            {
                movePos.y = topBorder;
            }
            chosenFrag.transform.position = movePos;
        }
    }

    public void isFinished()
    {
        foreach (Transform child in transform)
        {
            if (!child.GetComponent<JigsawFragment>().getMatchStatus())
            {
                return;
            }
        }
        onFinish?.Invoke();
    }
}
