using System;
using TDB.CraftSystem.EffectSystem.UI;
using TDB.GameManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.CraftSystem.EffectSystem.LevelUpEffect
{
    public class LevelUpRecipeEffectItemUI : TypedCraftMenuRecipeEffectItemUI<LevelUpEffectData>
    {
        [SerializeField] private TextMeshProUGUI _effectNameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private Image _fillProgressImage;

        private float _currentProgress;
        private float _targetTotalProgress;
        private float _smoothTime;
        private float _velocity;

        private void Awake()
        {
            _smoothTime = GameManager.Instance.GameConfig.LevelUpProgressFillTime;
        }

        private void OnEnable()
        {
            _currentProgress = 0;
            _targetTotalProgress = 0;
        }

        protected override void BindEffectData(LevelUpEffectData effectData)
        {
            var definition = effectData.Definition as LevelUpEffectDefinition;
            
            _effectNameText.text = definition!.EffectName;
            _levelText.text = effectData.Level.ToString();
            _targetTotalProgress = effectData.TotalProgress;
        }

        private void Update()
        {
            UpdateProgressFill();
        }

        private void UpdateProgressFill()
        {
            _currentProgress = Mathf.SmoothDamp(_currentProgress, _targetTotalProgress, ref _velocity, _smoothTime);
            var displayedPercent = Mathf.RoundToInt(_currentProgress * 100) % 100;
            _fillProgressImage.fillAmount = displayedPercent / 100f;
            _progressText.text = $"{displayedPercent}%";
        }
    }
}