using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
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
        
        [Title("Events")]
        [SerializeField] private EventChannel _cafeOperationStartEvent;
        [SerializeField] private EventChannel _cafeOperationEndEvent;

        private readonly Dictionary<Transform, CustomerController> _trackedCustomers = new();
        private Coroutine _spawnCoroutine;

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
                // wait for a while before spawn
                yield return new WaitForSeconds(_spawnInterval);

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