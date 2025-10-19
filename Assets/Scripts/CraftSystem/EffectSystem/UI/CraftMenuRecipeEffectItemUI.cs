using TDB.CraftSystem.EffectSystem.Data;
using UnityEngine;

namespace TDB.CraftSystem.EffectSystem.UI
{
    public abstract class CraftMenuRecipeEffectItemUI : MonoBehaviour
    {
        public abstract void BindEffectData(EffectData effectData);
    }

    public abstract class TypedCraftMenuRecipeEffectItemUI<T> : CraftMenuRecipeEffectItemUI where T : EffectData
    {
        protected abstract void BindEffectData(T effectData);
        
        public sealed override void BindEffectData(EffectData effectData)
        {
            if (effectData is not T typedData)
            {
                throw new System.ArgumentException(
                    $"Typed effect for {typeof(T)} can only take parameters associated the same type.");
            }
            
            BindEffectData(typedData);
        }
    }
}