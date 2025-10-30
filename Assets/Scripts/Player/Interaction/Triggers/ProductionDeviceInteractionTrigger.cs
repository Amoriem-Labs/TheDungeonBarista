using System;
using Sirenix.OdinInspector;
using TDB.CafeSystem.FurnitureSystem.FurnitureParts;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.EffectSystem.Data;
using TDB.GameManagers;
using TDB.Utils.EventChannels;
using UnityEngine;
using UnityEngine.Serialization;

namespace TDB.Player.Interaction.Triggers
{
    /// <summary>
    /// There might exist many production devices.
    /// So the interaction logic is handled here.
    /// </summary>
    public class ProductionDeviceInteractionTrigger : InteractionTrigger<ProductionDevice>
    {
        [TitleGroup("References", order: -1)]
        [SerializeField] private CustomerServiceInteractionTrigger _customerServiceInteractionTrigger;
        
        [TitleGroup("Events")]
        [SerializeField] private EventChannel _cafePreparationStartEvent;
        [SerializeField] private EventChannel _cafePreparationEndEvent;
        [SerializeField] private EventChannel _cafeOperationStartEvent;
        [SerializeField] private EventChannel _cafeOperationEndEvent;
        [SerializeField] private EventChannel _dungeonPreparationStartEvent;
        [SerializeField] private EventChannel _dungeonPreparationEndEvent;

        private enum EInteractionState
        {
            Invalid,
            CafePreparation,
            CafeOperation,
            DungeonPreparation,
        }
        private EInteractionState _interactionState = EInteractionState.Invalid;
        
        private EffectDefinition _qualityEffect;

        private EInteractionState InteractionState
        {
            get => _interactionState;
            set
            {
                if (_interactionState == value) return;
                _interactionState = value;
                TryUpdateCurrentInteractable();
            }
        }

        private void Awake()
        {
            _qualityEffect = GameManager.Instance.GameConfig.QualityEffect;
        }

        private void OnEnable()
        {
            _cafePreparationStartEvent.AddListener(HandleCafePreparationStart);
            _cafeOperationStartEvent.AddListener(HandleCafeOperationStart);
            _dungeonPreparationStartEvent.AddListener(HandleDungeonPreparationStart);
            
            _cafePreparationEndEvent.AddListener(DisableInteraction);
        }

        private void OnDisable()
        {
            _cafePreparationStartEvent.RemoveListener(HandleCafePreparationStart);
            _cafeOperationStartEvent.RemoveListener(HandleCafeOperationStart);
            _dungeonPreparationStartEvent.RemoveListener(HandleDungeonPreparationStart);
            
            _cafePreparationEndEvent.RemoveListener(DisableInteraction);
        }

        #region Interaction Trigger Interface

        private void HandleCafePreparationStart()
        {
            InteractionState = EInteractionState.CafePreparation;
        }

        private void HandleCafeOperationStart()
        {
            InteractionState = EInteractionState.CafeOperation;
        }

        private void HandleDungeonPreparationStart()
        {
            InteractionState = EInteractionState.DungeonPreparation;
        }

        private void DisableInteraction()
        {
            InteractionState = EInteractionState.Invalid;
        }

        public override string InteractionTip => InteractionState switch
        {
            EInteractionState.Invalid => "Invalid",
            EInteractionState.CafePreparation => "Configure",
            EInteractionState.CafeOperation => "Cook",
            EInteractionState.DungeonPreparation => "Cook",
            _ => throw new ArgumentOutOfRangeException()
        };

        protected override bool GetCanInteract(ProductionDevice device) =>
            InteractionState != EInteractionState.Invalid && base.GetCanInteract(device);

        protected override void Interact(ProductionDevice device)
        {
            switch (InteractionState)
            {
                case EInteractionState.Invalid:
                    break;
                case EInteractionState.CafePreparation:
                    InteractCafePreparation(device);
                    break;
                case EInteractionState.CafeOperation:
                    InteractCafeOperation(device);
                    break;
                case EInteractionState.DungeonPreparation:
                    InteractDungeonPreparation(device);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Actual Interactions

        /// <summary>
        /// Configure device.
        /// </summary>
        /// <param name="device"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void InteractCafePreparation(ProductionDevice device)
        {
            Debug.Log("Interacting cafe preparation");
            ToggleBlockingPlayerInput(true);
            device.ConfigureRecipe(() =>
            {
                ToggleBlockingPlayerInput(false);
            });
        }

        /// <summary>
        /// Try to cook products.
        /// </summary>
        /// <param name="device"></param>
        private void InteractCafeOperation(ProductionDevice device)
        {
            var success = device.TryCookProduct(out var recipe);
            if (recipe == null)
            {
                // TODO: notify player
                Debug.Log($"{device.gameObject.name} has no recipe configure.");
            }
            else if (!success)
            {
                // TODO: notify player
                Debug.Log($"There is not enough ingredients.");
            }
            else
            {
                // TODO: cooking game
                //      remember to set _isBlockedByAction
                // TODO: bind product to CustomerInteractionTrigger
                Debug.Log(
                    $"Selected recipe: {recipe.RecipeName}\n\t" +
                    $"The recipe uses {recipe.RawRecipe.RecipeName} as the raw recipe.\n\t" +
                    $"The recipe has {recipe.GetAllEffectData().Count} effects.\n\t"
                );

                // TODO: generate product data from recipe and minigame result
                // currently use the quality level of recipe as the quality level of the product
                var product = new ProductData(recipeData: recipe, qualityLevel: recipe.GetQualityLevel(_qualityEffect));
                // send to service interaction trigger so it can be served to customers
                _customerServiceInteractionTrigger.AddProductToServe(product);
            }
        }

        /// <summary>
        /// Try to cook consumables.
        /// </summary>
        /// <param name="device"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void InteractDungeonPreparation(ProductionDevice device)
        {
            Debug.LogWarning($"We don't support cooking consumables right now.");
        }

        #endregion
    }
}