using TDB.CraftSystem.Data;
using TDB.CraftSystem.EffectSystem.UI;
using TDB.CraftSystem.UI.Info;
using TDB.DungeonSystem.Collectibles;
using TDB.InventorySystem.IngredientStorage.UI;
using TDB.Utils.UI;
using TMPro;
using UnityEngine;

namespace TDB.InventorySystem.CollectibleInventory.UI
{
    [RequireComponent(typeof(UIEnabler))]
    public class CollectibleInfoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _headerText;
        
        private InventoryInfoDisplayDirection _pivotControl;
        private RectTransform _rectTransform;
        
        private UIEnabler _enabler;

        private void Awake()
        {
            _enabler = GetComponent<UIEnabler>();
            _pivotControl = GetComponentInChildren<InventoryInfoDisplayDirection>();
            _rectTransform = transform as RectTransform;
        }

        private void SetCollectible(CollectibleDefinition collectible)
        {
            _headerText.text = collectible.ItemName;
        }

        public void DisplayCollectibleInfo(InventoryInfoDisplayData<CollectibleDefinition> info)
        {
            _enabler.Enable();
            transform.position = info.RootPosition;
            _rectTransform.sizeDelta = info.RootSize;
            SetCollectible(info.Data);
        }

        public void StopDisplaying()
        {
            _enabler.Disable();
        }
    }
}
