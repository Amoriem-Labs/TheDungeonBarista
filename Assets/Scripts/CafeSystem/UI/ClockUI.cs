using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Managers;
using TDB.GameManagers;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.CafeSystem.UI
{
    public class ClockUI : MonoBehaviour
    {
        [SerializeField] private Transform _clockBody;
        
        [Title("Events")]
        [SerializeField] private EventChannel _cafeOperationStartEvent;
        
        private CafePhaseController _cafeController;

        private void Awake()
        {
            _cafeController = FindObjectOfType<CafePhaseController>();
        }

        private void OnEnable()
        {
            _cafeOperationStartEvent.AddListener(HandleCafeOperationStart);
        }

        private void OnDisable()
        {
            _cafeOperationStartEvent.RemoveListener(HandleCafeOperationStart);
        }

        private void HandleCafeOperationStart()
        {
            var operationTime = GameManager.Instance.GameConfig.CafeOperationTime;
            
            _clockBody.right = Vector3.down;
            _clockBody.DORotate(new Vector3(0, 0, 180), operationTime)
                .SetRelative(true)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _cafeController.EndCafeOperation();
                });
        }
    }
}
