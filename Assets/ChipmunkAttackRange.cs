using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    public class ChipmunkAttackRange : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        // enter attack range trigger
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // check if player
            if (collision.gameObject.layer == EntityData._playerLayer)
            {
                // set state to attacking
                GetComponentInParent<ChipmunkStateHandler>().ChangeState(ChipmunkStateHandler.States.attack);
                
               // playerInRange?.Invoke(collision.gameObject.GetComponentInParent<EntityData>());
               // dont know what this line does bu ill keep it just in case

               // set wander target to null
               GetComponentInParent<ChipmunkWander>()._target = null;
            }

                 
        }

        // leave attack range trigger
        private void OnTriggerExit2D(Collider2D collision)
        {
            // check if player
            if (collision.gameObject.layer == EntityData._playerLayer)
            {
                // set state to wandering
                GetComponentInParent<ChipmunkStateHandler>().ChangeState(ChipmunkStateHandler.States.wander);

                // set wander target to player
                GetComponentInParent<ChipmunkWander>()._target = collision.gameObject.GetComponentInParent<EntityData>();
            }
        }
    }
}
