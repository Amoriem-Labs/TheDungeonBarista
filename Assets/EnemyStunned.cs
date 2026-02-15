using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    public class EnemyStunned : MonoBehaviour
    {
        private EntityData _entityData;
        private BaseEnemyStateHandler _baseEnemyStateHandler;
        private void Awake()
        {
            _entityData = GetComponent<EntityData>();
            _baseEnemyStateHandler = GetComponent<BaseEnemyStateHandler>();

            //GetComponentInChildren<AttackHitbox>().dealDamage += GetComponent<EntityData>().DealDamage;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        public void StunnedUpdate()
        {
            
            _entityData.Velocity = new Vector2(Mathf.MoveTowards(_entityData.Velocity.x, 0, _entityData.Decceleration * Time.deltaTime)
                                                 , Mathf.MoveTowards(_entityData.Velocity.y, 0, _entityData.Decceleration * Time.deltaTime));

            _entityData.Rb.velocity = _entityData.Velocity;

            if (_entityData.Rb.velocity == new Vector2(0, 0))
            {
                _baseEnemyStateHandler.ChangeState(BaseEnemyStateHandler.States.wander);
            }
        }
    }

}
