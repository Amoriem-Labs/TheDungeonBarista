using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.Utils.UI
{
    public class FillBar : MonoBehaviour
    {
        [SerializeField] private float _smoothTime = 0.2f;
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _currentAmountText;
        [SerializeField] private TextMeshProUGUI _maxAmountText;
        [SerializeField] private int _padCurrent = 3;
        [SerializeField] private int _padMax = 0;

        private float _targetFillAmount = 1;
        private float _velocity;

        private void Update()
        {
            if (!Mathf.Approximately(_fillImage.fillAmount, _targetFillAmount))
            {
                _fillImage.fillAmount = Mathf.SmoothDamp(_fillImage.fillAmount, _targetFillAmount, ref _velocity, _smoothTime);
            }
        }

        public void UpdateUI(float currentAmount, float maxAmount, float baseAmount = 0)
        {
            if (_currentAmountText) _currentAmountText.text = currentAmount.ToString("0").PadLeft(_padCurrent, '0');
            if (_maxAmountText) _maxAmountText.text = maxAmount.ToString("0").PadLeft(_padMax, '0');

            _targetFillAmount = (currentAmount - baseAmount) / (maxAmount - baseAmount);
        }
    }
}