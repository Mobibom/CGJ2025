using System;
using System.Collections.Generic;
using UI.Subtitle;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ProjectBase.Subtitle
{
    [System.Serializable]
    public class DialogueEntry
    {
        public string name; // 说话者名字
        public Sprite avatar; // 可选头像
        [TextArea] public string content; // 对话内容
    }

    public enum SubtitleType
    {
        Bubble, // 气泡对话
        HalfScreen, // 半屏对话
        FullScreen, // 全屏对话
    }

    public class SubtitleMgr : BaseManager<SubtitleMgr>
    {
        public static SubtitleMgr Instance { get; private set; }

        // 由物品调用，传入对话内容和跟随目标
        public void ShowSubtitle(SubtitleType type, GameObject prefab, List<DialogueEntry> entries,
            Transform followTarget)
        {
            var go = Object.Instantiate(prefab, followTarget);

            switch (type)
            {
                case SubtitleType.Bubble:
                {
                    var sub = go.GetComponent<SubtitleBubble>();
                    sub.Init(entries, followTarget, () => { });
                    break;
                }
                case SubtitleType.HalfScreen:
                {
                    var sub = go.GetComponent<UI.Subtitle.Subtitle>();
                    sub.Init(entries, followTarget, () => { });
                    break;
                }
                case SubtitleType.FullScreen:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}