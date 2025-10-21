using System.Collections.Generic;
using TDB.CraftSystem.EffectSystem.UI;
using TDB.Utils.Misc;
using UnityEngine;

namespace TDB.CraftSystem.EffectSystem.Data
{
    /// <summary>
    /// Each instance represents a specific effect in the data layer.
    /// </summary>
    public abstract class EffectData
    {
        public EffectDefinition Definition { get; protected set; }
        
        internal virtual void SetDefinition(EffectDefinition definition) => Definition = definition;

        public abstract string GetTooltipText();
    }

    /// <summary>
    /// Each instance represents a familiar of effects with the same effect but different parameters.
    /// </summary>
    public abstract class EffectDefinition : ResourceScriptableObject
    {
        [field: SerializeField]
        public CraftMenuRecipeEffectItemUI CraftMenuRecipeEffectItemPrefab { get; protected set; } 
        
        /// <summary>
        /// Aggregates all effect instances defined by the list of parameters to generate the data. 
        /// </summary>
        public abstract EffectData GenerateEffectData(List<EffectParameterDefinition> parameters);

        /// <summary>
        /// String description of an ingredient/node effect.
        /// </summary>
        public abstract string GetIngredientEffectString(EffectParameterDefinition parameter);
    }

    /// <summary>
    /// Each instance represents the parameter of a specific effect.
    /// </summary>
    public abstract class EffectParameterDefinition : ResourceScriptableObject
    {
        
    }

    [System.Serializable]
    public struct EffectParamPair
    {
        [SerializeField] public EffectDefinition Effect;
        [SerializeField] public EffectParameterDefinition Parameter;

        public override string ToString() => Effect.GetIngredientEffectString(Parameter);
    }
}