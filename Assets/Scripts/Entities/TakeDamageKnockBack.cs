using UnityEngine;

namespace TDB
{
    public class TakeDamageKnockBack : MonoBehaviour, ITakeDamageHandler
    {
        // TODO: fix the knock back force in the damage target for the moment
        [SerializeField] private float _knockBackForce = 5f;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponentInParent<Rigidbody2D>();
        }

        public void TakeDamage(DamageData damage)
        {
            if (!damage.KnockBackSource) return;
            var direction = _rb.transform.position - damage.KnockBackSource.position;
            direction.Normalize();
            _rb.AddForce(direction * _knockBackForce, ForceMode2D.Impulse);
            Debug.Log(direction * _knockBackForce);
        }
    }
}