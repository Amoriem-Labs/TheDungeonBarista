using System;
using Cinemachine;
using UnityEngine;

namespace TDB.MapSystem
{
    public class CameraAreaController : MonoBehaviour
    {
        [SerializeField] private string _targetTag = "Player";
        [SerializeField] private CinemachineVirtualCamera _vcam;
        
        private void Awake()
        {
            _vcam.gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(_targetTag))
            {
                _vcam.gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(_targetTag))
            {
                _vcam.gameObject.SetActive(false);
            }
        }
    }
}
