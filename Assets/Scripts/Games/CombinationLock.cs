using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class CombinationLock : MonoBehaviour
{
    public int answer1st;
    public int answer2nd;
    public int answer3rd;
    public int answer4th;

    private ConbinationLockNumber number1st;
    private ConbinationLockNumber number2nd;
    private ConbinationLockNumber number3rd;
    private ConbinationLockNumber number4th;

    // Start is called before the first frame update
    void Start()
    {
        number1st = transform.Find("Number_1st").GetComponent<ConbinationLockNumber>();
        number2nd = transform.Find("Number_2nd").GetComponent<ConbinationLockNumber>();
        number3rd = transform.Find("Number_3rd").GetComponent<ConbinationLockNumber>();
        number4th = transform.Find("Number_4th").GetComponent<ConbinationLockNumber>();
    }

    // Update is called once per frame
    void Update()
    {
        if (answer1st == number1st.number &&
           answer2nd == number2nd.number &&
           answer3rd == number3rd.number &&
           answer4th == number4th.number)
        {
            Debug.Log("Congratulations!");
        }
    }
}
