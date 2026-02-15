using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TDB.CraftSystem.EffectSystem;
using TDB.CraftSystem.EffectSystem.Data;
using TDB.ShopSystem;
using TDB.ShopSystem.Framework;
using TDB.Utils.Misc;
using UnityEngine;

[assembly: RegisterFormatter(typeof(RsoPathFormatter<TDB.CraftSystem.Data.IngredientDefinition>))]

namespace TDB.CraftSystem.Data
{
    [CreateAssetMenu(fileName = "New Ingredient", menuName = "Data/Craft System/Ingredient Definition", order = 0)]
    public class IngredientDefinition : ResourceScriptableObject, IShopItemDefinition
    {
        [TableColumnWidth(180, resizable: false)]
        [VerticalGroup("Ingredient Info")]
        [LabelText("Name")]
        [LabelWidth(40)]
        [SerializeField] private string _ingredientName;
        
        [PreviewField(ObjectFieldAlignment.Center)]
        // [VerticalGroup("Ingredient Info")]
        [TableColumnWidth(120, resizable: false)]
        [SerializeField] private Sprite _ingredientSprite;
        
        [LabelWidth(40)]
        [VerticalGroup("Ingredient Info")]
        [SerializeField] private IngredientTypeDefinition _type;
        
        [TableList(AlwaysExpanded = true), HideLabel, TitleGroup("Effects")]
        [SerializeField] private List<EffectParamPair> _effects;

        public string IngredientName => _ingredientName;
        public Sprite IngredientSprite => _ingredientSprite;

        public IngredientTypeDefinition Type => _type;

        public List<EffectParamPair> Effects => _effects;

        public int GetEssence()
        {
            // TODO: find a better essence computation
            // placeholder: the number of effects determines the essence
            return 1 + Effects.Count;
        }
    }
}