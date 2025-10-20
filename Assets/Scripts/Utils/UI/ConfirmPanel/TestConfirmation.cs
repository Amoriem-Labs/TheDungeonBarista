using TDB.Utils.EventChannels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDB.Utils.UI.ConfirmPanel
{
    public class TestConfirmation : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private EventChannel _displayConfirmationEvent;


        public void OnPointerClick(PointerEventData eventData)
        {
            _displayConfirmationEvent.RaiseEvent(new ConfirmationData()
            {
                Message = "Test display message",
                LeftButtonInfo = ConfirmationButtonInfo.WarningButton(clickAction: () =>
                {
                    Debug.Log("Canceled");
                }),
                RightButtonInfo = ConfirmationButtonInfo.RegularButton(clickAction: () =>
                {
                    Debug.Log("Confirmed");
                })
            });
        }
    }
}