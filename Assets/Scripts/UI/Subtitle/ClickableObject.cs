using System.Collections.Generic;
using ProjectBase.Subtitle;
using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    [SerializeField] private SubtitleType type;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private List<DialogueEntry> entries;

    private void OnMouseDown()
    {
        Debug.Log("点击了可交互物体: " + gameObject.name);
        SubtitleMgr.GetInstance().ShowSubtitle(this.type, this.prefab, this.backgroundSprite, this.entries,
            this.gameObject.transform);
    }
}