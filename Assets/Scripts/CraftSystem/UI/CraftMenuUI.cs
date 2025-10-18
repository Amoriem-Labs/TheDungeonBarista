using System;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.UI.Inventory;
using TDB.GameManagers;
using TDB.IngredientStorageSystem.Data;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.CraftSystem.UI
{
    public class CraftMenuUI : MonoBehaviour
    {
        [TitleGroup("Events")]
        [SerializeField] private EventChannel _onRecipeSelectedEvent;

        private IIngredientStorageReceiver[] _ingredientStorageReceivers;


        private void Awake()
        {
            _ingredientStorageReceivers = GetComponentsInChildren<IIngredientStorageReceiver>();
        }

        /// <summary>
        /// Opening the craft menu from production devices with some existing recipe will open that recipe.
        /// </summary>
        /// <param name="currentRecipe"></param>
        [Button(ButtonSizes.Large, ButtonStyle.FoldoutButton), DisableInEditorMode]
        public void OpenMenu(IngredientStorageData ingredientStorage, FinalRecipeData currentRecipe = null)
        {
            // TODO: open the menu before doing anything

            // pass IS data
            foreach (var receiver in _ingredientStorageReceivers)
            {
                receiver.ReceiveIngredientStorage(ingredientStorage);
            }
            
            _onRecipeSelectedEvent.RaiseEvent(currentRecipe);
        }

        [Button(ButtonSizes.Large), DisableInEditorMode]
        public void OpenMenuWithTestData()
        {
            OpenMenu(GameManager.Instance.GameConfig.TestIngredientStorage,
                GameManager.Instance.GameConfig.TestFinalRecipe);
        }
    }

    // public struct OpenMenuInfo
    // {
    //     public FinalRecipeData CurrentRecipe;
    //     public IngredientStorageData IngredientStorage;
    // }

    public interface IIngredientStorageReceiver
    {
        public void ReceiveIngredientStorage(IngredientStorageData ingredientStorage);
    }
}