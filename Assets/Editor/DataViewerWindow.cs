#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.EffectSystem.LevelUpEffect;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace STLT.Editor
{
    public class DataViewerWindow : OdinEditorWindow
    {
        [MenuItem("Tools/TDB/Data Viewer")]
        private static void Open()
        {
            var w = GetWindow<DataViewerWindow>();
            w.titleContent = new GUIContent("Data Viewer");
            w.position = new Rect(100, 100, 1100, 640);
            w.Show();
        }

        // -------- Query Controls --------
        [BoxGroup("Search"), FolderPath(AbsolutePath = false, RequireExistingPath = true)]
        [OnValueChanged(nameof(RefreshAssets))]
        [LabelText("Folder")]
        public string Folder = "Assets";

        [BoxGroup("Search"), LabelText("Include Subfolders")]
        [OnValueChanged(nameof(RefreshAssets))]
        public bool IncludeSubfolders = true;

        [BoxGroup("Search"), LabelText("Name Contains")]
        [OnValueChanged(nameof(ApplyNameFilter))]
        public string NameFilter = "";

        [BoxGroup("Search"), Button(ButtonSizes.Medium)]
        public void RefreshAssets() => LoadAssets();

        // [ShowInInspector, ReadOnly, PropertyOrder(99)]
        // [LabelText("Count")]
        // private int Count => _rows?.Count ?? 0;

        // -------- Table --------
        [TabGroup("Raw Recipes", order:0)]
        [TableList(IsReadOnly = false, AlwaysExpanded = true, ShowPaging = true, NumberOfItemsPerPage = 30)]
        [Searchable] // quick search over the row text
        public List<RawRecipeDefinition> _recipes = new();
        
        [TabGroup("Ingredients", order:0)]
        [TableList(IsReadOnly = false, AlwaysExpanded = true, ShowPaging = true, NumberOfItemsPerPage = 30)]
        [Searchable] // quick search over the row text
        public List<IngredientDefinition> _ingredients = new();
        
        [TabGroup("Effect (Level Up)", order:0)]
        [TableList(IsReadOnly = false, AlwaysExpanded = true, ShowPaging = true, NumberOfItemsPerPage = 30)]
        [Searchable] // quick search over the row text
        public List<LevelUpEffectDefinition> _levelUpEffects = new();


        protected override void OnEnable()
        {
            base.OnEnable();
            LoadAssets();
        }

        private void LoadAssets()
        {
            LoadAssets(ref _recipes);
            LoadAssets(ref _ingredients);
            LoadAssets(ref _levelUpEffects);
            // LoadAssets(ref _enemies);
            // LoadAssets(ref _structures);
            // LoadAssets(ref _planetMods);
            // LoadAssets(ref _galaxies);

            Repaint();
        }

        private void LoadAssets<T>(ref List<T> rows) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(Folder) || !AssetDatabase.IsValidFolder(Folder))
            {
                rows = new List<T>();
                return;
            }

            var typeName = typeof(T).Name;                  // <-- not nameof(T)
            var guids = AssetDatabase.FindAssets($"t:{typeName}", new[] { Folder });

            var objs = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(p => AssetDatabase.LoadAssetAtPath<T>(p))
                .Where(o => o != null);

            if (!string.IsNullOrWhiteSpace(NameFilter))
            {
                var nf = NameFilter.Trim();
                objs = objs.Where(o => o.name.IndexOf(nf, System.StringComparison.OrdinalIgnoreCase) >= 0);
            }

            rows = objs.ToList();
        }


        private void ApplyNameFilter()
        {
            RefreshAssets();
        }
    }
}
#endif