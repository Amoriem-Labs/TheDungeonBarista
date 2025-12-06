using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.CafeSystem.FurnitureSystem;
using TDB.CraftSystem.Data;
using TDB.InventorySystem.FurnitureInventory;
using TDB.InventorySystem.IngredientStorage;
using TDB.ShopSystem.FurnitureShop;
using TDB.ShopSystem.IngredientShop;
using UnityEngine;

namespace TDB.GameManagers
{
    [System.Serializable]
    public class GameData
    {
        // public DateTime SaveTime;

        #region Inventory Data

        [SerializeField]
        [FoldoutGroup("Volatile Ingredient Storage"), HideLabel, InlineProperty]
        public IngredientStorageData VolatileIngredientStorageData;
        
        [SerializeField]
        [FoldoutGroup("Refrigerated Ingredient Storage"), HideLabel, InlineProperty]
        public IngredientStorageData RefrigeratedIngredientStorageData;

        [SerializeField]
        [FoldoutGroup("Furniture Inventory"), HideLabel, InlineProperty]
        public FurnitureInventoryData FurnitureInventoryData;
        
        #endregion

        
        #region Cafe Data
        
        [SerializeField]
        [FoldoutGroup("Recipe Book Data"), HideLabel, InlineProperty]
        public RecipeBookData RecipeBookData;
        
        [SerializeField]
        public List<FurnitureData> AllInstalledFurnitureData;
        
        #endregion


        #region Shop Data

        [SerializeField]
        [FoldoutGroup("Ingredient Shop Data"), HideLabel, InlineProperty]
        public IngredientShopData IngredientShopData;
        
        [SerializeField]
        [FoldoutGroup("Furniture Shop Data"), HideLabel, InlineProperty]
        public FurnitureShopData FurnitureShopData;

        #endregion
        

        public GameData(GameData newGameData)
        {
            // copy recipe book data
            RecipeBookData = new RecipeBookData(newGameData.RecipeBookData);
            
            // copy ingredient storage data
            VolatileIngredientStorageData = new IngredientStorageData(newGameData.VolatileIngredientStorageData);
            RefrigeratedIngredientStorageData = new IngredientStorageData(newGameData.RefrigeratedIngredientStorageData);
            // copy inventory data
            FurnitureInventoryData = new FurnitureInventoryData(newGameData.FurnitureInventoryData);
            
            // copy installed furniture data
            AllInstalledFurnitureData =
                newGameData.AllInstalledFurnitureData.Select(f => new FurnitureData(f)).ToList();
            
            // copy shop data
            IngredientShopData = new IngredientShopData(newGameData.IngredientShopData);
            FurnitureShopData = new FurnitureShopData(newGameData.FurnitureShopData);
        }
    }
}
