using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.IngredientStorageSystem.Data;
using UnityEngine;

namespace TDB.CraftSystem.Data
{
    [System.Serializable]
    public class FinalRecipeData
    {        
        [field: SerializeField] public RawRecipeDefinition RawRecipe { get; private set; }
        [field: SerializeField] public List<IngredientNodeData> NodeData { get; private set; }
        [field: SerializeField] public string RecipeName { get; private set; }

        public FinalRecipeData(RawRecipeDefinition rawRecipe)
        {
            RawRecipe = rawRecipe;
            NodeData = rawRecipe.InitialNodeData;

            RecipeName = rawRecipe.RecipeName + $"-{Random.Range(0, 100):00}";
        }

        public Dictionary<IngredientDefinition, int> GetAddedIngredients() =>
            NodeData.SelectMany(n => n.AddedIngredient)
                .GroupBy(i => i)
                .ToDictionary(g => g.Key, g => g.Count());

        public bool IsRecipeReady => NodeData.Any(n => n.IsNodeReady);

        public int GetServingsAvailable(IngredientStorageData ingredientStorage)
        {
            var perServing = GetAddedIngredients();
            var available = ingredientStorage.GetIngredientsCount;
            return perServing.Min(kv => available.GetValueOrDefault(kv.Key, 0) / kv.Value);
        }
    }
}