using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
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


        [TabGroup("CraftSystem")]
        [TabGroup("_DefaultTabGroup/CraftSystem/SubTabGroup", "Test Ingredient Storage")]
        [SerializeField, InlineProperty, HideLabel] public IngredientStorageData TestIngredientStorage;
        
        [FormerlySerializedAs("FinalRecipe")]
        [TabGroup("_DefaultTabGroup/CraftSystem/SubTabGroup", "Test Final Recipe")]
        [SerializeField, InlineProperty, HideLabel] public FinalRecipeData TestFinalRecipe;

        [TabGroup("_DefaultTabGroup/CraftSystem/SubTabGroup", "Animation")]
        [TitleGroup("Added Ingredient Animation")]
        [SerializeField, InlineProperty, HideLabel]
        public AddedIngredientAnimParam AddedIngredientAnimParam; 
        
        
#if UNITY_EDITOR
        public bool SkipTutorial => _skipTutorial;
        public bool InfiniteResource => _infiniteResource;
#else
        public bool SkipTutorial => false;
        public bool InfiniteResource => false;
#endif
    }
}