using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.Utils.Misc
{
    [RequireComponent(typeof(Image))]
    public class ImageOutlineController : MonoBehaviour
    {
        private static readonly int OutlineAlpha = Shader.PropertyToID("_OutlineAlpha");
        private Material _material;

        private List<object> _sources = new();
        
        private void Awake()
        {
            var sprite = GetComponent<Image>();
            Debug.Assert(sprite.material.HasFloat(OutlineAlpha), $"{gameObject.name} has no outline material.");
            
            _material = new Material(sprite.material);
            sprite.material = _material;
        }

        public void ToggleOutline(bool enable, object source = null)
        {
            // force override if no source is provided
            if (source == null)
            {
                _sources.Clear();
                _material.SetFloat(OutlineAlpha, enable ? 1f : 0f);
                return;
            }
            
            // no change
            if (enable == _sources.Contains(source)) return;
            if (enable)
            {
                _sources.Add(source);
                _material.SetFloat(OutlineAlpha, 1f);
            }
            else
            {
                _sources.Remove(source);
                if (_sources.Count == 0)
                {
                    _material.SetFloat(OutlineAlpha, 0f);
                }
            }
        }
    }
}