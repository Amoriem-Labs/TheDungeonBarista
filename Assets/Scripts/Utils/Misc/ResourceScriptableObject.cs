using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TDB.Utils.Misc
{
    /// <summary>
    /// SOs that has its resource path stored.
    /// Usually SOs cannot be deserialized properly after being loaded from save data.
    /// <s>They need to be manually loaded from Resources using ResourcesPath</s>.
    /// Use ResourceSOJsonConverter when serializing/deserializing to automate this process.
    /// </summary>
    public class ResourceScriptableObject : SerializedScriptableObject
    {
        [HideInTables]
        [SerializeField, ReadOnly]
        private string _resourcesPath;

        public string ResourcesPath => _resourcesPath;

        [ContextMenu("Load Resource Path")]
        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            _resourcesPath = ResourcePathUtility.GetResourcesPath(this);
#endif
        }

        private void Reset()
        {
#if UNITY_EDITOR
            _resourcesPath = ResourcePathUtility.GetResourcesPath(this);
#endif
        }
    }
}