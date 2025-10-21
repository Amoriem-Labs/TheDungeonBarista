using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.Utils.UI.ConfirmPanel
{
    [RequireComponent(typeof(Button))]
    public class ConfirmPanelButton : MonoBehaviour
    {
        [SerializeField] private Image _buttonImage;
        [SerializeField] private TextMeshProUGUI _buttonText;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void ConfigureButton(ConfirmationButtonInfo info, ConfirmPanel confirmPanel)
        {
            _buttonImage.color = info.ButtonColor;
            _buttonText.color = info.ButtonTextColor;
            _buttonText.text = info.ButtonText;
            
            _button.onClick.RemoveAllListeners();
            if (info.ClickAction != null) _button.onClick.AddListener(info.ClickAction);
            _button.onClick.AddListener(confirmPanel.Hide);
        }
    }
}