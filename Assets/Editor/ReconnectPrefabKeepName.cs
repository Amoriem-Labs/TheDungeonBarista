#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TDB.Editor
{
    public static class ReconnectPrefabKeepName
    {
        private const string MenuPath = "GameObject/Prefab/Reconnect Prefab (Keep Name)…";
        private static bool _waitingForPicker;

        [MenuItem(MenuPath, validate = true)]
        private static bool Validate()
        {
            return Selection.gameObjects != null &&
                   Selection.gameObjects.Length > 0 &&
                   Selection.gameObjects.All(go => go && !EditorUtility.IsPersistent(go));
        }

        [MenuItem(MenuPath)]
        private static void Run()
        {
            // Open Unity’s built-in object picker
            EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "t:Prefab", 0);
            _waitingForPicker = true;
            EditorApplication.update += WaitForObjectPickerClose;
        }

        private static void WaitForObjectPickerClose()
        {
            if (!_waitingForPicker)
                return;

            // Picker closes -> ObjectPickerControlID resets to 0
            if (EditorGUIUtility.GetObjectPickerObject() is GameObject prefabAsset &&
                GUIUtility.hotControl == 0) // hotControl resets after picker closes
            {
                _waitingForPicker = false;
                EditorApplication.update -= WaitForObjectPickerClose;
                DoConvert(prefabAsset);
            }
        }

        private static void DoConvert(GameObject prefabAsset)
        {
            if (!prefabAsset)
            {
                Debug.LogWarning("[Reconnect Prefab] No prefab selected.");
                return;
            }

            if (!EditorUtility.IsPersistent(prefabAsset))
            {
                EditorUtility.DisplayDialog("Invalid Prefab",
                    "The selected object is not a prefab asset.", "OK");
                return;
            }

            // sort by hierarchy depth to convert parents before children
            var targets = Selection.gameObjects
                .Where(go => go && !EditorUtility.IsPersistent(go))
                .OrderBy(go => GetDepth(go.transform))
                .ToArray();

            Undo.IncrementCurrentGroup();
            int group = Undo.GetCurrentGroup();

            foreach (var go in targets)
            {
                Undo.RegisterFullObjectHierarchyUndo(go, "Reconnect Prefab (Keep Name)");

                var settings = new ConvertToPrefabInstanceSettings
                {
                    changeRootNameToAssetName = false,
                    componentsNotMatchedBecomesOverride = true,
                    recordPropertyOverridesOfMatches = true,
                    gameObjectsNotMatchedBecomesOverride = true,
                };

                PrefabUtility.ConvertToPrefabInstance(go, prefabAsset, settings,
                    InteractionMode.UserAction);

                EditorSceneManager.MarkSceneDirty(go.scene);
            }

            Undo.CollapseUndoOperations(group);
        }

        private static int GetDepth(Transform t)
        {
            int depth = 0;
            while (t.parent != null)
            {
                depth++;
                t = t.parent;
            }
            return depth;
        }
    }
}
#endif
