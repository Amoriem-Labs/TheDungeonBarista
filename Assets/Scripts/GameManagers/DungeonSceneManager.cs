using Sirenix.OdinInspector;
using UnityEngine;

namespace TDB.GameManagers
{
    public class DungeonSceneManager : MonoBehaviour
    {
        /// <summary>
        /// Invoked once by the GameManager when the scene loading finishes.
        /// </summary>
        [Button(ButtonSizes.Large), DisableInEditorMode]
        public static void FindAndInitialize()
        {
            var manager = FindObjectOfType<DungeonSceneManager>();
            if (!manager)
            {
                Debug.LogError("CafeSceneManager not found");
                return;
            }
            manager.Initialize();
        }

        private void Initialize()
        {
            InitializePlayer();
        }

        private void InitializePlayer()
        {
            if (GameObject.FindGameObjectWithTag("Player").TryGetComponent(out EntityData entity))
            {
                entity.OnDeath += HandlePlayerDeath;
            }
            else
            {
                Debug.LogError("Player is not associated with EntityData");
            }
        }

        private void HandlePlayerDeath()
        {
            // TODO: notify player
            Debug.Log("Player is dead");
            
            GameManager.Instance.DungeonToCafe();
        }

        public void HandleExitDungeonLevel()
        {
            // TODO: currently jumps to cafe immediately
            
            GameManager.Instance.DungeonToCafe();
        }
    }
}