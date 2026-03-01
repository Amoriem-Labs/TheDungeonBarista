using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TDB
{
    public class Hurtbox : MonoBehaviour
    {
        private List<ITakeDamageHandler> _takeDamageHandlers;

        private void Awake()
        {
            _takeDamageHandlers = GetComponentsInChildren<ITakeDamageHandler>().ToList();
            
            var entityData = GetComponentInParent<EntityData>();
            _takeDamageHandlers.Add(entityData);
        }

        public void TakeDamage(DamageData damage)
        {
            foreach (var handler in _takeDamageHandlers)
            {
                handler.TakeDamage(damage);
            }
        }
    }

    public interface ITakeDamageHandler
    {
        public void TakeDamage(DamageData damage);
    }

    public struct DamageData
    {
        public float Amount;
        // set to null for damage without knock back
        public Transform KnockBackSource;
    }
}
