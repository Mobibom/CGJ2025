using DG.Tweening;
using UnityEngine;

public class FatherRoom : MonoBehaviour
{
    
    public GameObject SceneLightMangager; // 确保在 Inspector 中赋值
    public Camera MainCamera; // 确保在 Inspector 中赋值

    private SceneLightController slc;
    private GameObject pic;
    
    public float duration = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        slc = SceneLightMangager.GetComponent<SceneLightController>();
        EventCenter.GetInstance().AddEventListener("华容道通关", OnPuzzlePassed);
        EventCenter.GetInstance().AddEventListener("闪现画面消失", AfterShowPic);
        slc.LerpAToB(duration);

        pic = transform.Find("闪现画面")?.gameObject;
        if (pic != null) pic.SetActive(false);
    }

    private void OnPuzzlePassed()
    {
        pic.SetActive(true); // 启用子物体

        GameObject bihua = transform.Find("壁画")?.gameObject;
        if (bihua != null) bihua.SetActive(false);

        // 显示 0.5 秒后再禁用
        DOVirtual.DelayedCall(0.5f, () =>
        {
            pic.SetActive(false); // 再次禁用
        });
        
        EventCenter.GetInstance().EventTrigger("闪现画面消失");
    }

    private void AfterShowPic()
    {
        GameObject bg = transform.Find("图层1(房间)")?.gameObject;
        if (bg != null) bg.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("FatherRoom/父亲房间死");
    }
    
    public void OnPaintingConversationFinished()
    {
        Debug.Log("已点击画作");
        Vector2 position = new Vector2(-3, -3);
        EventCenter.GetInstance().EventTrigger("初始化华容道", position);
    }
}
