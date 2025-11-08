using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Managers;
using TDB.CraftSystem.EffectSystem.Data;
using TDB.CraftSystem.EffectSystem.LevelUpEffect;
using TDB.IngredientStorageSystem.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TDB.CraftSystem.Data
{
    [System.Serializable]
    public class FinalRecipeData
    {        
        [field: SerializeField] public RawRecipeDefinition RawRecipe { get; private set; }
        [field: SerializeField] public List<IngredientNodeData> NodeData { get; private set; }
        [field: SerializeField] public string RecipeName { get; private set; }

        public Action OnNameChange;
        
        public FinalRecipeData(RawRecipeDefinition rawRecipe)
        {
            RawRecipe = rawRecipe;
            NodeData = rawRecipe.InitialNodeData;

            RecipeName = rawRecipe.RecipeName + $"-{Random.Range(0, 100):00}";
        }

        protected FinalRecipeData(FinalRecipeData recipe)
        {
            RawRecipe = recipe.RawRecipe;
            NodeData = recipe.NodeData;
            RecipeName = recipe.RecipeName;
        }

        public Dictionary<IngredientDefinition, int> GetAddedIngredients() =>
            NodeData.SelectMany(n => n.AddedIngredients)
                .GroupBy(i => i)
                .ToDictionary(g => g.Key, g => g.Count());

        public bool IsRecipeReady => NodeData.Any(n => n.IsNodeReady);
        
        public List<EffectData> GetAllEffectData() =>
            NodeData.SelectMany(n => n.Effects)
                .GroupBy(p => p.Effect)
                .Select(g => 
                    g.Key.GenerateEffectData(g.Select(p => p.Parameter).ToList()))
                .ToList();

        public int GetServingsAvailable(IngredientStorageData ingredientStorage)
        {
            var perServing = GetAddedIngredients();
            var available = ingredientStorage.GetIngredientsCount;
            return perServing.Min(kv => available.GetValueOrDefault(kv.Key, 0) / kv.Value);
        }

        public void SetName(string text)
        {
            RecipeName = text;
            OnNameChange?.Invoke();
        }

        public int GetQualityLevel(EffectDefinition qualityEffect)
        {
            var effects = GetAllEffectData();
            var effect = effects.Find(e => e.Definition == qualityEffect);
            return effect is not LevelUpEffectData quality ? 0 : quality.Level;
        }

        public int GetBasicPrice()
        {
            // TODO: maybe some effects can increase basic price
            return RawRecipe.BasicPrice;
        }
    }
}