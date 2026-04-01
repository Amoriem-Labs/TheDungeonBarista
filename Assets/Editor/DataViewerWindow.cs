#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.EffectSystem.LevelUpEffect;
using UnityEditor;
using UnityEngine;

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
        [PropertyOrder(-15)]
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

        // =======================
        // Raw Recipes tab actions
        // =======================
        [TabGroup("Raw Recipes")]
        // [HorizontalGroup("Raw Recipes/Actions", Width = 1)]
        [PropertyOrder(-10)]
        [Button(ButtonSizes.Medium)]
        private void CreateRawRecipe()
        {
            CreateAssetInFolderPrompt<RawRecipeDefinition>("RR-");
            RefreshAssets();
        }

        [TabGroup("Raw Recipes")]
        [TableList(IsReadOnly = false, AlwaysExpanded = true, ShowPaging = true, NumberOfItemsPerPage = 30)]
        [Searchable]
        public List<RawRecipeDefinition> _recipes = new();

        // =======================
        // Ingredients tab actions
        // =======================
        [TabGroup("Ingredients", order: 0)]
        // [HorizontalGroup("Ingredients/Actions", Width = 1)]
        [PropertyOrder(-10)]
        [Button(ButtonSizes.Medium)]
        private void CreateIngredient()
        {
            CreateAssetInFolderPrompt<IngredientDefinition>("ING-");
            RefreshAssets();
        }

        [TabGroup("Ingredients", order: 0)]
        [TableList(IsReadOnly = false, AlwaysExpanded = true, ShowPaging = true, NumberOfItemsPerPage = 30)]
        [Searchable]
        public List<IngredientDefinition> _ingredients = new();

        // ===========================
        // Level Up Effects tab actions
        // ===========================
        [TabGroup("Effect (Level Up)", order: 0)]
        // [HorizontalGroup("Effect (Level Up)/Actions", Width = 1)]
        [PropertyOrder(-10)]
        [Button(ButtonSizes.Medium)]
        private void CreateLevelUpEffect()
        {
            CreateAssetInFolderPrompt<LevelUpEffectDefinition>("LUE-");
            RefreshAssets();
        }

        [TabGroup("Effect (Level Up)", order: 0)]
        [TableList(IsReadOnly = false, AlwaysExpanded = true, ShowPaging = true, NumberOfItemsPerPage = 30)]
        [Searchable]
        public List<LevelUpEffectDefinition> _levelUpEffects = new();

        protected override void OnEnable()
        {
            base.OnEnable();
            LoadAssets();
        }

        // -------- Create asset helpers --------

        private void CreateAssetInFolderPrompt<T>(string prefix) where T : ScriptableObject
        {
            var folder = GetValidFolderOrAssets();
            var defaultName = prefix + typeof(T).Name;

            // Prompt user for name/path (recommended for assets)
            var path = EditorUtility.SaveFilePanelInProject(
                title: $"Create {typeof(T).Name}",
                defaultName: defaultName,
                extension: "asset",
                message: $"Choose where to create the {typeof(T).Name} asset.",
                path: folder
            );

            if (string.IsNullOrEmpty(path))
                return; // cancelled

            var asset = ScriptableObject.CreateInstance<T>();
            asset.name = Path.GetFileNameWithoutExtension(path);

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path);

            EditorGUIUtility.PingObject(asset);
            Selection.activeObject = asset;
        }

        private string GetValidFolderOrAssets()
        {
            if (!string.IsNullOrEmpty(Folder) && AssetDatabase.IsValidFolder(Folder))
                return Folder;
            return "Assets";
        }

        // -------- loading --------
        private void LoadAssets()
        {
            LoadAssets(ref _recipes);
            LoadAssets(ref _ingredients);
            LoadAssets(ref _levelUpEffects);
            Repaint();
        }

        private void LoadAssets<T>(ref List<T> rows) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(Folder) || !AssetDatabase.IsValidFolder(Folder))
            {
                rows = new List<T>();
                return;
            }

            var typeName = typeof(T).Name;
            var guids = AssetDatabase.FindAssets($"t:{typeName}", new[] { Folder });

            IEnumerable<T> objs = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(p => AssetDatabase.LoadAssetAtPath<T>(p))
                .Where(o => o != null);

            if (!string.IsNullOrWhiteSpace(NameFilter))
            {
                var nf = NameFilter.Trim();
                objs = objs.Where(o => o.name.IndexOf(nf, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            rows = objs.ToList();
        }

        private void ApplyNameFilter() => RefreshAssets();
    }
}
#endif