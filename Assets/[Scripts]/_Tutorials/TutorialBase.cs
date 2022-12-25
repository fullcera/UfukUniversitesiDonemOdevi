using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace EKTemplate
{
    public abstract class TutorialBase : Panel
    {
        protected RectTransform handRect;
        protected RectTransform textRect;
        protected Text instructionText;
        private string playerPrefs;

        protected void Construct(string _playerPrefs, int _count = 1)
        {
            playerPrefs = _playerPrefs;

            if (GetCount() >= _count) DestroyImmediate(this);

            handRect = UIManager.instance.tutorialPanel.hand.rectTransform;
            textRect = UIManager.instance.tutorialPanel.instruction.rectTransform;
            instructionText = UIManager.instance.tutorialPanel.instruction;
            handRect.localScale = Vector3.one;
            handRect.GetComponent<Image>().color = Color.white;
            textRect.localScale = Vector3.one;
            textRect.GetComponent<Text>().color = Color.white;
        }

        private int GetCount()
        {
            return PlayerPrefs.GetInt(playerPrefs, 0);
        }

        protected void Activate()
        {
            UIManager.instance.tutorialPanel.ActiveSmooth(true);
        }

        protected void Deactivate()
        {
            textRect.DOKill(); handRect.DOKill();
            PlayerPrefs.SetInt(playerPrefs, PlayerPrefs.GetInt(playerPrefs, 0) + 1);
            UIManager.instance.tutorialPanel.ActiveSmooth(false, 0.5f, () =>
            {
                Destroy(this);
            });
        }
    }
}