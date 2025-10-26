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

    [CreateAssetMenu(menuName = "Minigames/Definition")]
    public class MinigameDefinition : ScriptableObject {
        public string DisplayName;
        public GameObject MinigamePrefab; // must implement IMinigame
        public InputActionAsset InputAsset; // optional, will use fallback if not provided
        public string ActionMapName = "Minigame";
    }

    public static class MinigameController
    {

        // Optionally override canvas per-call if you want a different parent.
        public static async Task<MinigameResult> PlayAsync(
            MinigameDefinition def,
            Canvas parentCanvas,
            float difficulty = .5f,
            float slowdownScale = 1f)
        {
            if (def == null) {
                Debug.LogError("[MinigameController] Definition is null.");
                return default;
            }

            float oldScale = Time.timeScale;
            Time.timeScale = slowdownScale;

            var go = UnityEngine.Object.Instantiate(def.MinigamePrefab, parentCanvas.transform);
            var mini = go.GetComponent<IMinigame>();
            if (mini == null) {
                Debug.LogError($"[MinigameController] Prefab '{def.MinigamePrefab?.name}' does not implement IMinigame.");
                UnityEngine.Object.Destroy(go);
                Time.timeScale = oldScale;
                return new MinigameResult { Success = false, Score = 0f };
            }

            var map = ResolveActionMap(def);
            map.Enable();

            // Await completion via TaskCompletionSource
            var tcs = new TaskCompletionSource<MinigameResult>();
            void OnComplete(MinigameResult r) { if (!tcs.Task.IsCompleted) tcs.SetResult(r); }

            
            mini.Initialize(difficulty, map, OnComplete);
            mini.Begin();

            var result = await tcs.Task;

            map.Disable();
            UnityEngine.Object.Destroy(go);
            Time.timeScale = oldScale;

            return result;
        }
        private static InputActionMap ResolveActionMap(MinigameDefinition def)
        {
            if (def.InputAsset != null && !string.IsNullOrEmpty(def.ActionMapName)) {
                var found = def.InputAsset.FindActionMap(def.ActionMapName, throwIfNotFound: false);
                if (found != null) return found;
            }

            // Fallback: construct a minimal Submit action
            var asset = ScriptableObject.CreateInstance<InputActionAsset>();
            var map = new InputActionMap("Minigame");
            var submit = map.AddAction("Submit", InputActionType.Button);
            submit.AddBinding("<Mouse>/leftButton");
            submit.AddBinding("<Keyboard>/space");
            submit.AddBinding("<Gamepad>/buttonSouth");
            asset.AddActionMap(map);
            return map;
        }
    }
}
