using System.Collections.Generic;
using ProjectBase.Subtitle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Subtitle
{
    /// <summary>
    /// 交互式气泡对话框，支持多句对话、点击切换、跟随目标。
    /// </summary>
    public class SubtitleBubble : MonoBehaviour
    {
        [Header("UI 组件")] [SerializeField] private TextMeshProUGUI bubbleText; // 气泡文本内容
        [SerializeField] private Image backgroundImage; // 气泡文本内容
        [SerializeField] private Button bubbleButton; // 气泡点击按钮
        [Header("资源")] [SerializeField] private Sprite backgroundSprite;

        [Header("气泡偏移（世界坐标）")] [SerializeField]
        private Vector3 offset = new Vector3(0, 2f, 0);

        [Header("对话序列")] [SerializeField] private List<DialogueEntry> dialogues;
        private int currentIndex = 0;
        private System.Action onFinish;
        private Transform followTarget;

        /// <summary>
        /// 初始化气泡内容和跟随目标。
        /// </summary>
        public void Init(Sprite bgSprite, List<DialogueEntry> entries, Transform followTarget = null,
            System.Action onFinish = null)
        {
            this.dialogues = entries;
            this.followTarget = followTarget;
            this.onFinish = onFinish;
            this.backgroundSprite = bgSprite;
            currentIndex = 0;
            ShowCurrentLine();
            if (bubbleButton != null)
                bubbleButton.onClick.AddListener(OnBubbleClick);
        }

        private void ShowCurrentLine()
        {
            if (bubbleText != null && dialogues != null && currentIndex < dialogues.Count)
            {
                bubbleText.text = dialogues[currentIndex].content; // 假设DialogueEntry有Text字段

                if (backgroundSprite == null || backgroundImage == null) return;

                backgroundImage.sprite = backgroundSprite; // 设置背景图片
            }
        }

        private void OnBubbleClick()
        {
            currentIndex++;
            if (dialogues != null && currentIndex < dialogues.Count)
            {
                ShowCurrentLine();
            }
            else
            {
                onFinish?.Invoke();
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (followTarget != null)
            {
                Vector3 worldPos = followTarget.position + offset;
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
                transform.position = screenPos;
            }
        }
    }
}