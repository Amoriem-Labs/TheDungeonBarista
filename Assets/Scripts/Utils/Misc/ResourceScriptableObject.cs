using Sirenix.OdinInspector;
using UnityEngine;

namespace TDB.Utils.Misc
{
    /// <summary>
    /// SOs that has its resource path stored.
    /// Usually SOs cannot be deserialized properly after being loaded from save data.
    /// They need to be manually loaded from Resources using ResourcesPath.
    /// </summary>
    public class ResourceScriptableObject : SerializedScriptableObject
    {
        [SerializeField, ReadOnly]
        private string _resourcesPath;

        public string ResourcesPath => _resourcesPath;

        [ContextMenu("Load Resource Path")]
        private void OnValidate()
        {
#if UNITY_EDITOR
            _resourcesPath = ResourcePathUtility.GetResourcesPath(this);
#endif
        }
    }
}