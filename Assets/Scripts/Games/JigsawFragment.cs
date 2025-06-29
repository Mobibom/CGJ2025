using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigsawFragment : MonoBehaviour
{
    public Vector3 targetPosition;
    public float matchThreshold = 0.3f;
    private bool alreadyMatched = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckMatch()
    {
        Debug.Log(transform.position.x + " " + transform.position.y + " " + targetPosition.x + " " + targetPosition.y);
        Debug.Log((new Vector2(transform.position.x, transform.position.y) -
            new Vector2(targetPosition.x, targetPosition.y)).sqrMagnitude);
        if ((new Vector2(transform.position.x, transform.position.y) -
            new Vector2(targetPosition.x, targetPosition.y)).sqrMagnitude < matchThreshold)
        {
            alreadyMatched = true;
            transform.position = targetPosition;
        }
    }

    public bool getMatchStatus()
    {
        return alreadyMatched;
    }
}
