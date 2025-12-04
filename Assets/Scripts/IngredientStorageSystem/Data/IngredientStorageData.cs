using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using UnityEngine;

namespace TDB.IngredientStorageSystem.Data
{
    /// <summary>
    /// Separate the storage data for one-day storage and refrigerator.
    /// Only merge them before sending to the craft system.
    /// </summary>
    [System.Serializable]
    public class IngredientStorageData
    {
        [field: TableList]
        [field: SerializeField]
        public List<IngredientStorageStackData> Ingredients { get; private set; } = new();

        public Dictionary<IngredientDefinition, int> GetIngredientsCount =>
            Ingredients
                .GroupBy(i => i.Definition)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Amount));
        
        public void TryConsume(Dictionary<IngredientDefinition, int> requirement)
        {
            foreach (var stack in Ingredients)
            {
                // not enough resource in the current stack
                if (stack.Amount <= 0) continue;
                
                var ingredient = stack.Definition;
                // current stack is not required or already satisfied
                if (!requirement.TryGetValue(ingredient, out var required) || required <= 0) continue;
                
                // consume from the stack and subtract from requirement
                var supply = Mathf.Min(stack.Amount, required);
                stack.Consume(supply);
                requirement[ingredient] = required - supply;
            }
        }

        public void AddIngredient(IngredientDefinition itemDefinition, int amount = 1)
        {
            var stack = Ingredients.Find(i => i.Definition == itemDefinition);
            if (stack == null)
            {
                stack = new IngredientStorageStackData(itemDefinition);
                Ingredients.Add(stack);
            }
            stack.Deposit(amount);
        }
    }
}