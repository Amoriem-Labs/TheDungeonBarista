using System;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Managers;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.CafeSystem.FurnitureSystem.FurnitureParts
{
    /// <summary>
    /// Cafe phase transition trigger.
    /// </summary>
    public class ShopDoor : MonoBehaviour
    {
        [Title("Events")]
        [SerializeField] private EventChannel _cafePreparationStartEvent;
        [SerializeField] private EventChannel _dungeonPreparationStartEvent;
        
        private CafePhaseController _controller;

        private enum DoorState
        {
            Invalid,
            WaitingToOpenShop,
            WaitingToEnterDungeon,
        }
        private DoorState _doorState = DoorState.Invalid;

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
            _doorState = DoorState.WaitingToOpenShop;
        }

        private void HandleDungeonPreparationStart()
        {
            _doorState = DoorState.WaitingToEnterDungeon;
        }

        [Button(ButtonSizes.Large), DisableInEditorMode]
        [EnableIf(nameof(_doorState), DoorState.WaitingToOpenShop)]
        private void OpenShop()
        {
            if (_doorState != DoorState.WaitingToOpenShop) return;
            _doorState = DoorState.Invalid;
            
            _controller.StartCafeOperation();
        }

        [Button(ButtonSizes.Large), DisableInEditorMode]
        [EnableIf(nameof(_doorState), DoorState.WaitingToEnterDungeon)]
        private void EnterDungeon()
        {
            if (_doorState != DoorState.WaitingToEnterDungeon) return;
            _doorState = DoorState.Invalid;
            
            _controller.EnterDungeon();
        }
    }
}