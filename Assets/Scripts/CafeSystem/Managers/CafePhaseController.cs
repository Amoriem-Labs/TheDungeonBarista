using Sirenix.OdinInspector;
using TDB.GameManagers;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.CafeSystem.Managers
{
    public class CafePhaseController : MonoBehaviour
    {
        [Title("Events")]
        [SerializeField] private EventChannel _cafePreparationStartEvent;
        [SerializeField] private EventChannel _cafePreparationEndEvent;
        [SerializeField] private EventChannel _cafeOperationStartEvent;
        [SerializeField] private EventChannel _cafeOperationEndEvent;
        [SerializeField] private EventChannel _dungeonPreparationStartEvent;
        [SerializeField] private EventChannel _dungeonPreparationEndEvent;
        
        // temporary
        private bool _canEndOperation = false;
        
        /// <summary>
        /// Invoked once by the GameManager when the scene transition finishes.
        /// </summary>
        [Button(ButtonSizes.Large), DisableInEditorMode]
        public static void FindAndStart()
        {
            var controller = FindObjectOfType<CafePhaseController>();
            if (!controller)
            {
                Debug.LogError("CafePhaseController not found");
                return;
            }
            controller.StartCafePreparation();
        }

        public void StartCafePreparation()
        {
            // TODO: maybe some (blocking) animation before preparation phase
            _cafePreparationStartEvent.RaiseEvent();
        }

        public void StartCafeOperation()
        {
            _cafePreparationEndEvent.RaiseEvent();
            // TODO: maybe some (blocking) animation before operation phase
            _cafeOperationStartEvent.RaiseEvent();

            _canEndOperation = true;
        }

        [InfoBox("What is the condition for closing the shop? End the operation hour here for the moment.")]
        [Button(ButtonSizes.Large), DisableInEditorMode]
        [EnableIf(nameof(_canEndOperation))]
        public void EndCafeOperation()
        {
            _canEndOperation = false;
            
            _cafeOperationEndEvent.RaiseEvent();
            // TODO: maybe some (blocking) animation for closing the cafe
            _dungeonPreparationStartEvent.RaiseEvent();
        }

        public void EnterDungeon()
        {
            _dungeonPreparationEndEvent.RaiseEvent();
            // TODO: maybe some (blocking) animation before leaving the cafe scene
            GameManager.Instance.CafeToDungeon();
        }
    }
}