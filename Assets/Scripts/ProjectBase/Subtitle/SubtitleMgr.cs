using System;
using System.Collections.Generic;
using UI.Subtitle;
using UnityEngine;
using UnityEngine.UIElements;
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
        private GameObject currentSubtitlePrefab; // 当前使用的字幕预制体


        private GameObject GetPrefab(SubtitleType type)
        {
            switch (type)
            {
                case SubtitleType.Bubble:
                    return Resources.Load<GameObject>("Prefab/Subtitle/SubtitleBubble");
                case SubtitleType.HalfScreen:
                    return Resources.Load<GameObject>("Prefab/Subtitle/SubtitleHalfScreen");
                case SubtitleType.FullScreen:
                    // return Resources.Load<GameObject>("Prefab/Subtitle/SubtitleFullScreen");
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        // 由物品调用，传入对话内容和跟随目标
        public void ShowSubtitle(SubtitleType type, Sprite bgSprite,
            List<DialogueEntry> entries,
            Transform followTarget,
            Vector2 offset,
            Action onFinish = null)
        {
            if (currentSubtitlePrefab)
            {
                return; // 如果当前已经有字幕在显示，则不再创建新的
            }

            var go = Object.Instantiate(GetPrefab(type), followTarget);
            currentSubtitlePrefab = go;
            switch (type)
            {
                case SubtitleType.Bubble:
                {
                    var sub = go.GetComponent<SubtitleBubble>();
                    sub.Init(bgSprite, entries, followTarget, offset, () =>
                    {
                        Object.Destroy(currentSubtitlePrefab);
                        currentSubtitlePrefab = null;
                        onFinish?.Invoke();
                    });
                    break;
                }
                case SubtitleType.HalfScreen:
                {
                    var sub = go.GetComponent<SubtitleHalfScreen>();
                    sub.Init(bgSprite, entries, followTarget, offset, () =>
                    {
                        Object.Destroy(currentSubtitlePrefab);
                        currentSubtitlePrefab = null;
                        onFinish?.Invoke();
                    });
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