using System.Collections.Generic;
using DG.Tweening;
using ProjectBase.Subtitle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Subtitle
{
    public class Subtitle : MonoBehaviour
    {
        [Header("UI 元素")] [SerializeField] private Image backgroundImage;
        [SerializeField] private Image avatarImage;
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private TextMeshProUGUI nameText;

        [Header("配置")] [SerializeField] private float typingSpeed = 0.05f; // 打字速度，单位为秒
        [Header("资源")] [SerializeField] private Sprite backgroundSprite;
        [SerializeField] private Sprite defaultAvatar;
        [Header("音效")] [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip typeSfx;

        [Header("对话序列")] [SerializeField] private List<DialogueEntry> dialogues;

        private System.Action onFinish;

        private int currentIndex = 0;

        private Tween typingTween;
        private string fullContent;
        private bool isTyping = false;
        private Sprite lastAvatar = null;
        private int lastCharCount = 0;
        private string lastName = null;

        public void Init(List<DialogueEntry> entries, Transform followTarget = null, System.Action onFinish = null)
        {
            this.dialogues = entries;
            this.onFinish = onFinish;
            currentIndex = 0;
            ShowDialogue(0);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isTyping)
                {
                    // 跳过动画，直接显示完整内容
                    typingTween.Kill();
                    contentText.text = fullContent;
                    isTyping = false;
                }
                else
                {
                    NextDialogue();
                }
            }
        }

        private void ShowDialogue(int index)
        {
            if (dialogues == null || dialogues.Count == 0 || index >= dialogues.Count) return;
            var entry = dialogues[index];
            backgroundImage.sprite = backgroundSprite;
            // 头像逻辑：第一张为空用默认，后续为空延续前一张
            if (index == 0)
            {
                avatarImage.sprite = entry.avatar != null ? entry.avatar : defaultAvatar;
            }
            else
            {
                avatarImage.sprite = entry.avatar != null ? entry.avatar : avatarImage.sprite;
            }

            // 检查头像是否变化，变化则抖动，抖动强度与内容长度相关
            if (lastAvatar != avatarImage.sprite)
            {
                avatarImage.transform.DOComplete(); // 停止之前的动画
                float shakeStrength = Mathf.Clamp(entry.content.Length * 0.8f, 20f, 90f); // 根据内容长度调整抖动强度
                avatarImage.rectTransform.DOShakeAnchorPos(0.3f, new Vector2(shakeStrength, shakeStrength),
                    (int)shakeStrength, 90, true);
            }

            lastAvatar = avatarImage.sprite;
            fullContent = entry.content;
            if (typingTween != null && typingTween.IsActive()) typingTween.Kill();
            contentText.text = "";
            isTyping = true;
            lastCharCount = 0;
            typingTween = DOTween.To(() => 0, x =>
                    {
                        contentText.text = fullContent.Substring(0, x);
                        if (x <= lastCharCount || x <= 0 || typeSfx == null || audioSource == null) return;
                        audioSource.PlayOneShot(typeSfx);
                        lastCharCount = x;
                    },
                    fullContent.Length,
                    fullContent.Length * typingSpeed)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    contentText.text = fullContent;
                    isTyping = false;
                });

            // 设置说话者名字，若为空则继承上一次的
            string showName = !string.IsNullOrEmpty(entry.name) ? entry.name : lastName;
            if (nameText != null)
            {
                nameText.text = showName;
            }

            lastName = showName;
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
}