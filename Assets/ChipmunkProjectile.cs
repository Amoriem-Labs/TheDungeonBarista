using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace TDB
{
    public class ChipmunkProjectile : MonoBehaviour
    {
        // ================================
        // Fields
        // ================================
        [SerializeField] float lifetime = 5; // lifetime of the projectile in seconds
        private float timer = 0;

        private Rigidbody2D _rb;


        // ================================
        // Unity Lifecycle Methods
        // ================================

        // Start is called before the first frame update
        void Awake()
        {
            // start with this object disabled
            gameObject.SetActive(false);

            // get the rigidbody component
            _rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
            // lifetime timer
            if (timer < lifetime)
            {
                // increase timer by deltatime
                timer += Time.deltaTime;
            }
            else
            {
                // reset projectile
                gameObject.SetActive(false); // deactivate
                // set position to the position of the parent (main body)
                gameObject.transform.position = gameObject.transform.parent.transform.position; 

                // reset timer
                timer = 0;
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            // if the object is a player then deal damage
            if (collision.gameObject.layer == EntityData._playerLayer)
            {
                GetComponentInParent<EntityData>().DealDamage(collision.gameObject);
            }

            // check if the gameobject is not itself or its parent
            if (collision.transform.root != transform.root)
            {
                // if so, reset timer and such
                // reset projectile and timer
                gameObject.SetActive(false); // deactivate
                // set position to the position of the parent (main body)
                transform.position = transform.root.position;
                timer = 0;
            }
        }

        // ================================
        // Public Methods
        // ================================

        public void FireProjectile(EntityData target)
        {
            // enable projectile
            gameObject.SetActive(true);

            // get positon of target

            // fire at target
            // FIXME: does not fire at target for debugging purposes
            _rb.velocity = new Vector2(0,1);
        }
    }
}
