using UnityEngine;
using UnityEngine.UI;

namespace TDB.InventorySystem.IngredientStorage.UI
{
    public class BackpackButton : MonoBehaviour
    {
        [SerializeField] private CollectibleStorageUI _storageUI;
        
        private void Awake()
        {
            var btn = GetComponent<Button>();
            btn.onClick.AddListener(DisplayInventory);
        }

        private void DisplayInventory()
        {
            _storageUI.Display(null);
        }
    }
}