using TDB.CraftSystem.EffectSystem.Data;
using TDB.Utils.UI.Tooltip;
using UnityEngine;

namespace TDB.CraftSystem.EffectSystem.UI
{
    public abstract class CraftMenuRecipeEffectItemUI : MonoBehaviour
    {
        private TooltipHandler _tooltip;

        protected virtual void Awake()
        {
            _tooltip = GetComponent<TooltipHandler>();
        }
        
        public virtual void BindEffectData(EffectData effectData)
        {
            _tooltip.SetTooltipText(effectData.GetTooltipText());
        }
    }

    public abstract class TypedCraftMenuRecipeEffectItemUI<T> : CraftMenuRecipeEffectItemUI where T : EffectData
    {
        protected abstract void BindEffectData(T effectData);
        
        public sealed override void BindEffectData(EffectData effectData)
        {
            base.BindEffectData(effectData);
            
            if (effectData is not T typedData)
            {
                throw new System.ArgumentException(
                    $"Typed effect for {typeof(T)} can only take parameters associated the same type.");
            }
            
            BindEffectData(typedData);
        }
    }
}