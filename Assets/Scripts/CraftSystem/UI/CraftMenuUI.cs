using System;
using System.Collections.Generic;
using System.Linq;
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

        [Title("Testing")]
        [SerializeField, ReadOnly] private bool _testRecipeBookInitialized;
        [SerializeField, ReadOnly, HideLabel, InlineProperty, BoxGroup("Chosen Recipe")]
        private FinalRecipeData _testChosenRecipe;
        [SerializeField, ReadOnly, HideLabel, InlineProperty, BoxGroup("Current Recipe Book")]
        private RecipeBookData _testCurrentRecipeBook;
        
        private IIngredientStorageReceiver[] _ingredientStorageReceivers;

        private RawRecipeMenuUI _rawRecipeMenu;
        private UIEnabler _rawRecipeMenuEnabler;
        private UIEnabler _craftMenuEnabler;
        private Action<FinalRecipeData> _callback;

        private void Awake()
        {
            _ingredientStorageReceivers = GetComponentsInChildren<IIngredientStorageReceiver>();

            _rawRecipeMenu = GetComponentInChildren<RawRecipeMenuUI>();
            _rawRecipeMenuEnabler = _rawRecipeMenu.GetComponent<UIEnabler>();
            _craftMenuEnabler = GetComponent<UIEnabler>();
        }

        /// <summary>
        /// Opening the craft menu from production devices with some existing recipe will open that recipe.
        /// See OpenMenuInfo for more details.
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
            // confirm callback
            _callback = openMenuInfo.RecipeDecidedCallback;

            _testChosenRecipe = null;
        }

        [Button(ButtonSizes.Large), DisableInEditorMode]
        public void OpenMenuWithTestData()
        {
            var testStorage = GameManager.Instance.GameConfig.TestIngredientStorage;
            var testSelectedRecipe = GameManager.Instance.GameConfig.TestFinalRecipe;
            
            if (!_testRecipeBookInitialized)
            {
                _testRecipeBookInitialized = true;
                _testCurrentRecipeBook = GameManager.Instance.GameConfig.ExtendedTestRecipeBook;
            }
            else
            {
                // simulate the situation where no recipe was selected on the production device
                testSelectedRecipe = null;
            }
                
            OpenMenu(new OpenMenuInfo(){
                IngredientStorage = testStorage,
                CurrentRecipe = testSelectedRecipe,
                RecipeBook = _testCurrentRecipeBook,
                RecipeDecidedCallback = recipe =>
                {
                    _testChosenRecipe = recipe;
                    if (recipe == null)
                    {
                        Debug.Log("No recipe was selected.");
                    }
                    else
                    {
                        // this demonstrates some usages of the FinalRecipeData
                        Debug.Log(
                            $"Selected recipe: {recipe.RecipeName}\n\t" +
                            $"The recipe uses {recipe.RawRecipe.RecipeName} as the raw recipe.\n\t" +
                            $"The recipe has {recipe.GetAllEffectData().Count} effects.\n\t" +
                            $"The recipe consumes {recipe.GetAddedIngredients().Sum(kv => kv.Value)} ingredients.\n\t" +
                            (recipe.IsRecipeReady
                                ? $"The recipe has {recipe.GetServingsAvailable(testStorage)} servings available."
                                : "The recipe is not ready to be served.")
                        );
                    }
                }
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

        public void ConfirmFinalRecipe(FinalRecipeData recipe)
        {
            _callback?.Invoke(recipe);
            // close menu
            _craftMenuEnabler.Disable(.5f);
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
        // tell the caller of the CraftMenu the recipe decided by the player
        // There are three cases:
        //      - the returned recipe is null: the player chooses no recipe
        //      - the returned recipe is not null, but recipe.IsRecipeReady = false:
        //              the chosen recipe is incomplete and cannot be used
        //      - the returned recipe is not null and recipe.IsRecipeReady = true:
        //              the chosen recipe is ready to use
        public Action<FinalRecipeData> RecipeDecidedCallback;
    }

    public interface IIngredientStorageReceiver
    {
        public void ReceiveIngredientStorage(IngredientStorageData ingredientStorage);
    }
}