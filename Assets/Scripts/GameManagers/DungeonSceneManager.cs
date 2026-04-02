using Sirenix.OdinInspector;
using TDB.DungeonSystem.Generate;
using TDB.GameManagers.SessionManagers;
using UnityEngine;

namespace TDB.GameManagers
{
    public class DungeonSceneManager : MonoBehaviour
    {
        private SessionManager _session;

        /// <summary>
        /// Invoked once by the GameManager when the scene loading finishes.
        /// </summary>
        /// <param name="enterData"></param>
        [Button(ButtonSizes.Large), DisableInEditorMode]
        public static void FindAndInitialize(EnterDungeonData enterData)
        {
            var manager = FindObjectOfType<DungeonSceneManager>();
            if (!manager)
            {
                Debug.LogError("CafeSceneManager not found");
                return;
            }
            manager.Initialize(enterData);
        }

        private void Initialize(EnterDungeonData enterData)
        {
            _session = FindObjectOfType<SessionManager>();
            if (!_session)
            {
                Debug.LogError("SessionManager not found");
            }
            
            InitializePlayer();
            InitializeDungeon(enterData);
        }

        private void InitializeDungeon(EnterDungeonData enterData)
        {
            var gen = FindObjectOfType<DungeonGenerator>();
            if (!gen)
            {
                Debug.LogError("DungeonGenerator not found");
                return;
            }
            
            gen.GenerateDungeon(enterData.DungeonDefinition);
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

            HandleExitDungeonLevel();
        }

        public void HandleExitDungeonLevel()
        {
            // transfer all collectibles
            _session.TransferCollectibles();
            
            GameManager.Instance.DungeonToCafe();
        }
    }

    public class EnterDungeonData
    {
        public RoomLibrary DungeonDefinition;
    }
}