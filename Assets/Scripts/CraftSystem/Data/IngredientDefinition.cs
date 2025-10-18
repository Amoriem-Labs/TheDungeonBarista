using Sirenix.Serialization;
using TDB.Utils.Misc;
using UnityEngine;

[assembly: RegisterFormatter(typeof(RsoPathFormatter<TDB.CraftSystem.Data.IngredientDefinition>))]

namespace TDB.CraftSystem.Data
{
    [CreateAssetMenu(fileName = "New Ingredient", menuName = "Data/Craft System/Ingredient Definition", order = 0)]
    public class IngredientDefinition : ResourceScriptableObject
    {
        [SerializeField] private string _ingredientName;
        [SerializeField] private Sprite _ingredientSprite;
        [SerializeField] private IngredientTypeDefinition _type;

        public string IngredientName => _ingredientName;
        public Sprite IngredientSprite => _ingredientSprite;

        public IngredientTypeDefinition Type => _type;
    }
}