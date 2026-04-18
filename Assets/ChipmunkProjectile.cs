using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Rendering;
using UnityEditor.UI;
using UnityEngine;

namespace TDB
{
    public class ChipmunkProjectile : MonoBehaviour
    {
        // ================================
        // Fields
        // ================================
        [SerializeField] float lifetime = 5; // lifetime of the projectile in seconds
        private float timer = 0; // timer for calculating lifetime of the projectile
        private Rigidbody2D rb; // rigidbody of the projectile
        [SerializeField] float speed = 2; // speed of the projectile


        // ================================
        // Unity Lifecycle Methods
        // ================================

        // Start is called before the first frame update
        void Awake()
        {
            // start with this object disabled
            gameObject.SetActive(false);

            // get the rigidbody component
            rb = GetComponent<Rigidbody2D>();
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
                GetComponentInParent<EntityData>().DealDamage(collision.gameObject.transform.root.gameObject);
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
            // get direction to fire in
            Vector2 trajectory = ((Vector2)target.gameObject.transform.position - (Vector2)transform.position).normalized;

            // enable projectile
            gameObject.SetActive(true);

            // fire at target
            rb.velocity = trajectory * speed;
        }
    }
}
