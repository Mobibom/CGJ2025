using System;
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
    private Action onFinish;


    // Start is called before the first frame update
    void Start()
    {
        var numbers = GetComponentsInChildren<ConbinationLockNumber>();
        if (numbers == null)
        {
            Debug.LogError("未找到 ConbinationLockNumber 组件");
            return;
        }

        if (numbers.Length < 4)
        {
            Debug.LogError("ConbinationLockNumber 组件数量不足，至少需要 4 个");
            return;
        }

        number1st = numbers[0];
        number2nd = numbers[1];
        number3rd = numbers[2];
        number4th = numbers[3];

        var canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.sortingLayerID = SortingLayer.NameToID("Game");
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
            onFinish?.Invoke();
        }
    }

    public void SetFinishedCallback(Action callback)
    {
        // 设置完成回调
        if (callback != null)
        {
            onFinish = callback;
        }
    }
}
