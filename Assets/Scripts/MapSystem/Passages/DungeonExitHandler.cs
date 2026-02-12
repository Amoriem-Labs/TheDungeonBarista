using System;
using System.Collections;
using TDB.CafeSystem.Managers;
using TDB.GameManagers;
using UnityEngine;

namespace TDB.MapSystem.Passages
{
    public class DungeonExitHandler : MonoBehaviour, IPassageHandler
    {
        private DungeonSceneManager _dungeonManager;

        private void Awake()
        {
            _dungeonManager = FindObjectOfType<DungeonSceneManager>();
        }

        public IEnumerator HandleEnterPassage(Action abort)
        {
            _dungeonManager.HandleExitDungeonLevel();
            // this should be the terminal handler
            // block all following handlers
            yield return new WaitWhile(() => true);
        }

        public IEnumerator UndoEffect()
        {
            yield break;
        }
    }
}