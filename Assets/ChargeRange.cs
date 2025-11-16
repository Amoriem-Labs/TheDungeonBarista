using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TDB
{
    public class ChargeRange : MonoBehaviour
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
                GetComponentInParent<EnemyWander>()._target = collision.gameObject.GetComponentInParent<EntityData>();
               // playerInRange?.Invoke(collision.gameObject.GetComponentInParent<EntityData>());
            }

                 
        }


        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == EntityData._playerLayer)
            {
                GetComponentInParent<EnemyWander>()._target = null;
            }
        }
        }
}
