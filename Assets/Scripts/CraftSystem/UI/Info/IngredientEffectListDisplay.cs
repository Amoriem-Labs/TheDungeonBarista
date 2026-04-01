using System.Collections.Generic;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.EffectSystem.UI;
using UnityEngine;

namespace TDB.CraftSystem.UI.Info
{
    public class IngredientEffectListDisplay
    {
        private readonly Transform _effectItemContainer;
        private readonly EffectPairItemUI _effectPairItemPrefab;
        private readonly List<EffectPairItemUI> _trackedItems = new();

        public IngredientEffectListDisplay(Transform effectItemContainer, EffectPairItemUI effectPairItemPrefab)
        {
            _effectItemContainer = effectItemContainer;
            _effectPairItemPrefab = effectPairItemPrefab;
        }

        public void DisplayIngredientEffectList(IngredientDefinition ingredient)
        {
            int i = 0;
            for (; i < ingredient.Effects.Count; i++)
            {
                EffectPairItemUI item = null;
                if (i >= _trackedItems.Count)
                {
                    item = Object.Instantiate(_effectPairItemPrefab, _effectItemContainer);
                    _trackedItems.Add(item);
                }
                else
                {
                    item = _trackedItems[i];
                    item.gameObject.SetActive(true);
                }
                
                var effect = ingredient.Effects[i];
                item.BindEffectParamPair(effect);
            }

            for (; i < _trackedItems.Count; i++)
            {
                _trackedItems[i].gameObject.SetActive(false);
            }
        }
    }
}