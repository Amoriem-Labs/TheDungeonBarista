using TDB.Utils.EventChannels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDB.CraftSystem.UI.RecipeGraph
{
    public class NodeDeselectionBackground : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private EventChannel _onNodeSelectionEvent;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            _onNodeSelectionEvent.RaiseEvent<IngredientNodeUI>(null);
        }
    }
}