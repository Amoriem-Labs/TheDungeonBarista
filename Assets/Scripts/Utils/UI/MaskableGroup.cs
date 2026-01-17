using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TDB
{
    [ExecuteAlways]
    [RequireComponent(typeof(MaskableGraphic))]
    public class MaskableGroup : MonoBehaviour
    {
        private MaskableGraphic _parentMaskable;
        [SerializeField, ReadOnly] private bool _maskable;
        private MaskableGraphic[] _childMaskables;

        private void Awake()
        {
            _parentMaskable = GetComponent<MaskableGraphic>();
            _maskable = _parentMaskable.maskable;

            _childMaskables = GetComponentsInChildren<MaskableGraphic>();

            SyncMaskable(true);
        }

        protected void SyncMaskable(bool force = false)
        {
            if (!force && _maskable == _parentMaskable.maskable) return;
            
            _maskable = _parentMaskable.maskable;
            foreach (var graphic in _childMaskables)
            {
                graphic.maskable = _maskable;
            }
        }
    }
}
