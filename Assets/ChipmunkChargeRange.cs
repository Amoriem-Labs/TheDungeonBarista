using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TDB
{
    public class ChipmunkChargeRange : MonoBehaviour
    {
        // Start is called before the first frame update

        public delegate void InRangeDelegate(EntityData player);
        public InRangeDelegate playerInRange;
        public InRangeDelegate playerOutOfRange;

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == EntityData._playerLayer)
            {
                // set state to wander
                GetComponentInParent<ChipmunkStateHandler>().ChangeState(ChipmunkStateHandler.States.wander);

                // set target as the player
                GetComponentInParent<ChipmunkWander>()._target = collision.gameObject.GetComponentInParent<EntityData>();
                
                // playerInRange?.Invoke(collision.gameObject.GetComponentInParent<EntityData>());
                // not sure what above line does but keeping it just in case its needed at some point
            }

                 
        }


        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == EntityData._playerLayer)
            {
                // change state to chase (is this the default state? i mean i have a feeling that the states
                // are currently a little crongled but i can fix that later if i need to)
                GetComponentInParent<ChipmunkStateHandler>().ChangeState(ChipmunkStateHandler.States.chase);

                // set target to null
                GetComponentInParent<ChipmunkWander>()._target = null;
            }
        }
        }
}
