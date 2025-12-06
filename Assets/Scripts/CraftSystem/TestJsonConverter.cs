using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TDB.CraftSystem.Data;
using TDB.GameManagers;
using TDB.InventorySystem.IngredientStorage;
using UnityEngine;

namespace TDB.CraftSystem
{
    /// <summary>
    /// Ensure serializations for save data work.
    /// </summary>
    public class TestJsonConverter : MonoBehaviour
    {
        [SerializeField, ReadOnly, HideLabel, BoxGroup("Loaded Storage")]
        private IngredientStorageData _loadedStorage;
        
        [SerializeField, ReadOnly, HideLabel, BoxGroup("Loaded Recipe Book")]
        public RecipeBookData _loadedRecipeBook;
        
        // [Button(ButtonSizes.Large)]
        // public void Test()
        // {
        //     var originStorage = GameManager.Instance.GameConfig.TestIngredientStorage;
        //     var originRecipeBook = GameManager.Instance.GameConfig.ExtendedTestRecipeBook;
        //     
        //     var bytes = SerializationUtility.SerializeValue(originStorage, DataFormat.JSON);
        //     _loadedStorage = SerializationUtility.DeserializeValue<IngredientStorageData>(bytes, DataFormat.JSON);
        //     
        //     bytes = SerializationUtility.SerializeValue(originRecipeBook, DataFormat.JSON);
        //     _loadedRecipeBook = SerializationUtility.DeserializeValue<RecipeBookData>(bytes, DataFormat.JSON);
        // }
    }
}