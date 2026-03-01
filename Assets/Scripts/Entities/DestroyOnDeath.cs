using UnityEngine;

namespace TDB
{
    [RequireComponent(typeof(EntityData))]
    public class DestroyOnDeath : MonoBehaviour
    {
        private EntityData _entity;

        private void Awake()
        {
            _entity = GetComponent<EntityData>();
        }

        private void OnEnable()
        {
            _entity.OnDeath += HandleDeath;
        }

        private void OnDisable()
        {
            _entity.OnDeath -= HandleDeath;
        }

        private void HandleDeath()
        {
            Destroy(gameObject);
        }
    }
}