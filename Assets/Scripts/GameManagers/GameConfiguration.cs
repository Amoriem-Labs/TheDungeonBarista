using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.Utils.SceneTransitions;
using UnityEngine;

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

#if UNITY_EDITOR
        public bool SkipTutorial => _skipTutorial;
        public bool InfiniteResource => _infiniteResource;
#else
        public bool SkipTutorial => false;
        public bool InfiniteResource => false;
#endif
    }
}