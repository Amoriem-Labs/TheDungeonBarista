using System.Collections.Generic;
using TDB.Utils.Singletons;
using UnityEngine;

namespace TDB.Utils.CrossSceneCameraBinding
{
    public class CameraBindingManager : PassiveSingleton<CameraBindingManager>
    {
        private Dictionary<CameraType, Camera> _cameras = new();
        private Dictionary<CameraType, List<CameraReceiver>> _receivers = new();

        public void RegisterReceiver(CameraReceiver receiver)
        {
            var type = receiver.CameraType;
            // ensure key exists
            if (!_receivers.TryGetValue(type, out var receivers))
            {
                receivers = new List<CameraReceiver>();
                _receivers.Add(type, receivers);
            }
            // ensure only one
            if (receivers.Contains(receiver))
            {
                Debug.LogError("Receiver already registered");
                return;
            }
            // add receiver
            receivers.Add(receiver);
            // bind camera
            if (_cameras.TryGetValue(type, out var camera) && camera != null)
            {
                receiver.BindCamera(camera);
            }
        }

        public void UnregisterReceiver(CameraReceiver receiver)
        {
            var type = receiver.CameraType;
            if (!_receivers.TryGetValue(type, out var receivers) || !receivers.Contains(receiver))
            {
                Debug.LogError("Receiver not registered with the right type");
                return;
            }
            receivers.Remove(receiver);
            receiver.BindCamera(null);
        }

        public void RegisterCamera(CameraType cameraType, Camera camera)
        {
            _cameras[cameraType] = camera;
            if (!_receivers.TryGetValue(cameraType, out var receivers)) return;
            foreach (var receiver in receivers)
            {
                receiver.BindCamera(camera);
            }
        }

        public void UnregisterCamera(CameraType cameraType, Camera camera)
        {
            // do nothing if the camera was bound to another camera
            if (_cameras.TryGetValue(cameraType, out var currentCamera) && currentCamera != camera) return;
            // unbind
            RegisterCamera(cameraType, null);
        }
    }
}