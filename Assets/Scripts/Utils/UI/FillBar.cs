using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.Utils.UI
{
    public class FillBar : MonoBehaviour
    {
        [SerializeField] private float _animTime = 0.2f;
        [SerializeField] private Image _mask;
        [SerializeField] private TextMeshProUGUI _currentAmountText;
        [SerializeField] private TextMeshProUGUI _maxAmountText;
        [SerializeField] private int _padCurrent = 3;
        [SerializeField] private int _padMax = 0;

        private float _targetFillAmount = 1;
        private float _velocity;

        private void Update()
        {
            if (!Mathf.Approximately(_mask.fillAmount, _targetFillAmount))
            {
                _mask.fillAmount = Mathf.SmoothDamp(_mask.fillAmount, _targetFillAmount, ref _velocity, _animTime);
            }
        }

        public void UpdateUI(float currentAmount, float maxAmount, float baseAmount = 0)
        {
            _currentAmountText.text = currentAmount.ToString("0").PadLeft(_padCurrent, '0');
            _maxAmountText.text = maxAmount.ToString("0").PadLeft(_padMax, '0');

            _targetFillAmount = (currentAmount - baseAmount) / (maxAmount - baseAmount);
        }
    }
}