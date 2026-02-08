using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TDB.GameManagers.SessionManagers;
using TDB.ShopSystem.Framework;
using TMPro;
using UnityEngine;

namespace TDB.CafeSystem.UI
{
    public class MoneyUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _moneyText;
        
        private MoneyManager _moneyManager;
        private int _currentMoneyDisplay;
        private TweenerCore<int, int, NoOptions> _numberAnim;

        private void Awake()
        {
            _moneyManager = FindObjectOfType<MoneyManager>();
        }

        private void OnEnable()
        {
            _moneyManager.OnResourceUpdate += HandleMoneyUpdate;
            _moneyManager.OnReceiveMoney += HandleReceiveMoney;
        }

        private void OnDisable()
        {
            _moneyManager.OnResourceUpdate -= HandleMoneyUpdate;
            _moneyManager.OnReceiveMoney -= HandleReceiveMoney;
        }

        /// <summary>
        /// Number change animation.
        /// </summary>
        private void HandleMoneyUpdate()
        {
            if (_numberAnim != null && _numberAnim.IsPlaying())
            {
                _numberAnim.Kill();
            }
            
            var targetMoney = _moneyManager.GetResource();
            float time = Mathf.Min(3f, (targetMoney - _currentMoneyDisplay) * 0.01f);
            _numberAnim = DOTween.To(() => _currentMoneyDisplay, x =>
            {
                _currentMoneyDisplay = x;
                _moneyText.text = MoneyManager.MoneyToString(_currentMoneyDisplay);
            }, targetMoney, time);
        }

        /// <summary>
        /// Only receive animation.
        /// </summary>
        private void HandleReceiveMoney(ReceiveMoneyInfo info)
        {
            // TODO: money fly animation
        }
    }
}