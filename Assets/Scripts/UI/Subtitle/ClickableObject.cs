using System;
using System.Collections.Generic;
using System.Linq;
using ProjectBase.Subtitle;
using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    [SerializeField] private SubtitleType type;
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Vector2 offset;
    [SerializeField] private UnityEngine.Events.UnityEvent onFinished;
    [SerializeField] private List<DialogueEntry> dialogues0;
    [SerializeField] private List<DialogueEntry> dialogues1;
    [SerializeField] private List<DialogueEntry> dialogues2;
    [SerializeField] private List<DialogueEntry> dialogues3;
    [SerializeField] private List<DialogueEntry> dialogues4;

    private void Start()
    {
        // 如果没有 EventSystem，则创建一个
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("点击了可交互物体: " + gameObject.name);

        var entries = new List<List<DialogueEntry>>
        {
            dialogues0,
            dialogues1,
            dialogues2,
            dialogues3,
            dialogues4
        }.Where(e => e != null && e.Count > 0).ToList();

        if (entries.Count > 0)
        {
            int idx = UnityEngine.Random.Range(0, entries.Count);
            var selectedEntry = entries[idx];
            SubtitleMgr.GetInstance().ShowSubtitle(this.type, this.backgroundSprite, selectedEntry
                , this.gameObject.transform, this.offset, onFinished.Invoke);
        }
        else
        {
            Debug.LogWarning("entries 为空，无法显示字幕");
        }
    }
}