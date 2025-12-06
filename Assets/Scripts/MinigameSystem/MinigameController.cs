// MinigameController.cs

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TDB.MinigameSystem
{
    public struct MinigameResult {
        public bool Success;
        public float Score;
        public float Duration;
    }

    public interface IMinigame {
        void Initialize(float diff, InputActionMap input, Action<MinigameResult> onComplete);
        
        void Begin();
        
        void Abort();
    }

    public static class MinigameController
    {

        // Optionally override canvas per-call if you want a different parent.
        public static void Play(
            MinigameDefinition def,
            Canvas parentCanvas,
            Action<MinigameResult> onComplete,
            float difficulty = .5f,
            float slowdownScale = 1f)
        {
            if (onComplete is null)
            {
                Debug.LogError("[MinigameController] No onComplete callback provided.");
                return;
            }
    
            if (def == null) 
            {
                Debug.LogError("[MinigameController] Definition is null.");
                onComplete.Invoke(default);
                return;
            }

            float oldScale = Time.timeScale;
            Time.timeScale = slowdownScale;

            var go = UnityEngine.Object.Instantiate(def.MinigamePrefab, parentCanvas.transform);
            var mini = go.GetComponent<IMinigame>();
            if (mini == null) 
            {
                Debug.LogError($"[MinigameController] Prefab '{def.MinigamePrefab?.name}' does not implement IMinigame.");
                UnityEngine.Object.Destroy(go);
                Time.timeScale = oldScale;
                onComplete.Invoke(new MinigameResult { Success = false, Score = 0f });
                return;
            }

            var map = ResolveActionMap(def);
            map.Enable();

            // Callback wrapper that cleans up and forwards to user callback
            void OnMinigameComplete(MinigameResult result)
            {
                map.Disable();
                UnityEngine.Object.Destroy(go);
                Time.timeScale = oldScale;
        
                onComplete.Invoke(result);
            }
    
            mini.Initialize(difficulty, map, OnMinigameComplete);
            mini.Begin();
        }
        
        private static InputActionMap ResolveActionMap(MinigameDefinition def)
        {
            if (def.InputAsset != null && !string.IsNullOrEmpty(def.ActionMapName)) {
                var found = def.InputAsset.FindActionMap(def.ActionMapName, throwIfNotFound: false);
                if (found != null) return found;
            }
            
            Debug.LogError("[MinigameController] ActionMap not found.");
            
            return null;
        }
    }
}
