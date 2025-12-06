using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.CafeSystem.FurnitureSystem.FurnitureParts;
using TDB.CafeSystem.UI.OrderUI;
using TDB.CafeSystem.UI.ProductUI;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.UI;
using TDB.CraftSystem.UI.Info;
using TDB.CraftSystem.UI.RecipeGraph;
using TDB.Utils.UI.ConfirmPanel;
using TDB.Utils.UI.Tooltip;
using UnityEngine;
using UnityEngine.Events;

namespace TDB.Utils.EventChannels
{
    [CreateAssetMenu(menuName = "Event/Event Channel", fileName = "New Event Channel")]
    public class EventChannel : VoidEvent
    {
        [ValueDropdown("GetAvailableTypes")]
        [EnableIf(nameof(CanEdit))]
        [OnValueChanged("ClearEventCallback")]
        [SerializeField]
        private string _paramType;

        [SerializeField, TextArea(10, 100), PropertyOrder(Single.MaxValue)] private string _comment;

        
        private bool CanEdit => !Application.isPlaying;
    
        private Type _cachedType;
        private Delegate _paramEventCallback;
        private Delegate _voidEventCallback;

        public Type EventType
        {
            get
            {
                if (_cachedType == null && !string.IsNullOrEmpty(_paramType))
                    _cachedType = Type.GetType(_paramType);
                return _cachedType;
            }
        }

        private IEnumerable<ValueDropdownItem<string>> GetAvailableTypes()
        {
            // You can hardcode or dynamically load types here
            var types = new List<Type>
            {
                null,
                
                // built-in data types
                typeof(object),
                typeof(int),
                typeof(float),
                typeof(string),
                typeof(bool),
                typeof(Vector2),
                typeof(Vector3),
            
                // custom data types
                typeof(ConfirmationData),
                typeof(TooltipData),

                #region CraftSystemTypes
                typeof(FinalRecipeData),
                typeof(ReturnIngredientInfo),
                typeof(IngredientNodeUI),
                typeof(DisplayIngredientInfo),
                typeof(OpenMenuInfo),
                #endregion

                #region CafeSystemTypes

                typeof(DisplayCustomerOrderInfo),
                typeof(ProductionDeviceData),
                typeof(ProductData),
                typeof(ServeProductInfo),

                #endregion
            };

            return types.Select(t =>
                new ValueDropdownItem<string>(t?.Name ?? "Null", t?.AssemblyQualifiedName ?? "Null"));
        }

        private void ClearEventCallback()
        {
            _cachedType = null;
            _paramEventCallback = null;
            _voidEventCallback = null;
        }

        /// <summary>
        /// Raise typed events and void events (default & optional).
        /// </summary>
        public void RaiseEvent<T>(T arg, bool triggerVoid = true)
        {
            EnsureCorrectType<T>();
            if (_paramEventCallback is UnityAction<T> paramCallback)
                paramCallback.Invoke(arg);
            if (triggerVoid && _voidEventCallback is UnityAction voidCallback)
                voidCallback.Invoke();
        }

        /// <summary>
        /// Raise void events.
        /// </summary>
        [Button("Raise Void Event")]
        public void RaiseEvent()
        {
            if (_voidEventCallback is UnityAction voidCallback)
                voidCallback.Invoke();
        }

        public void AddListener<T>(UnityAction<T> listener)
        {
            EnsureCorrectType<T>();
            _paramEventCallback = Delegate.Combine(_paramEventCallback, listener);
        }

        public override void AddListener(UnityAction listener)
        {
            _voidEventCallback = Delegate.Combine(_voidEventCallback, listener);
        }

        public void RemoveListener<T>(UnityAction<T> listener)
        {
            EnsureCorrectType<T>();
            _paramEventCallback = Delegate.Remove(_paramEventCallback, listener);
        }

        public override void RemoveListener(UnityAction listener)
        {
            _voidEventCallback = Delegate.Remove(_voidEventCallback, listener);
        }

        private void EnsureCorrectType<T>()
        {
            var expected = EventType;
            var actual = typeof(T);

            if (expected == null)
                throw new InvalidOperationException("Event type is not set.");

            if (expected != actual)
                throw new InvalidOperationException($"Type mismatch: expected {expected}, but got {actual}");
        }
    }
}