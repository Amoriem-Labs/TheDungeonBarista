using System.Collections;
using Cinemachine;
using UnityEngine;

namespace TDB.MapSystem
{
    public class PlayerCameraFollow : MonoBehaviour
    {
        [SerializeField] private string _playerTag = "Player";
        [SerializeField] private CinemachineVirtualCamera _vcam;
        [SerializeField] private float _pollIntervalSeconds = 0.25f;

        private Coroutine _bindRoutine;

        private void OnEnable()
        {
            _bindRoutine = StartCoroutine(BindWhenAvailable());
        }

        private void OnDisable()
        {
            if (_bindRoutine != null)
                StopCoroutine(_bindRoutine);
            _bindRoutine = null;
        }

        private IEnumerator BindWhenAvailable()
        {
            while (_vcam != null && _vcam.Follow == null)
            {
                GameObject target = GameObject.FindGameObjectWithTag(_playerTag);
                if (target != null)
                {
                    _vcam.Follow = target.transform;
                    yield break;
                }

                yield return new WaitForSeconds(_pollIntervalSeconds);
            }
        }
    }
}
