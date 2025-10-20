using System;
using Sirenix.OdinInspector;
using TDB.Utils.EventChannels;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace TDB.Utils.UI.ConfirmPanel
{
    [RequireComponent(typeof(UIEnabler))]
    public class ConfirmPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private ConfirmPanelButton _leftButton;
        [SerializeField] private ConfirmPanelButton _rightButton;
        
        [Title("Events")]
        [SerializeField] private EventChannel _displayConfirmationEvent;

        private UIEnabler _enabler;

        private void Awake()
        {
            _enabler = GetComponent<UIEnabler>();
        }

        private void OnEnable()
        {
            _displayConfirmationEvent.AddListener<ConfirmationData>(HandleConfirmation);
        }

        private void OnDisable()
        {
            _displayConfirmationEvent.RemoveListener<ConfirmationData>(HandleConfirmation);
        }

        private void HandleConfirmation(ConfirmationData data)
        {
            _enabler.Enable(.2f);
            
            _messageText.text = data.Message;
            _leftButton.ConfigureButton(data.LeftButtonInfo, this);
            _rightButton.ConfigureButton(data.RightButtonInfo, this);
        }

        public void Hide()
        {
            _enabler.Disable(.2f);
        }
    }

    [System.Serializable]
    public class ConfirmationData
    {
        public string Message;
        public ConfirmationButtonInfo LeftButtonInfo = ConfirmationButtonInfo.WarningButton();
        public ConfirmationButtonInfo RightButtonInfo = ConfirmationButtonInfo.RegularButton();
    }
    
    [System.Serializable]
    public struct ConfirmationButtonInfo
    {
        public Color ButtonColor;
        public Color ButtonTextColor;
        public string ButtonText;
        public UnityAction ClickAction;

        public static ConfirmationButtonInfo RegularButton(string text = "Confirm", UnityAction clickAction = null) => new()
        {
            ButtonColor = Color.white,
            ButtonTextColor = Color.black,
            ButtonText = text,
            ClickAction = clickAction
        };

        public static ConfirmationButtonInfo WarningButton(string text = "Cancel", UnityAction clickAction = null) => new()
        {
            ButtonColor = Color.red,
            ButtonTextColor = Color.white,
            ButtonText = text,
            ClickAction = clickAction
        };
    }
}