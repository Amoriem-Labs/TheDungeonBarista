using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TDB.GameManagers.SessionManagers;
using TMPro;
using UnityEngine;

namespace TDB.CafeSystem.UI
{
    public class EssenceUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _essenceText;
        
        private EssenceManager _essenceManager;
        private int _currentEssenceDisplay;
        private TweenerCore<int, int, NoOptions> _numberAnim;

        private void Awake()
        {
            _essenceManager = FindObjectOfType<EssenceManager>();
        }

        private void OnEnable()
        {
            _essenceManager.OnResourceUpdate += HandleEssenceUpdate;
        }

        private void OnDisable()
        {
            _essenceManager.OnResourceUpdate -= HandleEssenceUpdate;
        }

        /// <summary>
        /// Number change animation.
        /// </summary>
        private void HandleEssenceUpdate()
        {
            if (_numberAnim != null && _numberAnim.IsPlaying())
            {
                _numberAnim.Kill();
            }
            
            var targetEssence = _essenceManager.GetResource();
            float time = Mathf.Min(3f, (targetEssence - _currentEssenceDisplay) * 0.01f);
            _numberAnim = DOTween.To(() => _currentEssenceDisplay, x =>
            {
                _currentEssenceDisplay = x;
                _essenceText.text = EssenceManager.EssenceToString(_currentEssenceDisplay);
            }, targetEssence, time);
        }
    }
}