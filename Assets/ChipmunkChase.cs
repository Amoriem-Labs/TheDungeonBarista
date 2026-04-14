using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    public class ChipmunkChase : MonoBehaviour
    {
        // reference to the chipmunk's entity data
        private EntityData _chipmunk;


        // Start is called before the first frame update
        void Start()
        {
        
        }

        void Awake()
        {
            // get the entity data of the chipmunk
            _chipmunk = GetComponent<EntityData>();
        }

        // Update is called once per frame, set to act as update by the chipmunk state handler
        public void ChaseUpdate()
        {
            // stop movement
            _chipmunk.Velocity = new Vector2(Mathf.MoveTowards(_chipmunk.Velocity.x, 0, _chipmunk.Decceleration * Time.deltaTime)
                                          , Mathf.MoveTowards(_chipmunk.Velocity.y, 0, _chipmunk.Decceleration * Time.deltaTime));

            _chipmunk.Rb.velocity = _chipmunk.Velocity;
        }

        // NOTE: THIS IS PROBABLY THE WRONG WAY AROUND WITH THE WANDER SCRIPT. 
        // I DONT KNOW WHATS GOING ON WITH THIS BUT THIS IS HOW ITS GOING TO BE RIGHT NOW.
    }
}
