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
        [SerializeField] private RectTransform _child;
        private RectTransform _parent;
        private Camera _camera;
        private Vector3 _anchorMin;
        private Vector3 _anchorMax;

        private void Awake()
        {
            _parent = transform as RectTransform;

            var canvas = GetComponentInParent<Canvas>();
            _camera = canvas.worldCamera;
        }

        private void Update()
        {
            var viewPos = _camera.WorldToViewportPoint((_anchorMin + _anchorMax) / 2);
            var viewOffset = viewPos - new Vector3(.5f, .5f);

            var xPivot = _xDirection == 0 ? 0.5f : _xDirection * Mathf.Sign(viewOffset.x) > 0 ? 1 : 0;
            var yPivot = _yDirection == 0 ? 0.5f : _yDirection * Mathf.Sign(viewOffset.y) > 0 ? 1 : 0;

            var position = new Vector3(Mathf.Lerp(_anchorMin.x, _anchorMax.x, xPivot),
                Mathf.Lerp(_anchorMin.y, _anchorMax.y, yPivot));
            if (_parent.position != position.SetZ(0))
            {
                _parent.position = position.SetZ(0);
                // _parent.anchoredPosition = Vector2.zero;
            }

            var childPivot = Vector2.one - new Vector2(xPivot, yPivot);
            if (_child.anchorMin != childPivot)
            {
                _child.anchorMin = childPivot;
                _child.anchorMax = childPivot;
                _child.pivot = childPivot;
                _child.anchoredPosition = Vector2.zero;
            }
        }

        public void SetAnchors(Vector3 infoAnchorMin, Vector3 infoAnchorMax)
        {
            _anchorMin = infoAnchorMin;
            _anchorMax = infoAnchorMax;
        }
    }
}
