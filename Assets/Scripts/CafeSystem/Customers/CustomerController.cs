using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TDB.CafeSystem.UI.OrderUI;
using TDB.GameManagers;
using TDB.Utils.EventChannels;
using TDB.Utils.Misc;
using TDB.Utils.ObjectPools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TDB.CafeSystem.Customers
{
    /// <summary>
    /// Top-level controller of the customer and holds the CustomerData
    /// </summary>
    public class CustomerController : MonoBehaviour, IPooledObject<CustomerController>
    {
        [SerializeField, ReadOnly, InlineProperty, HideLabel, BoxGroup("Customer Data")]
        private CustomerData _customerData;

        [Title("Events")]
        [SerializeField]
        private EventChannel _displayCustomerOrderEvent;
        
        private SpriteOutlineController _spriteOutlineController;
        
        private System.Action _onCustomerDataUpdated;
        private MonoObjectPool<CustomerController> _pool;
        private Action<CustomerController> _onCustomerFinish;

        public SpriteOutlineController OutlineController => _spriteOutlineController;

        #region Data Access

        private CustomerData Data
        {
            get => _customerData;
            set
            {
                _customerData = value;
                _onCustomerDataUpdated?.Invoke();
            }
        }

        public bool IsPreferenceRevealed
        {
            get => Data.IsPreferenceRevealed;
            set
            {
                Debug.Log($"Change preference reveal from {Data.IsPreferenceRevealed} to {value}");
                if (Data.IsPreferenceRevealed == value) return;
                Data.IsPreferenceRevealed = value;
                _onCustomerDataUpdated?.Invoke();
            }
        }

        public List<CustomerPreferenceData> Preferences
        {
            get => Data.Preferences;
            set
            {
                Data.Preferences = value;
                _onCustomerDataUpdated?.Invoke();
            }
        }

        public CustomerStatus Status
        {
            get => Data.Status;
            set
            {
                if (Data.Status == value) return;
                Data.Status = value;
                _onCustomerDataUpdated?.Invoke();

                if (Data.Status == CustomerStatus.Eating)
                {
                    StartEating();
                }
            }
        }

        public void BindOnCustomerDataUpdatedCallback(System.Action action)
        {
            _onCustomerDataUpdated += action;
        }

        #endregion

        private void Awake()
        {
            _spriteOutlineController = GetComponentInChildren<SpriteOutlineController>();
        }

        /// <summary>
        /// Handles the initialization of the customer.
        /// Invoked by customer spawner to initialize data and start behavior.
        /// </summary>
        /// <param name="onCustomerFinish"></param>
        [Button(ButtonSizes.Large), DisableInEditorMode]
        public void SpawnCustomer(Action<CustomerController> onCustomerFinish)
        {
            // initialize data
            InitializeRandomData();
            
            // TODO: walk to a table before entering the Waiting state
            Data.Status = CustomerStatus.Waiting;
            _onCustomerDataUpdated?.Invoke();

            _onCustomerFinish = onCustomerFinish;
            
            // TODO: temporary intro animation
            transform.DOScale(1, .2f)
                .From(0)
                .SetEase(Ease.OutBack);
        }
        
        private void InitializeRandomData()
        {
            var allFlavors = GameManager.Instance.GameConfig.AllFlavors.ToList();
            allFlavors.Shuffle();
            var currentFlavorIdx = 0;

            var preferences = new List<CustomerPreferenceData>();
            var (minLevel, maxLevel) = (-3, 3);
            var (minStep, maxStep) = (2, 3);
            for (int currentLevel = Random.Range(minLevel, 0);
                 currentLevel <= maxLevel && currentFlavorIdx < allFlavors.Count;
                 currentLevel += Random.Range(minStep, maxStep + 1))
            {
                if (currentLevel == 0)
                {
                    // instead of randomly decide, always go up
                    // currentLevel = Random.Range(0f, 1f) < 0.5f ? -1 : 1;
                    currentLevel = 1;
                }
                preferences.Add(new CustomerPreferenceData()
                {
                    Flavor = allFlavors[currentFlavorIdx],
                    PreferenceLevel = currentLevel
                });
                currentFlavorIdx++;
            }
            
            Data = new CustomerData(preferences);

            Data.OnReadyUpdate += UpdateSpriteOutline;
        }

        private void StartEating()
        {
            IEnumerator EatingCoroutine()
            {
                yield return new WaitForSeconds(GameManager.Instance.GameConfig.CustomerEatTime);

                FinishEating();
            }

            StartCoroutine(EatingCoroutine());
        }

        private void FinishEating()
        {
            _onCustomerFinish?.Invoke(this);
        }

        [Button(ButtonSizes.Large), DisableInEditorMode]
        public void RevealPreferences()
        {
            _displayCustomerOrderEvent?.RaiseEvent<DisplayCustomerOrderInfo>(new DisplayCustomerOrderInfo()
            {
                CustomerData = _customerData,
                CustomerPosition = transform.position
            });
        }

        #region PooledObject

        public void SetPool(MonoObjectPool<CustomerController> pool)
        {
            _pool = pool;
        }

        public void DestroyCustomer()
        {
            // TODO: temporary outro animation
            transform.DOScale(0, .2f)
                .From(1)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    if (_pool)
                    {
                        _pool.Release(this);
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                });
        }

        #endregion
        
        public void SetReady(object source) => _customerData.SetReady(true, source);

        public void SetNotReady(object source) => _customerData.SetReady(false, source);

        private void UpdateSpriteOutline() =>
            OutlineController.ToggleOutline(_customerData.IsReady);
    }
}