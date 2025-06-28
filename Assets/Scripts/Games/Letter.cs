using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class letter : MonoBehaviour
{
    // Start is called before the first frame update

    private JigsawFragment chosenFrag;
    void Start()
    {
        EventCenter.GetInstance().AddEventListener<KeyCode>("某键按下", OnKeyDown);
        EventCenter.GetInstance().AddEventListener<KeyCode>("某键抬起", OnKeyUp);
        EventCenter.GetInstance().AddEventListener<Vector3>("鼠标移动", OnMouseMove);

        foreach (Transform child in transform)
        {
            child.transform.position = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), child.transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {

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
        }
    }

    void OnMouseMove(Vector3 pos)
    {
        if (chosenFrag != null)
        {
            Vector3 movePos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y,
                    Camera.main.nearClipPlane + 0.1f));
            if (movePos.x > 3)
            {
                movePos.x = 3;
            }
            if (movePos.x < -3)
            {
                movePos.x = -3;
            }
            if (movePos.y > 3)
            {
                movePos.y = 3;
            }
            if (movePos.y < -3)
            {
                movePos.y = -3;
            }
            chosenFrag.transform.position = movePos;
        }
    }
}
