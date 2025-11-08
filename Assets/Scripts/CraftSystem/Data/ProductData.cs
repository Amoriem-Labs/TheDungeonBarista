using UnityEngine;

namespace TDB.CraftSystem.Data
{
    [System.Serializable]
    public class ProductData : FinalRecipeData
    {
        public float MinigamePriceMultiplier;

        public FinalRecipeData RecipeData => this;
        public int Price => Mathf.CeilToInt(RecipeData.GetBasicPrice() * MinigamePriceMultiplier);

        public ProductData(FinalRecipeData recipeData, MinigameOutcome minigameOutcome) : base(recipeData)
        {
            MinigamePriceMultiplier = minigameOutcome.PriceMultiplier;
        }
    }

    public struct MinigameOutcome
    {
        public float PriceMultiplier;
    }
}