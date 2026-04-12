using System;
using System.Collections;
using System.Collections.Generic;
using TDB.Damage;
using UnityEngine;

namespace TDB
{
    public class Hurtbox : MonoBehaviour, IDamageable
    {
        private EntityData _entityData;

        private void Awake()
        {
            _entityData = GetComponentInParent<EntityData>();
        }

        public void TakeDamage(DamageData damage)
        {
            int ownerLayer = damage.DamageSourceLayer;
            int targetLayer = gameObject.layer;

            // TODO: damage layer matrix
            
            // bool playerHitsEnemy = ownerLayer == _playerLayer && targetLayer == _enemyLayer;
            // bool enemyHitsPlayer = ownerLayer == _enemyLayer && targetLayer == _playerLayer;

            if (ownerLayer != targetLayer)
            {
                _entityData.CurrentHealth -= damage.Amount;
                //Debug.Log($"Damage applied to {targetData.name}, CurrentHealth={targetData.CurrentHealth}");
                if (_entityData.CurrentHealth <= 0)
                    Destroy(_entityData.gameObject);
            }
            
            if (targetLayer == AttackHitbox._playerLayer)
            {
                _entityData.GetComponent<PlayerStateHandler>().ChangeState(PlayerStateHandler.States.stunned);
            }
            
            if (targetLayer == AttackHitbox._enemyLayer)
            {
                _entityData.GetComponent<BaseEnemyStateHandler>().ChangeState(BaseEnemyStateHandler.States.stunned);
            }
        }
    }
}
