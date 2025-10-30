using TDB.CraftSystem.EffectSystem.Data;
using UnityEngine;

namespace TDB.CafeSystem.Customers
{
    [CreateAssetMenu(fileName = "New Flavor", menuName = "Data/Flavor", order = 0)]
    public class FlavorDefinition : ScriptableObject
    {
        [field: SerializeField] public EffectDefinition EffectDefinition { get; private set; }

        public string Name => EffectDefinition.EffectName;
        
        // TODO: extra data
    }
}