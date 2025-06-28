using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Addnumber : MonoBehaviour
{
    // Start is called before the first frame update
    ConbinationLockNumber parentNumber;

    void Start()
    {
        parentNumber = gameObject.transform.parent.gameObject.GetComponent<ConbinationLockNumber>();
        EventCenter.GetInstance().AddEventListener<KeyCode>("某键按下", OnKeyDown);
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
                    if (hit.transform.gameObject == this.gameObject)
                    {
                        parentNumber.number = (parentNumber.number + 1 + 10) % 10;
                        parentNumber.text.SetText(parentNumber.number.ToString());
                    }
                }
            break;
        }
    }
}
