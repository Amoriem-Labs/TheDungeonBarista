using TDB.CraftSystem.Data;
using TDB.GameManagers;
using TDB.Utils.Misc;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using UnityEngine;

namespace TDB.DungeonSystem.Collectibles
{
    [CreateAssetMenu(menuName = "Data/Collectibles", fileName = "CollectibleDefinition", order = 0)]
    public class CollectibleDefinition : ResourceScriptableObject
    {
        [SerializeField] public ICollectibleSource Definition;
        public string ItemName => Definition.ItemName;

        public void Transfer(GameData gameData, int amount) => Definition.Transfer(gameData, amount);

#if UNITY_EDITOR
        [MenuItem("Assets/CreateIngredientsCollectible", false, 0)]
        private static void CreateIngredientsCollectible()
        {
            var ingredients = Resources.LoadAll<IngredientDefinition>("Data/CraftSystem/Ingredients");

            // Target folder (you can change this)
            string targetFolder = "Assets/Resources/Data/Collectibles/Ingredients";

            // Ensure folder exists
            if (!AssetDatabase.IsValidFolder(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
                AssetDatabase.Refresh();
            }

            foreach (var ingredient in ingredients)
            {
                if (ingredient == null) continue;

                string assetName = $"{ingredient.name}_Collectible.asset";
                string assetPath = Path.Combine(targetFolder, assetName);

                // Avoid duplicates
                var existing = AssetDatabase.LoadAssetAtPath<CollectibleDefinition>(assetPath);
                if (existing != null)
                {
                    Debug.Log($"Skipping existing: {assetName}");
                    continue;
                }

                // Create new CollectibleDefinition
                var collectible = ScriptableObject.CreateInstance<CollectibleDefinition>();
                collectible.name = $"{ingredient.name}_Collectible";
                collectible.Definition = ingredient; // IngredientDefinition implements ICollectibleSource

                // Create asset
                AssetDatabase.CreateAsset(collectible, assetPath);
                EditorUtility.SetDirty(collectible);

                Debug.Log($"Created: {assetPath}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

#endif
    }
}