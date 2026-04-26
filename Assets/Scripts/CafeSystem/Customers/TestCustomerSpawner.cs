using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Managers;
using TDB.Utils.EventChannels;
using TDB.Utils.ObjectPools;
using UnityEngine;

namespace TDB.CafeSystem.Customers
{
    public class TestCustomerSpawner : MonoObjectPool<CustomerController>
    {
        [Title("Spawner Config")]
        [SerializeField] private List<Transform> _spawnPoints;
        [SerializeField] private float _spawnInterval = 5f;
        [SerializeField] private CafeTimeController _cafeTimeController;
        
        [Title("Events")]
        [SerializeField] private EventChannel _cafeOperationStartEvent;
        [SerializeField] private EventChannel _cafeOperationEndEvent;

        private readonly Dictionary<Transform, CustomerController> _trackedCustomers = new();
        private Coroutine _spawnCoroutine;

        protected override void Awake()
        {
            base.Awake();
            
            if (_cafeTimeController == null)
            {
                _cafeTimeController = FindObjectOfType<CafeTimeController>();
            }
        }

        private void OnEnable()
        {
            _cafeOperationStartEvent.AddListener(HandleCafeOperationStart);
            _cafeOperationEndEvent.AddListener(HandleCafeOperationEnd);
        }

        private void OnDisable()
        {
            _cafeOperationStartEvent.RemoveListener(HandleCafeOperationStart);
            _cafeOperationEndEvent.RemoveListener(HandleCafeOperationEnd);
        }

        private void HandleCafeOperationStart()
        {
            if (_cafeTimeController == null)
            {
                Debug.LogError($"{nameof(TestCustomerSpawner)} requires a {nameof(CafeTimeController)} to spawn customers.", this);
                return;
            }

            _spawnCoroutine = StartCoroutine(SpawnCustomerCoroutine());
        }

        private void HandleCafeOperationEnd()
        {
            if (_spawnCoroutine == null) return;
            StopCoroutine(_spawnCoroutine);

            ClearWaitingCustomers();
        }

        private IEnumerator SpawnCustomerCoroutine()
        {
            while (true)
            {
                // wait for valid position
                yield return new WaitUntil(() => _spawnPoints.Any(p => !_trackedCustomers.ContainsKey(p)));

                var targetCafeTime = _cafeTimeController.CafeTime + _spawnInterval;
                yield return new WaitUntil(() => _cafeTimeController.CafeTime >= targetCafeTime);

                var spawnPoint = _spawnPoints.First(p => !_trackedCustomers.ContainsKey(p));
                var customer = Get(spawnPoint.position, Quaternion.identity);
                _trackedCustomers.Add(spawnPoint, customer);
                customer.SpawnCustomer(OnCustomerFinish);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void OnCustomerFinish(CustomerController customer)
        {
            var kv = _trackedCustomers.First(kv => kv.Value == customer);
            kv.Value.DestroyCustomer();
            _trackedCustomers.Remove(kv.Key);
        }

        private void ClearWaitingCustomers()
        {
            var kvs = _trackedCustomers.ToList();
            foreach (var (spawnPoint, customer) in kvs)
            {
                // customers who are eating will destroy themselves
                if (customer.Status == CustomerStatus.Eating) continue;
                
                customer.DestroyCustomer();
                _trackedCustomers.Remove(spawnPoint);
            }
        }
    }
}
