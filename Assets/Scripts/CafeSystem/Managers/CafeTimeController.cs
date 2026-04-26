using System;
using TDB.GameManagers;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.CafeSystem.Managers
{
    public class CafeTimeController : MonoBehaviour
    {
        [SerializeField] private EventChannel _cafeOperationStartEvent;

        private bool _operationStarted;
        private float _cafeTime;
        private float _cafeTimeScale = 1f;
        private float _totalTime;
        private CafePhaseController _phaseController;

        public float OperationProgress => Mathf.Clamp01(_cafeTime / _totalTime);
        public float CafeTime => _cafeTime;

        private void Awake()
        {
            _totalTime = GameManager.Instance.GameConfig.CafeOperationTime;
            _phaseController = FindObjectOfType<CafePhaseController>();
        }

        private void OnEnable()
        {
            _cafeOperationStartEvent.AddListener(HandleCafeOperationStart);
        }

        private void OnDisable()
        {
            _cafeOperationStartEvent.RemoveListener(HandleCafeOperationStart);
        }

        private void Update()
        {
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            if (!_operationStarted) return;
            if (OperationProgress >= 1) return;

            _cafeTime += Time.deltaTime * _cafeTimeScale;
            if (OperationProgress >= 1) _phaseController.EndCafeOperation();
        }

        private void HandleCafeOperationStart()
        {
            _totalTime = GameManager.Instance.GameConfig.CafeOperationTime;
            
            _operationStarted = true;
            _cafeTime = 0;
            _cafeTimeScale = 1f;
        }
    }
}