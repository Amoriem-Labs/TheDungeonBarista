#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TDB.Utils.Misc
{
    public static class CreateUtility
    {
        public static GameObject CreatePrefab(string path)
        {
            GameObject newObject = PrefabUtility.InstantiatePrefab(Resources.Load(path)) as GameObject;
            return newObject;
        }

        public static GameObject CreateObject(string name, params Type[] types)
        {
            GameObject newObject = ObjectFactory.CreateGameObject(name, types);
            return newObject;
        }

        public static void Place(GameObject gameObject, Transform parent, string assumedParentName = "", bool resetZ = true)
        {
            // Find location
            SceneView lastView = SceneView.lastActiveSceneView;
            gameObject.transform.position = lastView ? (resetZ ? lastView.pivot.SetZ(0) : lastView.pivot) : Vector3.zero;
            gameObject.transform.SetParent(parent);

            if (assumedParentName != "" && parent?.name != assumedParentName)
            {
                Debug.LogWarning($"The game object {gameObject.name} is supposed to be created as a child of {assumedParentName}.");
            }

            // Make sure we place the object in the proper scene, with a relevant name
            StageUtility.PlaceGameObjectInCurrentStage(gameObject);
            GameObjectUtility.EnsureUniqueNameForSibling(gameObject);

            // Record undo, and select
            Undo.RegisterCreatedObjectUndo(gameObject, $"Create Object: {gameObject.name}");
            Selection.activeGameObject = gameObject;

            // For prefabs, let's mark the scene as dirty for saving
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }


        // EXAMPLE
        //[MenuItem("GameObject/Circuits/Create Circuit Wire", true, -55)]
        //public static bool CreateCircuitValidation()
        //{
        //    return Selection.activeGameObject?.name == "Circuits";
        //}

        //[MenuItem("GameObject/Circuits/Create Circuit Wire", priority = -55)]
        //public static void CreateCircuitWire(MenuCommand menuCommand)
        //{
        //    var parent = menuCommand.context as GameObject;
        //    var go = CreateUtility.CreatePrefab("Circuits/CircuitWire");
        //    Place(go, parent?.transform, "Circuits");
        //}
    }
}
#endif
