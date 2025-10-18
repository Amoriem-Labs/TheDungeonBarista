using TDB.CraftSystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.CraftSystem.UI.Inventory
{
    public class CraftMenuInventoryItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _inStockNumber;
        [SerializeField] private TextMeshProUGUI _requireNumber;
        [SerializeField] private Image _iconImage;
        
        public void SetInStockNumber(int number)
        {
            _inStockNumber.text = number.ToString();
        }

        public void SetRequiredNumber(int number)
        {
            _requireNumber.text = number.ToString();
        }

        public void BindIngredient(IngredientDefinition ingredient)
        {
            _iconImage.sprite = ingredient.IngredientSprite;
        }
    }
}