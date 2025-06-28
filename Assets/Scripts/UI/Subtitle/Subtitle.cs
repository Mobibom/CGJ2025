using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DialogueEntry
{
    public Sprite avatar; // 可选头像
    [TextArea]
    public string content; // 对话内容
}

public class Subtitle : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image avatarImage;
    [SerializeField] private TextMeshProUGUI contentText;

    [Header("资源")]
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Sprite defaultAvatar;
    [SerializeField] private List<DialogueEntry> dialogues;

    private int currentIndex = 0;

    void Start()
    {
        ShowDialogue(0);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextDialogue();
        }
    }

    private void ShowDialogue(int index)
    {
        if (dialogues == null || dialogues.Count == 0 || index >= dialogues.Count) return;
        var entry = dialogues[index];
        backgroundImage.sprite = backgroundSprite;
        avatarImage.sprite = entry.avatar != null ? entry.avatar : defaultAvatar;
        contentText.text = entry.content;
    }

    private void NextDialogue()
    {
        currentIndex++;
        if (currentIndex < dialogues.Count)
        {
            ShowDialogue(currentIndex);
        }
        // 可根据需要添加对话结束后的处理
    }
}
