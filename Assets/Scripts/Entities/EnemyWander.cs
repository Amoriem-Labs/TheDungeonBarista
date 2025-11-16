using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;

namespace TDB
{
    public class EnemyWander : MonoBehaviour
    {
        private EntityData _entityData;
        public EntityData _target;
        private Vector2 _homePos = Vector2.zero;
        public Vector2 Range = new Vector2(10, 10);
        public float CooldownTimerMax = 5;
        private float _cooldownTimerCurrent;


        // Start is called before the first frame update
        void Start()
        {
            
        }

        public void ChooseNewPoint()
        {
            //_targetPos = _homePos + new Vector2(Random.Range(-Range.x, Range.x), Random.Range(-Range.y, Range.y));
            

        }

        public void SetHomePoint()
        {
            _homePos = this.transform.position;
        }
        // Update is called once per frame
        public void WanderUpdate()
        {
            //print("wandering and stuff");


           // print(_target);
            _entityData.Rb.velocity = _entityData.Velocity;

            _cooldownTimerCurrent -= Time.deltaTime;

            if (_target != null)
            {
                _entityData.Velocity = ( _target.transform.position - _entityData.transform.position ).normalized * _entityData.MaxSpeed;
            }
            else
            {
                _entityData.Velocity = new Vector2(Mathf.MoveTowards(_entityData.Velocity.x, 0, _entityData.Decceleration * Time.deltaTime)
                                          , Mathf.MoveTowards(_entityData.Velocity.y, 0, _entityData.Decceleration * Time.deltaTime));
            }


            // _entityData.Velocity = new Vector2(Mathf.MoveTowards(_entityData.Velocity.x, 0, _entityData.Decceleration * Time.deltaTime)
            //      , Mathf.MoveTowards(_entityData.Velocity.y, 0, _entityData.Decceleration * Time.deltaTime));



            if (_cooldownTimerCurrent < 0)
            {
                //_entityData.Velocity = _entityData.MaxSpeed * (_entityData.Rb.position - _target.Rb.position).normalized;
                //switch to attack
                _cooldownTimerCurrent = CooldownTimerMax;
            }
        }

        private void Awake()
        {
            _entityData = GetComponent<EntityData>();
            ChooseNewPoint();
            _cooldownTimerCurrent = CooldownTimerMax;
            GetComponentInChildren<ChargeRange>().playerInRange += PlayerInRange;
            GetComponentInChildren<ChargeRange>().playerInRange += PlayerOutOfRange;
            
        }

        public void PlayerInRange(EntityData player)
        {
            _target = player.GetComponent<EntityData>();
        }
        public void PlayerOutOfRange(EntityData player)
        {
            _target = null;
        }


    }
}
