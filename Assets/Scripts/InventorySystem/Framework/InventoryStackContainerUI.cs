using System;
using System.Collections.Generic;
using TDB.CraftSystem.Data;
using TDB.InventorySystem.IngredientStorage.UI;
using TDB.Utils.Misc;
using UnityEngine;

namespace TDB.InventorySystem.Framework
{
    public class InventoryStackContainerUI<T> : MonoBehaviour where T : ResourceScriptableObject
    {
        [SerializeField] private InventoryStackUI<T> _stackUIPrefab;
        [SerializeField] private Transform _container;

        private readonly ListItemBuffer<InventoryStackUI<T>> _listItemBuffer = new();
        
        public void Clear()
        {
            _listItemBuffer.Clear();
        }

        public void SetInventory(InventoryData<T> inventory, Predicate<InventoryStackData<T>> hidePolicy = null)
        {
            foreach (InventoryStackData<T> item in inventory)
            {
                if (hidePolicy?.Invoke(item) == true) continue;

                AddItem(item);
            }
        }

        public void HideItem(InventoryStackUI<T> stack) => _listItemBuffer.RemoveItem(s => s == stack);

        public InventoryStackUI<T> FindItem(Predicate<T> predicate) =>
            _listItemBuffer.FindItem(item => predicate(item.Stack.Definition));

        public void AddItem(InventoryStackData<T> newStack) =>
            _listItemBuffer.AddItem(
                () => GameObject.Instantiate(_stackUIPrefab, _container),
                stackUI => stackUI.SetStack(newStack));
    }

    public class ListItemBuffer<T> where T : MonoBehaviour
    {
        private readonly List<T> _trackedItems = new List<T>();
        private int _currentIndex = 0;
        
        public void Clear()
        {
            foreach (var item in _trackedItems)
            {
                item.gameObject.SetActive(false);
            }
            
            _currentIndex = 0;
        }
        
        public T FindItem(Predicate<T> predicate)
        {
            var idx = _trackedItems.FindIndex(predicate);
            if (idx < 0 ||  idx >= _currentIndex) return null;
            return _trackedItems[idx];
        }

        public void AddItem(Func<T> initializer, Action<T> onActivate)
        {
            T item;
            if (_currentIndex < _trackedItems.Count)
            {
                item = _trackedItems[_currentIndex];
            }
            else
            {
                item = initializer();
                _trackedItems.Add(item);
            }
            item.gameObject.SetActive(true);
            onActivate(item);
            _currentIndex++;
        }

        public void RemoveItem(Predicate<T> predicate)
        {
            var idx = _trackedItems.FindIndex(predicate);
            if (idx < 0 ||  idx >= _currentIndex) return;
            var item = _trackedItems[idx];
            _trackedItems.RemoveAt(idx);
            item.gameObject.SetActive(false);
            _trackedItems.Add(item);
            _currentIndex--;
        }
    }
}