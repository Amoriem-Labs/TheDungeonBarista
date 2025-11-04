using UnityEngine;

namespace TDB.CraftSystem.Data
{
    [System.Serializable]
    public class ProductData
    {
        public FinalRecipeData RecipeData;
        public float MinigamePriceMultiplier;

        public int Price => Mathf.CeilToInt(RecipeData.GetBasicPrice() * MinigamePriceMultiplier);

        public ProductData(FinalRecipeData recipeData, MinigameOutcome minigameOutcome)
        {
            RecipeData = recipeData;
            MinigamePriceMultiplier = minigameOutcome.PriceMultiplier;
        }
    }

    public struct MinigameOutcome
    {
        public float PriceMultiplier;
    }
}