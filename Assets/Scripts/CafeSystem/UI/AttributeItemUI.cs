using System;
using DG.Tweening;
using TDB.Utils.Misc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.CafeSystem.UI
{
    public class AttributeItemUI : MonoBehaviour
    {
        [SerializeField] private Image _highlightBG;
        [SerializeField] private TextMeshProUGUI _contentText;
        [SerializeField] private TextMeshProUGUI _rightText;

        private void OnEnable()
        {
            _highlightBG.color = _highlightBG.color.SetAlpha(0);
            _rightText.color = Color.clear;
        }

        public void SetText(string text)
        {
            _contentText.text = text;
        }

        public void TurnOnHighlight(float animTime)
        {
            _highlightBG.DOFade(1, animTime)
                .From(0);
            _highlightBG.transform.DOScale(1, animTime)
                .From(0)
                .SetEase(Ease.OutBack);
        }

        public void DisplayRightText(string text, Color color, float animTime)
        {
            _rightText.color = color;
            _rightText.text = text;
            _rightText.transform.DOScale(1, animTime)
                .From(0).SetEase(Ease.OutBack);
            _rightText.DOFade(1, animTime).From(0);
        }
    }
}