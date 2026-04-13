using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    public class ChipmunkStunned : MonoBehaviour
    {
        private EntityData _entityData;
        private ChipmunkStateHandler _chipmunkStateHandler;
        private void Awake()
        {
            _entityData = GetComponent<EntityData>();
            _chipmunkStateHandler = GetComponent<ChipmunkStateHandler>();

            GetComponentInChildren<AttackHitbox>().dealDamage += GetComponent<EntityData>().DealDamage;
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
                _chipmunkStateHandler.ChangeState(ChipmunkStateHandler.States.wander);
            }
        }
    }

}
