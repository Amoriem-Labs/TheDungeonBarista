using DG.Tweening;
using TDB.GameManagers.SessionManagers;
using TDB.ShopSystem.Framework;
using TDB.Utils.Misc;
using TDB.Utils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.ShopSystem.UI
{
    [RequireComponent(typeof(UIEnabler))]
    public class PreviewPanel : MonoBehaviour
    {
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private TextMeshProUGUI _soldOutText;
        
        private UIEnabler _enabler;
        private PreviewTrigger _currentTrigger;

        private void Awake()
        {
            _enabler = GetComponent<UIEnabler>();
            _soldOutText.color = _soldOutText.color.SetAlpha(0);
            
            _purchaseButton.onClick.AddListener(HandlePurchase);
        }

        private void HandlePurchase()
        {
            _currentTrigger?.HandlePurchase();
        }

        public void HideCurrentTrigger()
        {
            _currentTrigger?.HideItem();
        }
        
        public void SetCurrentTrigger(PreviewTrigger previewItem)
        {
            _currentTrigger = previewItem;
            _priceText.text = MoneyManager.MoneyToString(_currentTrigger.Price);

            HandlePurchasableUpdate();
            
            _enabler.Enable(.2f);
        }

        public void HandlePurchasableUpdate()
        {
            _purchaseButton.interactable = _currentTrigger.Purchasable;
            
            _soldOutText.DOKill();
            _soldOutText.DOFade(_currentTrigger.IsSoldOut ? 1 : 0, .2f);
        }
    }
}