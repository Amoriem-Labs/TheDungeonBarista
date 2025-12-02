using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.Audio;
using TDB.CafeSystem.Managers;
using TDB.Utils.CrossSceneCameraBinding;
using TDB.Utils.Misc;
using TDB.Utils.ObjectPools;
using TDB.Utils.SceneTransitions;
using TDB.Utils.Singletons;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace TDB.GameManagers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField, HideLabel, BoxGroup("Game Config"), InlineEditor]
        private GameConfiguration _gameConfig;

        private SceneTransition _transitionController;

        public GameConfiguration GameConfig => _gameConfig;

        public override void Initialize()
        {
            // GameManager determines the initialization order of all passive singletons.
            // All passive singletons should be a CHILD of the GameManager.
            InitializeManager<AudioManager>();
            InitializeManager<CameraBindingManager>();

            // pooled object initialization is usually dependent on other systems
            InitializeManager<ObjectPoolManager>();
            
            _transitionController = FindObjectOfType<SceneTransition>();
        }

        private void InitializeManager<T>() where T : PassiveSingleton<T>
        {
            var manager = GetComponentInChildren<T>();
            if (manager)
            {
                manager.Initialize();
                Debug.Log(typeof(T).Name + " initialized.");
            }
            else
            {
                var go = new GameObject(typeof(T).Name, typeof(T));
                go.transform.SetParent(transform);
                manager = go.AddComponent<T>();
                manager.Initialize();
                Debug.LogWarning(typeof(T).Name + " is missing. It is created and initialized.");
            }
        }

        private void Start()
        {
#if !UNITY_EDITOR
            StartGame();
#endif
        }

        #region Scene Transitions

        [Button(ButtonSizes.Large)]
        private void StartGame()
        {
            StartCoroutine(SceneTransitionCoroutine(GameConfig.MainMenuScenes));
        }

        [Button(ButtonSizes.Large)]
        public void StartSession()
        {
            var coroutine = SceneTransitionCoroutine(
                GameConfig.CafePhaseScenes,
                scenesToUnload: GameConfig.MainMenuScenes,
                sceneLoadedCallback: StartSessionOnLoaded(),
                transitionOutroCallback: StartSessionOutroFinish()
            );
            StartCoroutine(coroutine);
        }

        private IEnumerator StartSessionOnLoaded()
        {
            CafeSceneManager.FindAndInitialize();
            yield break;
        }
        
        private IEnumerator StartSessionOutroFinish()
        {
            CafePhaseController.FindAndStart();
            yield break;
        }

        [Button(ButtonSizes.Large)]
        public void CafeToDungeon()
        {
            StartCoroutine(SceneTransitionCoroutine(GameConfig.DungeonPhaseScenes,
                scenesToUnload: GameConfig.CafePhaseScenes));
        }

        [Button(ButtonSizes.Large)]
        public void DungeonToCafe()
        {
            var coroutine = SceneTransitionCoroutine(
                GameConfig.CafePhaseScenes,
                scenesToUnload: GameConfig.DungeonPhaseScenes,
                sceneLoadedCallback: DungeonToCafeOnLoaded(),
                transitionOutroCallback: DungeonToCafeOutroFinish()
            );
            StartCoroutine(coroutine);
        }

        private IEnumerator DungeonToCafeOnLoaded()
        {
            CafeSceneManager.FindAndInitialize();
            yield break;
        }
        
        private IEnumerator DungeonToCafeOutroFinish()
        {
            CafePhaseController.FindAndStart();
            yield break;
        }

        [Button(ButtonSizes.Large)]
        public void GoToMainMenu()
        {
            StartCoroutine(SceneTransitionCoroutine(GameConfig.MainMenuScenes,
                scenesToUnload: GameConfig.DungeonPhaseScenes.Union(GameConfig.CafePhaseScenes).ToList()));
        }

        #endregion
        
        private IEnumerator SceneTransitionCoroutine(
            List<SceneAsset> scenesToLoad,
            List<SceneAsset> scenesToUnload = null,
            IEnumerator transitionIntroCallback = null,
            IEnumerator transitionOutroCallback = null,
            IEnumerator sceneUnloadedCallback = null,
            IEnumerator sceneLoadedCallback = null,
            bool reloadOverlappedScenes = false)
        {
            scenesToUnload = scenesToUnload ?? new List<SceneAsset>();
            
            // get actual scenes to unload
            scenesToUnload = scenesToUnload.Where(toUnload =>
                    // scene to unload should be loaded
                    SceneManager.GetSceneByName(toUnload.SceneName).isLoaded &&
                    // either all overlapped scenes are force reloaded
                    // or the scene to unload is not the one to be loaded 
                    (reloadOverlappedScenes || scenesToLoad.All(toLoad => toLoad.SceneName != toUnload.SceneName)))
                .ToList();
            // get actual scenes to load
            scenesToLoad = scenesToLoad.Where(toLoad =>
                    // scene to load should not be loaded
                    !SceneManager.GetSceneByName(toLoad.SceneName).isLoaded &&
                    // either all overlapped scenes are force reloaded
                    // or the scene to load is not the one to be unloaded 
                    (reloadOverlappedScenes || scenesToUnload.All(toUnload => toUnload.SceneName != toLoad.SceneName)))
                .ToList();


            IEnumerator SceneLoadingCoroutine()
            {
                // unload scenes in order
                foreach (var toUnload in scenesToUnload)
                {
                    if (!SceneManager.GetSceneByName(toUnload.SceneName).isLoaded) continue;
                
                    var unload = SceneManager.UnloadSceneAsync(toUnload.SceneName);
                    while (unload is { isDone: false }) yield return null;
                }
                // unloading finished
                yield return sceneUnloadedCallback;
                // load scenes in order
                foreach (var toLoad in scenesToLoad)
                {
                    if (SceneManager.GetSceneByName(toLoad.SceneName).isLoaded) continue;
                
                    var load = SceneManager.LoadSceneAsync(toLoad.SceneName, LoadSceneMode.Additive);
                    while (load is { isDone: false }) yield return null;
                }
                // loading finished
                yield return sceneLoadedCallback;
            }
            var sceneLoadingCoroutine = SceneLoadingCoroutine();

            yield return StartTransition(sceneLoadingCoroutine, transitionIntroCallback, transitionOutroCallback);
        }

        public IEnumerator StartTransition(IEnumerator workDuringTransition,
            IEnumerator transitionIntroCallback = null,
            IEnumerator transitionOutroCallback = null)
        {
            // transition intro
            yield return _transitionController.StartTransitionIntro();
            yield return transitionIntroCallback;
            // scene loading
            yield return workDuringTransition;
            // transition intro
            yield return _transitionController.StartTransitionOutro();
            yield return transitionOutroCallback;
        }
    }
}
