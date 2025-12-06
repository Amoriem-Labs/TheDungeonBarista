using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.EffectSystem.Data;
using TDB.CraftSystem.EffectSystem.UI;
using TDB.Utils.EventChannels;
using UnityEngine;

namespace TDB.CraftSystem.UI.Info
{
    public class RecipeEffectInfoUI : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        
        [Title("Events")]
        [SerializeField] private EventChannel _onRecipeSelectedEvent;
        [SerializeField] private EventChannel _onRecipeUpdatedEvent;
        
        private FinalRecipeData _recipe;
        private readonly Dictionary<EffectDefinition, CraftMenuRecipeEffectItemUI> _trackedEffectItems = new();

        private void OnEnable()
        {
            _onRecipeSelectedEvent.AddListener<FinalRecipeData>(HandleRecipeSelected);
            _onRecipeUpdatedEvent.AddListener(HandleRecipeUpdated);
        }

        private void OnDisable()
        {
            _onRecipeSelectedEvent.RemoveListener<FinalRecipeData>(HandleRecipeSelected);
            _onRecipeUpdatedEvent.RemoveListener(HandleRecipeUpdated);
        }

        private void HandleRecipeSelected(FinalRecipeData recipe)
        {
            _recipe = recipe;

            HandleRecipeUpdated();
        }

        private void HandleRecipeUpdated()
        {
            // get all effects
            var effects = _recipe?.GetAllEffectData() ?? new List<EffectData>();
            // always effect not in the recipe
            foreach (var (definition, itemUI) in _trackedEffectItems)
            {
                if (effects.Any(e => e.Definition == definition)) continue;
                itemUI.gameObject.SetActive(false);
            }
            // display all effects from the recipe
            foreach (var effectData in effects)
            {
                var definition = effectData.Definition;
                if (!_trackedEffectItems.TryGetValue(definition, out var itemUI))
                {
                    // create new one if not tracked before
                    itemUI = Instantiate(definition.CraftMenuRecipeEffectItemPrefab, _container);
                    _trackedEffectItems.Add(definition, itemUI);
                }
                itemUI.gameObject.SetActive(true);
                // bind data
                itemUI.BindEffectData(effectData);
            }
        }
    }
}