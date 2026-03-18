using System.Collections;
using System.Collections.Generic;
using TDB.Player.Input;
using UnityEngine;

namespace TDB
{
    public class CervBase : MonoBehaviour
    {
        private CervData _entityData;
        private void Awake()
        {
            _entityData = GetComponent<CervData>();

            GetComponentInChildren<CervHitBox>().dealDamage += GetComponent<CervData>().DealDamage;
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
           // _entityData.Velocity = new Vector2(Mathf.MoveTowards(_entityData.Velocity.x, 0, _entityData.Decceleration * Time.deltaTime)
             //                                    , Mathf.MoveTowards(_entityData.Velocity.y, 0, _entityData.Decceleration * Time.deltaTime));
        
            //_entityData.Rb.velocity = _entityData.Velocity;
        }
    }
}
