using System.Collections.Generic;
using DG.Tweening;
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
        [Header("音效")] [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip typeSfx;
        [Header("打字速度")] [SerializeField] private float typingSpeed = 0.05f;

        [Header("气泡偏移（世界坐标）")] [SerializeField]
        private Vector3 offset = new Vector3(0, 2f, 0);

        [Header("对话序列")] [SerializeField] private List<DialogueEntry> dialogues;
        private int currentIndex = 0;
        private System.Action onFinish;
        private Transform followTarget;

        private Tween typingTween;
        private string fullContent;
        private bool isTyping = false;
        private int lastCharCount = 0;

        /// <summary>
        /// 初始化气泡内容和跟随目标。
        /// </summary>
        public void Init(Sprite bgSprite, List<DialogueEntry> entries, Transform followTarget,
            Vector2 offset,
            System.Action onFinish = null)
        {
            this.dialogues = entries;
            this.followTarget = followTarget;
            this.onFinish = onFinish;
            this.backgroundSprite = bgSprite;
            (this.offset.x, this.offset.y) = (offset.x, offset.y);

            currentIndex = 0;
            ShowCurrentLine();
            if (bubbleButton != null)
                bubbleButton.onClick.AddListener(OnBubbleClick);
        }

        private void ShowCurrentLine()
        {
            if (bubbleText != null && dialogues != null && currentIndex < dialogues.Count)
            {
                if (backgroundSprite != null && backgroundImage != null)
                {
                    backgroundImage.sprite = backgroundSprite;
                }

                fullContent = dialogues[currentIndex].content;
                if (typingTween != null && typingTween.IsActive()) typingTween.Kill();
                bubbleText.text = "";
                isTyping = true;
                lastCharCount = 0;
                // 按钮抖动逻辑
                if (bubbleButton != null)
                {
                    bubbleButton.transform.DOComplete();
                    float shakeStrength = Mathf.Clamp(fullContent.Length * 0.8f, 20f, 90f);
                    bubbleButton.transform.DOShakePosition(0.3f, new Vector3(shakeStrength, shakeStrength, 0),
                        (int)shakeStrength, 90, true);
                }

                typingTween = DG.Tweening.DOTween.To(() => 0, x =>
                        {
                            bubbleText.text = fullContent.Substring(0, x);
                            if (x <= lastCharCount || x <= 0 || typeSfx == null || audioSource == null) return;
                            audioSource.PlayOneShot(typeSfx);
                            lastCharCount = x;
                        },
                        fullContent.Length,
                        fullContent.Length * typingSpeed)
                    .SetEase(DG.Tweening.Ease.Linear)
                    .OnComplete(() =>
                    {
                        bubbleText.text = fullContent;
                        isTyping = false;
                    });
            }
        }

        private void OnBubbleClick()
        {
            if (isTyping)
            {
                if (typingTween != null && typingTween.IsActive()) typingTween.Kill();
                bubbleText.text = fullContent;
                isTyping = false;
                return;
            }

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
                bubbleButton.transform.localPosition = offset;
            }
        }
    }
}