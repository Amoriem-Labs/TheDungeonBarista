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
        // TODO: the minigame canvas can probably be a child of this interaction trigger and gets referred to by it
        
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
            _customerServiceInteractionTrigger.CanAddProduct
            && InteractionState != EInteractionState.Invalid
            && base.GetCanInteract(device);

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
                StartCookingMinigame(recipe);
            }
        }

        [TitleGroup("Testing")]
        [Button(ButtonSizes.Large, ButtonStyle.FoldoutButton)]
        private void TestStartCookingMinigame()
        {
            // enforce to use manually set test data
            StartCookingMinigame(null);
        }
        
        private void StartCookingMinigame(FinalRecipeData recipe)
        {
            // block input before starting minigame
            ToggleBlockingPlayerInput(true);
                
            // TODO: cooking game
            // you probably need the following data
            // manually set them for testing
            var recipeName = recipe?.RecipeName ?? "Test Name";
            var basicPrice = recipe?.GetBasicPrice() ?? 100;
            var recipeQuality = recipe?.GetQualityLevel(_qualityEffect) ?? 3;
            // finish callback
            var finishCallback = new Action<MinigameOutcome>(outcome =>
            {
                // unblock input after finishing the minigame
                ToggleBlockingPlayerInput(false);
                // generate product info based on minigame output
                var product = new ProductData(recipeData: recipe, minigameOutcome: outcome);
                // send to service interaction trigger so it can be served to customers
                _customerServiceInteractionTrigger.AddProductToServe(product);
                // interactable update due to product creation
                TryUpdateCurrentInteractable();
            });
                
            // TODO: start cooking game
            // TODO: you probably need to bind input actions in the minigame
            //       please check out Scripts/Player/Input/InputController.cs on how to do it
            Debug.Log("Starting cooking minigame");
                
            // TODO: temporarily invoke finish callback directly for testing,
            // remove this when finishing the cooking game
            finishCallback.Invoke(new MinigameOutcome()
            {
                PriceMultiplier = 1f
            });
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