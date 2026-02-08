using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TDB.Player.Interaction;
using UnityEngine;

namespace TDB.MapSystem.Passages
{
    public class PassageEntrance : MonoBehaviour, IInteractable
    {
        [SerializeField] private PassageEntranceDefinition _definition;
        [SerializeField] private Transform _spawnPoint;

        [SerializeField] private string _enterTipText = "Enter";
        
        private IPassageCondition[] _conditions;
        private IPassageHandler[] _handlers;

        public PassageEntranceDefinition EntranceDefinition => _definition;
        public Transform SpawnPoint => _spawnPoint;
        public string EnterTipText => _enterTipText;
        
        private void Awake()
        {
            _conditions = GetComponentsInChildren<IPassageCondition>();
            _handlers = GetComponentsInChildren<IPassageHandler>();

            foreach (var condition in _conditions)
            {
                condition.OnConditionChanged += () => OnInteractableUpdated?.Invoke();
            }
        }

        public void EnterPassage()
        {
            IEnumerator EnterPassageCoroutine()
            {
                var aborted = false;
                int i = 0;
                for (; i < _handlers.Length; i++)
                {
                    var effect = _handlers[i];
                    yield return effect.HandleEnterPassage(() => aborted = true);
                    if (aborted)
                    {
                        Debug.Log("EnterPassage aborted");
                        break;
                    }
                }
                
                if (!aborted) yield break;
                // abort executed handlers
                for (; i > 0; i--)
                {
                    var effect = _handlers[i - 1];
                    yield return effect.UndoEffect();
                }
            }
            
            StartCoroutine(EnterPassageCoroutine());
        }

        public bool IsInteractable => _conditions.All(c => c.IsSatisfied);
        public Action OnInteractableUpdated { get; set; }

        public void SetReady()
        {
            // TODO:
        }

        public void SetNotReady()
        {
            // TODO:
        }
    }

    public interface IPassageCondition
    {
        public bool IsSatisfied { get; }
        public Action OnConditionChanged { get; set; }
    }

    public interface IPassageHandler
    {
        /// <summary>
        /// Triggered when entering the passage.
        /// </summary>
        /// <param name="abort">
        ///     Invoke abort to terminate passage entering and undo all previous effects.
        ///     Remember to undo any effect taken by the handler itself.
        /// </param>
        public IEnumerator HandleEnterPassage(Action abort);
        
        /// <summary>
        /// Undo the effect.
        /// Usually not necessary when the effect is the terminal effect. 
        /// </summary>
        public IEnumerator UndoEffect();
    }
}