using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    public class EnemyWander : MonoBehaviour
    {
        private EntityData _entityData;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void Awake()
        {
            _entityData = GetComponent<EntityData>();
        }
    }
}
