using TDB.MinigameSystem;
using UnityEngine;

namespace TDB.CraftSystem.Data
{
    [System.Serializable]
    public class ProductData : FinalRecipeData
    {
        public float MinigamePriceMultiplier;

        public FinalRecipeData RecipeData => this;
        public int Price => Mathf.CeilToInt(RecipeData.GetBasicPrice() * MinigamePriceMultiplier);

        public ProductData(FinalRecipeData recipeData, MinigameResult minigameResult) : base(recipeData)
        {
            // TODO: Create some proper path for the multiplier to be calculated from score. I assume upgrades and other things can matter here.
            MinigamePriceMultiplier = minigameResult.Score + 1f;
        }
    }
}