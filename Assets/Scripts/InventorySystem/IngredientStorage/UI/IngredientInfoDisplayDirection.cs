using System;
using TDB.Utils.Misc;
using UnityEngine;

namespace TDB.InventorySystem.IngredientStorage.UI
{
    [ExecuteAlways]
    public class IngredientInfoDisplayDirection : MonoBehaviour
    {
        [SerializeField, Range(-1, 1)] private int _xDirection;
        [SerializeField, Range(-1, 1)] private int _yDirection;
        [SerializeField] private RectTransform _parent;
        [SerializeField] private RectTransform _child;
        private Camera _camera;
        private Vector3 _anchorMin;
        private Vector3 _anchorMax;

        private void Awake()
        {
            var canvas = GetComponentInParent<Canvas>();
            _camera = canvas?.worldCamera;
        }

        private void Update()
        {
            if (!_camera) return;
            
            var viewPos = _camera.WorldToViewportPoint(transform.position);
            var viewOffset = viewPos - new Vector3(.5f, .5f);

            var xPivot = _xDirection == 0 ? 0.5f : _xDirection * Mathf.Sign(viewOffset.x) > 0 ? 1 : 0;
            var yPivot = _yDirection == 0 ? 0.5f : _yDirection * Mathf.Sign(viewOffset.y) > 0 ? 1 : 0;

            var pivot = new Vector2(xPivot, yPivot);
            if (_parent && (_parent.anchorMin != pivot || _parent.anchorMax != pivot))
            {
                _parent.anchorMin = pivot;
                _parent.anchorMax = pivot;
                _parent.pivot = pivot;
                _parent.anchoredPosition = Vector2.zero;
            }

            var childPivot = Vector2.one - pivot;
            if (_child && (_child.anchorMin != childPivot || _child.anchorMax != childPivot))
            {
                _child.anchorMin = childPivot;
                _child.anchorMax = childPivot;
                _child.pivot = childPivot;
                _child.anchoredPosition = Vector2.zero;
            }
        }
    }
}
