using System;
using System.Collections;
using TDB.CafeSystem.Managers;
using UnityEngine;

namespace TDB.MapSystem.Passages
{
    public class DungeonEntranceHandler : MonoBehaviour, IPassageHandler
    {
        private CafePhaseController _cafeController;

        private void Awake()
        {
            _cafeController = FindObjectOfType<CafePhaseController>();

            if (!_cafeController)
            {
                Debug.LogError(
                    $"CafePhaseController not found. Please ensure the CafeScene is loaded before loading {gameObject.scene.name}.");
            }
        }

        public IEnumerator HandleEnterPassage(Action abort)
        {
            _cafeController?.EnterDungeon();
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