using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.UI.Inventory;
using TDB.CraftSystem.UI.RecipeBook;
using TDB.GameManagers;
using TDB.IngredientStorageSystem.Data;
using TDB.Utils.EventChannels;
using TDB.Utils.UI;
using UnityEngine;

namespace TDB.CraftSystem.UI
{
    [RequireComponent(typeof(UIEnabler))]
    public class CraftMenuUI : MonoBehaviour
    {
        [SerializeField] private Transform _addedIngredientParent;
        
        [TitleGroup("Events")]
        [SerializeField] private EventChannel _onRecipeSelectedEvent;

        private IIngredientStorageReceiver[] _ingredientStorageReceivers;

        private RawRecipeMenuUI _rawRecipeMenu;
        private UIEnabler _rawRecipeMenuEnabler;
        private UIEnabler _craftMenuEnabler;

        private void Awake()
        {
            _ingredientStorageReceivers = GetComponentsInChildren<IIngredientStorageReceiver>();

            _rawRecipeMenu = GetComponentInChildren<RawRecipeMenuUI>();
            _rawRecipeMenuEnabler = _rawRecipeMenu.GetComponent<UIEnabler>();
            _craftMenuEnabler = GetComponent<UIEnabler>();
        }

        /// <summary>
        /// Opening the craft menu from production devices with some existing recipe will open that recipe.
        /// </summary>
        [Button(ButtonSizes.Large, ButtonStyle.FoldoutButton), DisableInEditorMode]
        public void OpenMenu(OpenMenuInfo openMenuInfo)
        {
            // open menu
            _craftMenuEnabler.Enable(.5f);
            // open raw recipe menu if no current recipe selected
            ToggleRecipeBook(openMenuInfo.CurrentRecipe == null);

            // pass IS data
            foreach (var receiver in _ingredientStorageReceivers)
            {
                receiver.ReceiveIngredientStorage(openMenuInfo.IngredientStorage);
            }
            // bind recipe book data
            _rawRecipeMenu.BindRecipeBook(openMenuInfo.RecipeBook);
            // select/deselect recipe
            _onRecipeSelectedEvent.RaiseEvent(openMenuInfo.CurrentRecipe);
        }

        [Button(ButtonSizes.Large), DisableInEditorMode]
        public void OpenMenuWithTestData()
        {
            OpenMenu(new OpenMenuInfo(){
                IngredientStorage = GameManager.Instance.GameConfig.TestIngredientStorage,
                CurrentRecipe = GameManager.Instance.GameConfig.TestFinalRecipe,
                RecipeBook = new RecipeBookData(GameManager.Instance.GameConfig.TestRecipeBook.AllObtainedRawRecipes,
                    new List<FinalRecipeData> { GameManager.Instance.GameConfig.TestFinalRecipe })
            });
        }

        public void ToggleRecipeBook() => ToggleRecipeBook(!_rawRecipeMenuEnabler.Enabled);

        public void ToggleRecipeBook(bool enable)
        {
            if (enable)
            {
                _rawRecipeMenuEnabler.Enable();
                // move to bottom (visually) when menu is open
                _addedIngredientParent.SetAsFirstSibling();
            }
            else
            {
                _rawRecipeMenuEnabler.Disable();
                // move to top (visually) when menu is closed
                _addedIngredientParent.SetAsLastSibling();
            }
        }

        public void ConfirmFinalRecipe()
        {
            throw new NotImplementedException();
        }
    }

    [System.Serializable]
    public struct OpenMenuInfo
    {
        // current recipe in the production device from which the craft menu is opened
        public FinalRecipeData CurrentRecipe;
        // merged storage of all ingredients from all refrigerators and one-day storage
        public IngredientStorageData IngredientStorage;
        // all recipes obtained & created by the player
        public RecipeBookData RecipeBook;
    }

    public interface IIngredientStorageReceiver
    {
        public void ReceiveIngredientStorage(IngredientStorageData ingredientStorage);
    }
}