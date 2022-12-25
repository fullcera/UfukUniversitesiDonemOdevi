using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace EKTemplate
{
    public class SettingsPanel : MonoBehaviour
    {
        public RectTransform settingsIcon;
        public RectTransform settingsButton;
        public RectTransform soundButton;
        public RectTransform vibrationButton;

        public Image soundImg;
        public Image vibrationImg;

        public SettingsSprite soundSprites;
        public SettingsSprite vibrationSprites;

        private Vector2 soundStartPos;
        private Vector2 vibrationStartPos;

        private bool isOpen;

        private Tween activeTween;
        private Tween activeScaleTween;
        private Tween activePositionTween;

        private void Start()
        {
            soundStartPos = soundButton.anchoredPosition;
            vibrationStartPos = vibrationButton.anchoredPosition;

            soundButton.localScale = vibrationButton.localScale = Vector3.zero;

            soundButton.anchoredPosition = settingsButton.anchoredPosition;
            vibrationButton.anchoredPosition = settingsButton.anchoredPosition;

            soundImg.sprite = DataManager.instance.sound ? soundSprites.on : soundSprites.off;
            vibrationImg.sprite = DataManager.instance.vibration ? vibrationSprites.on : vibrationSprites.off;


            Color c = soundImg.color; c.a = 0f;
            soundImg.color = vibrationImg.color = c;
        }

        public void OnPressSettingsButton()
        {
            if (!isOpen) AppearSettings();
            else DisappearSettings();
            isOpen = !isOpen;
        }

        public void OnPressSoundButton()
        {
            DataManager.instance.SetSound(!DataManager.instance.sound);
            soundImg.sprite = DataManager.instance.sound ? soundSprites.on : soundSprites.off;
            SoundManager.instance.AllSound(DataManager.instance.sound);
        }

        public void OnPressVibrationButton()
        {
            DataManager.instance.SetVibration(!DataManager.instance.vibration);
            vibrationImg.sprite = DataManager.instance.vibration ? vibrationSprites.on : vibrationSprites.off;
        }

        public void AppearSettings(float duration = 0.3f)
        {
            settingsIcon.DORotate(new Vector3(0, 0, -360), duration, RotateMode.FastBeyond360);

            if (activeTween != null) activeTween.Kill(true);
            activeTween =
            DOTween
            .Sequence()
            .AppendCallback(() =>
            {
                soundButton.DOAnchorPos(soundStartPos, duration * 0.5f);
                soundButton.DOScale(1f, duration * 0.5f).SetEase(Ease.OutBack);
                soundImg.DOFade(1f, duration);
            })
            .AppendCallback(() =>
            {
                vibrationButton.DOAnchorPos(vibrationStartPos, duration * 0.5f);
                vibrationButton.DOScale(1f, duration * 0.5f).SetEase(Ease.OutBack);
                vibrationImg.DOFade(1f, duration);
            });
        }

        public void DisappearSettings(float duration = 0.3f)
        {
            settingsIcon.DORotate(new Vector3(0, 0, 360), duration, RotateMode.FastBeyond360);

            if (activeTween != null) activeTween.Kill(true);
            activeTween =
            DOTween
            .Sequence()
            .AppendCallback(() =>
            {
                soundButton.DOAnchorPos(new Vector2(0, -soundButton.rect.size.y * 0.5f), duration * 0.5f);
                soundButton.DOScale(0f, duration * 0.5f).SetEase(Ease.InBack);
                soundImg.DOFade(0f, duration);
            })
            .AppendCallback(() =>
            {
                vibrationButton.DOAnchorPos(new Vector2(0, -vibrationButton.rect.size.y * 0.5f), duration * 0.5f);
                vibrationButton.DOScale(0f, duration * 0.5f).SetEase(Ease.InBack);
                vibrationImg.DOFade(0f, duration);
            });

        }
    }

    [System.Serializable]
    public struct SettingsSprite
    {
        public Sprite on;
        public Sprite off;
    }
}