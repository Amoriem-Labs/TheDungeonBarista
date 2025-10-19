using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDB.CraftSystem.Data
{
    [System.Serializable]
    public class RecipeBookData
    {
        [SerializeField] private List<RawRecipeDefinition> _obtainedRawRecipes;
        [SerializeField] private List<FinalRecipeData> _allRecipeData;

        public RecipeBookData(List<RawRecipeDefinition> obtainedRawRecipes, List<FinalRecipeData> allRecipeData)
        {
            _obtainedRawRecipes = obtainedRawRecipes;
            _allRecipeData = allRecipeData;
        }

        public List<RawRecipeDefinition> AllObtainedRawRecipes =>
            _obtainedRawRecipes.Union(_allRecipeData.Select(r => r.RawRecipe).Distinct()).ToList();

        public List<FinalRecipeData> GetFinalRecipesDerivedFrom(RawRecipeDefinition rawRecipe) =>
            _allRecipeData.Where(r => r.RawRecipe == rawRecipe).ToList();

        public void DeleteRecipe(FinalRecipeData finalRecipe)
        {
            if (!_allRecipeData.Remove(finalRecipe))
            {
                Debug.LogError("Delete recipe failed! Deleted recipe is not in the recipe book.");
            }
        }

        public void AddRecipe(FinalRecipeData newRecipe)
        {
            if (_allRecipeData.Contains(newRecipe))
            {
                Debug.LogError("Add recipe failed! Added recipe is already in the recipe book.");
                return;
            }
            
            _allRecipeData.Add(newRecipe);
        }
    }
}