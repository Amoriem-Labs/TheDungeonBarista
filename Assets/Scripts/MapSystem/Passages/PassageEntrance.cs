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
                foreach (var effect in _handlers)
                {
                    yield return effect.HandleEnterPassage();
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
        public IEnumerator HandleEnterPassage();
    }
}