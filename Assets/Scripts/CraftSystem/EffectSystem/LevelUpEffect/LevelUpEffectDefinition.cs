using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sirenix.Serialization;
using TDB.CraftSystem.EffectSystem.Data;
using TDB.CraftSystem.EffectSystem.LevelUpEffect;
using UnityEngine;

[assembly: RegisterFormatter(typeof(RsoPathFormatter<LevelUpEffectDefinition>))]

namespace TDB.CraftSystem.EffectSystem.LevelUpEffect
{
    /// <summary>
    /// "LevelUp" means the effect is associated with a level attribute,
    /// which increases with more effects of the same definition get aggregated. 
    /// </summary>
    [CreateAssetMenu(fileName = "New Level Up Effect", menuName = "Data/Effects/Level Up Effect")]
    public class LevelUpEffectDefinition : TypedEffectDefinition<LevelUpEffectData>
    {
        [SerializeField] private string _attributeName;
        
        protected override LevelUpEffectData GenerateEffectData(List<TypedEffectParameter<LevelUpEffectData>> typedParams)
        {
            var totalProgress = typedParams
                .Where(p => p is LevelUpProgressParameter)
                .Sum(p => ((LevelUpProgressParameter)p).Progress);
            
            return new LevelUpEffectData(totalProgress);
        }

        protected override string GetIngredientEffectString(TypedEffectParameter<LevelUpEffectData> typedParam) =>
            typedParam is LevelUpProgressParameter p
                ? $"{_attributeName}{(p.Progress > 0 ? '+' : '-')}{(int)(Mathf.Abs(p.Progress) * 100)}%"
                : "Undefined Parameter";
        
        public string EffectName => _attributeName;
    }
}