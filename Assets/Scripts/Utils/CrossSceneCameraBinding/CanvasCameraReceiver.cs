using UnityEngine;

namespace TDB.Utils.CrossSceneCameraBinding
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasCameraReceiver : CameraReceiver
    {
        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        internal override void BindCamera(Camera camera)
        {
            base.BindCamera(camera);
            
            _canvas.worldCamera = camera;
        }
    }
}