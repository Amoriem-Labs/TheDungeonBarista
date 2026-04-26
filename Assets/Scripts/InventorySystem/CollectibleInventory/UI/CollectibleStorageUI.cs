using System;
using TDB.DungeonSystem.Collectibles;
using TDB.GameManagers.SessionManagers;
using TDB.InventorySystem.CollectibleInventory.UI;
using TDB.Utils.EventChannels;
using TDB.Utils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.InventorySystem.IngredientStorage.UI
{
    [RequireComponent(typeof(UIEnabler))]
    public class CollectibleStorageUI : MonoBehaviour, IInventoryInfoDisplayer<CollectibleDefinition>
    {
        [SerializeField] private EventChannel _enableInputEvent;
        [SerializeField] private EventChannel _disableInputEvent;
        [SerializeField] private Button _closeButton;
        
        private CollectibleInfoUI _infoDisplayer;
        private Action _onExitMenu;
        private UIEnabler _enabler;
        private SessionManager _session;
        private CollectibleStackItemUIContainer _container;

        private void Awake()
        {
            _session = FindObjectOfType<SessionManager>();
            
            _enabler = GetComponent<UIEnabler>();

            _infoDisplayer = GetComponentInChildren<CollectibleInfoUI>();
            _container = GetComponentInChildren<CollectibleStackItemUIContainer>();
            
            _closeButton.onClick.AddListener(Hide);
        }

        public void Display(Action onExitMenu)
        {
            _onExitMenu = onExitMenu;
            _enabler.Enable();

            _container.BindAndDisplay(_session.CollectibleInventoryData);
            
            _disableInputEvent.RaiseEvent();
        }
        
        private void Hide()
        {
            if (_onExitMenu != null)
            {
                _onExitMenu?.Invoke();
                _onExitMenu = null;
            }
            _enabler.Disable();
            
            _enableInputEvent.RaiseEvent();
        }
        
        public void DisplayIngredientInfo(InventoryInfoDisplayData<CollectibleDefinition> info) =>
            _infoDisplayer.DisplayCollectibleInfo(info);

        public void StopDisplaying() => _infoDisplayer.StopDisplaying();
    }
}