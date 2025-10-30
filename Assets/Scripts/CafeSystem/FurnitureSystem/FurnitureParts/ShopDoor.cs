using System;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Managers;
using TDB.Player.Interaction;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.CafeSystem.FurnitureSystem.FurnitureParts
{
    /// <summary>
    /// Cafe phase transition trigger.
    /// </summary>
    public class ShopDoor : MonoBehaviour, IInteractable
    {
        [Title("Events")]
        [SerializeField] private EventChannel _cafePreparationStartEvent;
        [SerializeField] private EventChannel _dungeonPreparationStartEvent;
        
        private CafePhaseController _controller;
        [SerializeField] private EDoorState _doorState = EDoorState.Invalid;

        private enum EDoorState
        {
            Invalid,
            WaitingToOpenShop,
            WaitingToEnterDungeon,
        }

        private EDoorState DoorState
        {
            get => _doorState;
            set
            {
                if (_doorState == value) return;
                _doorState = value;
                OnInteractableUpdated?.Invoke();
            }
        }

        private void Awake()
        {
            _controller = FindObjectOfType<CafePhaseController>();
        }

        private void OnEnable()
        {
            _cafePreparationStartEvent.AddListener(HandleCafePreparationStart);
            _dungeonPreparationStartEvent.AddListener(HandleDungeonPreparationStart);
        }

        private void OnDisable()
        {
            _cafePreparationStartEvent.RemoveListener(HandleCafePreparationStart);
            _dungeonPreparationStartEvent.RemoveListener(HandleDungeonPreparationStart);
        }

        private void HandleCafePreparationStart()
        {
            DoorState = EDoorState.WaitingToOpenShop;
        }

        private void HandleDungeonPreparationStart()
        {
            DoorState = EDoorState.WaitingToEnterDungeon;
        }

        [Button(ButtonSizes.Large), DisableInEditorMode]
        [EnableIf(nameof(DoorState), EDoorState.WaitingToOpenShop)]
        private void OpenShop()
        {
            if (DoorState != EDoorState.WaitingToOpenShop) return;
            DoorState = EDoorState.Invalid;
            
            _controller.StartCafeOperation();
        }

        [Button(ButtonSizes.Large), DisableInEditorMode]
        [EnableIf(nameof(DoorState), EDoorState.WaitingToEnterDungeon)]
        private void EnterDungeon()
        {
            if (DoorState != EDoorState.WaitingToEnterDungeon) return;
            DoorState = EDoorState.Invalid;
            
            _controller.EnterDungeon();
        }

        #region Interaction

        public bool IsInteractable => DoorState != EDoorState.Invalid;
        public Action OnInteractableUpdated { get; set; }

        public string InteractionTip =>
            DoorState switch
            {
                EDoorState.Invalid => "Invalid",
                EDoorState.WaitingToOpenShop => "Open Shop",
                EDoorState.WaitingToEnterDungeon => "Enter Dungeon",
                _ => throw new ArgumentOutOfRangeException()
            };

        public void Interact()
        {
            switch (DoorState)
            {
                case EDoorState.Invalid:
                    break;
                case EDoorState.WaitingToOpenShop:
                    OpenShop();
                    break;
                case EDoorState.WaitingToEnterDungeon:
                    EnterDungeon();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetReady()
        {
            // TODO: update sprite
        }

        public void SetNotReady()
        {
            // TODO: update sprite
        }

        #endregion
    }
}