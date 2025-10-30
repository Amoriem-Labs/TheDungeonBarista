using UnityEngine;

namespace TDB.CraftSystem.Data
{
    [System.Serializable]
    public class ProductData
    {
        public FinalRecipeData RecipeData;
        public int QualityLevel;

        public ProductData(FinalRecipeData recipeData, int qualityLevel)
        {
            RecipeData = recipeData;
            QualityLevel = qualityLevel;
        }
    }
}