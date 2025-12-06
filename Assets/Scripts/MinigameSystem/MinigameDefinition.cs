using UnityEngine;
using UnityEngine.InputSystem;

namespace TDB.MinigameSystem
{
    [CreateAssetMenu(menuName = "Minigames/Definition")]
    public class MinigameDefinition : ScriptableObject {
        public string DisplayName;
        public GameObject MinigamePrefab; // must implement IMinigame
        public InputActionAsset InputAsset; // optional, will use fallback if not provided
        public string ActionMapName = "Minigame";
    }
}