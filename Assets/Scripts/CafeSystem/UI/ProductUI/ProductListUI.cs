using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Customers;
using TDB.CafeSystem.FurnitureSystem.FurnitureParts;
using TDB.CafeSystem.UI.OrderUI;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.EffectSystem.Data;
using TDB.GameManagers;
using TDB.GameManagers.SessionManagers;
using TDB.Utils.EventChannels;
using TDB.Utils.Misc;
using TMPro;
using UnityEngine;

namespace TDB.CafeSystem.UI.ProductUI
{
    public class ProductListUI : DynamicItemListUI<FinalRecipeData>
    {
        [SerializeField] private CustomerOrderUI _orderListUI;
        [SerializeField] private TextMeshProUGUI _productCountText;
        
        [Title("Events")]
        [SerializeField] private EventChannel _productionDeviceReadyEvent;
        [SerializeField] private EventChannel _productionDeviceNotReadyEvent;
        [SerializeField] private EventChannel _addProductEvent;
        [SerializeField] private EventChannel _serveProductEvent;
        
        private ProductionDeviceData _currentDevice;
        private ProductItemUI _currentItem;

        private readonly Dictionary<ProductData, ProductItemUI> _productItems = new();
        private int _maxProductCount;
        private MoneyManager _moneyManager;

        private static string ProductCountTemplate => "Space: {0} / {1}";

        private void Awake()
        {
            _maxProductCount = GameManager.Instance.GameConfig.ProductListCapacity;
            _productCountText.text = string.Format(ProductCountTemplate, _productItems.Count, _maxProductCount);

            _moneyManager = FindObjectOfType<MoneyManager>();
        }

        private void OnEnable()
        {
            _productionDeviceReadyEvent.AddListener<ProductionDeviceData>(HandleProductionDeviceReady);
            _productionDeviceNotReadyEvent.AddListener(HandleProductionDeviceNotReady);
            _addProductEvent.AddListener<ProductData>(HandleAddProduct);
            _serveProductEvent.AddListener<ServeProductInfo>(HandleServeProduct);
        }

        private void OnDisable()
        {
            _productionDeviceReadyEvent.RemoveListener<ProductionDeviceData>(HandleProductionDeviceReady);
            _productionDeviceNotReadyEvent.RemoveListener(HandleProductionDeviceNotReady);
            _addProductEvent.RemoveListener<ProductData>(HandleAddProduct);
            _serveProductEvent.RemoveListener<ServeProductInfo>(HandleServeProduct);
        }

        private void HandleProductionDeviceReady(ProductionDeviceData device)
        {
            if (_currentDevice != null)
            {
                Debug.LogError("Current device is still ready when a new device becomes ready.");
                return;
            }
            
            _currentDevice = device;
            _currentDevice.OnRecipeUpdated += HandleCurrentRecipeUpdate;

            _currentItem =
                ((ProductItemUI)AddItem(transform.position + new Vector3(100, 0), _currentDevice.ConfiguredRecipe))!;
            _currentItem.SetStartPosition();

            _currentItem.Outline.ToggleOutline(true);
        }

        private void HandleProductionDeviceNotReady()
        {
            if (_currentDevice == null)
            {
                Debug.LogError("There is no device ready.");
                return;
            }
            
            _currentDevice.OnRecipeUpdated -= HandleCurrentRecipeUpdate;
            _currentDevice = null;

            _currentItem.DestroyItemInProductList();
            _currentItem = null;
        }

        private void HandleCurrentRecipeUpdate()
        {
            _currentItem.BindData(_currentDevice.ConfiguredRecipe);
            // _currentItem.UpdateUI();
        }

        private void HandleAddProduct(ProductData product)
        {
            if (!_currentItem)
            {
                Debug.LogError("There should be some existing item for recipe preview before creating the product.");
                return;
            }
            
            // the current item becomes the one for the product
            _productItems.Add(product, _currentItem);
            _currentItem.BindData(product);
            _currentItem.Outline.ToggleOutline(false);
            
            _productCountText.text = string.Format(ProductCountTemplate, _productItems.Count, _maxProductCount);
            
            // create a new one if there is a current device
            if (_currentDevice == null)
            {
                _currentItem = null;
                return;
            }
            _currentItem =
                ((ProductItemUI)AddItem(_currentItem.transform.position, _currentDevice.ConfiguredRecipe))!;
            _currentItem.Outline.ToggleOutline(true);
        }

        private void HandleServeProduct(ServeProductInfo info)
        {
            var product = info.Product;
            if (!_productItems.Remove(product, out var productItemUI))
            {
                Debug.LogError("There should be some existing product item for the served product.");
                return;
            }

            _orderListUI.ServeOrder(info, productItemUI, _moneyManager);
            _productCountText.text = string.Format(ProductCountTemplate, _productItems.Count, _maxProductCount);
        }
    }

    public struct ServeProductInfo
    {
        public ProductData Product;
        public CustomerData Customer;
        public Dictionary<EffectDefinition, float> FlavorMultipliers;
        public float TotalFlavorMultiplier;
        public int FinalPrice;
    }
}