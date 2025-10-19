using Sirenix.Serialization;
using TDB.CraftSystem.EffectSystem.Data;
using TDB.CraftSystem.EffectSystem.LevelUpEffect;
using UnityEngine;

[assembly: RegisterFormatter(typeof(RsoPathFormatter<LevelUpProgressParameter>))]

namespace TDB.CraftSystem.EffectSystem.LevelUpEffect
{
    public class LevelUpProgressParameter : TypedEffectParameter<LevelUpEffectData>
    {
        [field: SerializeField] public float Progress { get; private set; }
    }
}