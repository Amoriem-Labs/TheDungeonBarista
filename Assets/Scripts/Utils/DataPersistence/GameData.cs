using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.CafeSystem.FurnitureSystem;
using TDB.CraftSystem.Data;
using TDB.InventorySystem.IngredientStorage;

namespace TDB.Utils.DataPersistence
{
    [System.Serializable]
    public class GameData
    {
        // public DateTime SaveTime;

        [FoldoutGroup("Recipe Book Data"), HideLabel, InlineProperty]
        public RecipeBookData RecipeBookData;

        [FoldoutGroup("Volatile Ingredient Storage"), HideLabel, InlineProperty]
        public IngredientStorageData VolatileIngredientStorageData;
        [FoldoutGroup("Refrigerated Ingredient Storage"), HideLabel, InlineProperty]
        public IngredientStorageData RefrigeratedIngredientStorageData;
        
        public List<FurnitureData> AllInstalledFurnitureData;

        public GameData(GameData newGameData)
        {
            RecipeBookData = new RecipeBookData(newGameData.RecipeBookData);
            
            VolatileIngredientStorageData = new IngredientStorageData(newGameData.VolatileIngredientStorageData);
            RefrigeratedIngredientStorageData = new IngredientStorageData(newGameData.RefrigeratedIngredientStorageData);
            
            AllInstalledFurnitureData =
                newGameData.AllInstalledFurnitureData.Select(f => new FurnitureData(f)).ToList();
        }
    }
}
