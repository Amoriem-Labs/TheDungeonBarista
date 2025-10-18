using Sirenix.Serialization;
using TDB.Utils.Misc;
using UnityEngine;

[assembly: RegisterFormatter(typeof(RsoPathFormatter<TDB.CraftSystem.Data.IngredientTypeDefinition>))]

namespace TDB.CraftSystem.Data
{
    [CreateAssetMenu(fileName = "New Ingredient Type", menuName = "Data/Craft System/Ingredient Type Definition", order = 0)]
    public class IngredientTypeDefinition : ResourceScriptableObject
    {
        [SerializeField] private Sprite _icon;
        
        public Sprite Icon => _icon;
    }
}