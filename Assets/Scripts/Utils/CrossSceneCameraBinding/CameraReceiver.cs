using System;
using UnityEngine;

namespace TDB.Utils.CrossSceneCameraBinding
{
    [DisallowMultipleComponent]
    public class CameraReceiver : MonoBehaviour
    {
        [SerializeField] private CameraType _cameraType;
        
        internal CameraType CameraType => _cameraType;
        public Camera BoundCamera { get; private set; }
        
        private void OnEnable()
        {
            CameraBindingManager.Instance.RegisterReceiver(this);
        }

        private void OnDisable()
        {
            CameraBindingManager.Instance.UnregisterReceiver(this);
        }

        internal virtual void BindCamera(Camera camera)
        {
            BoundCamera = camera;
        }
    }
}