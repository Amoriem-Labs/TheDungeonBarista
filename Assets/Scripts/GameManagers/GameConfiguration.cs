using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Customers;
using TDB.CafeSystem.FurnitureSystem;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.EffectSystem.Data;
using TDB.CraftSystem.UI.RecipeGraph;
using TDB.IngredientStorageSystem.Data;
using TDB.Utils.SceneTransitions;
using UnityEngine;
using UnityEngine.Serialization;

namespace TDB.GameManagers
{
    /// <summary>
    /// A centralized SO for storing all game configurations.
    /// </summary>
    [CreateAssetMenu(fileName = "New Game Config", menuName = "Data/Game Config")]
    public class GameConfiguration : ScriptableObject
    {
        [TabGroup("Scenes")] [SerializeField] public List<SceneAsset> MainMenuScenes;
        [TabGroup("Scenes")] [SerializeField] public List<SceneAsset> CafePhaseScenes;
        [TabGroup("Scenes")] [SerializeField] public List<SceneAsset> DungeonPhaseScenes;
        
        [TabGroup("Testing")] [SerializeField] private bool _skipTutorial;
        [TabGroup("Testing")] [SerializeField] private bool _infiniteResource;

        #region CraftSystem

        [TabGroup("CraftSystem")]
        [TabGroup("_DefaultTabGroup/CraftSystem/SubTabGroup", "Test Ingredient Storage")]
        [SerializeField, InlineProperty, HideLabel] public IngredientStorageData TestIngredientStorage;
        
        [FormerlySerializedAs("FinalRecipe")]
        [TabGroup("_DefaultTabGroup/CraftSystem/SubTabGroup", "Test Final Recipe")]
        [SerializeField, InlineProperty, HideLabel] public FinalRecipeData TestFinalRecipe;
        
        [TabGroup("_DefaultTabGroup/CraftSystem/SubTabGroup", "Test Recipe Book")]
        [SerializeField, InlineProperty, HideLabel] public RecipeBookData TestRecipeBook;

        public RecipeBookData ExtendedTestRecipeBook =>
            new RecipeBookData(TestRecipeBook.AllObtainedRawRecipes,
                new List<FinalRecipeData> { TestFinalRecipe });

        [TabGroup("_DefaultTabGroup/CraftSystem/SubTabGroup", "Animation")]
        [SerializeField]
        public float LevelUpProgressFillTime = .5f;
        
        [TabGroup("_DefaultTabGroup/CraftSystem/SubTabGroup", "Animation")]
        [Title("Added Ingredient Animation")]
        [SerializeField, InlineProperty, HideLabel]
        public AddedIngredientAnimParam AddedIngredientAnimParam;

        [TabGroup("CraftSystem")]
        [SerializeField]
        public EffectDefinition QualityEffect;
        
        #endregion

        #region CafeSystem

        [TabGroup("CafeSystem")]
        [SerializeField] public FurniturePreset DefaultFurniturePreset;

        [TabGroup("CafeSystem")]
        [SerializeField] public List<FlavorDefinition> AllFlavors;

        [TabGroup("CafeSystem")]
        [SerializeField] public float BonusPerFlavorLevel = .2f;
        [TabGroup("CafeSystem")]
        [SerializeField] public float PunishmentPerFlavorLevel = .1f;
        [TabGroup("CafeSystem")]
        [SerializeField] public float BonusPerQualityLevel = .2f;
        
        #endregion
        
#if UNITY_EDITOR
        public bool SkipTutorial => _skipTutorial;
        public bool InfiniteResource => _infiniteResource;
#else
        public bool SkipTutorial => false;
        public bool InfiniteResource => false;
#endif
    }
}