using UnityEngine;

namespace TDB.Utils.CrossSceneCameraBinding
{
    [RequireComponent(typeof(Camera))]
    public class CameraBinder : MonoBehaviour
    {
        [SerializeField] private CameraType _cameraType;
        
        private Camera _camera;

        private void Awake()
        {
            _camera = gameObject.GetComponent<Camera>();
        }

        private void OnEnable()
        {
            if (CameraBindingManager.Instance == null)
            {
                Debug.LogWarning($"[CameraBinder] CameraBindingManager is NULL when enabling {gameObject.name}. Camera will not be registered.");
                return;
            }
            CameraBindingManager.Instance.RegisterCamera(_cameraType, _camera);
        }

        private void OnDisable()
        {
            CameraBindingManager.Instance.UnregisterCamera(_cameraType, _camera);
        }
    }
}