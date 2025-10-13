using System;
using UnityEngine;

//using Sirenix.OdinInspector;

namespace TDB.Utils.SceneTransitions
{
    /// <summary>
    /// SceneAsset is a wrapper around the Unity's Scene Asset.
    /// It allows referencing scenes easier and can associate extra data to the scene.
    /// Can be created by right-clicking the Scene and select "Generate Scene Asset".
    /// </summary>
    [CreateAssetMenu(fileName = "New Scene")]
    [Serializable]
    public class SceneAsset : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField] private UnityEditor.SceneAsset _sceneAsset;
#endif
        [SerializeField]
        //[SerializeField, ReadOnly]
        private string _sceneName;

        public string SceneName => _sceneName;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_sceneAsset != null && _sceneAsset.name != _sceneName)
            {
                _sceneName = _sceneAsset.name;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }

        [UnityEditor.MenuItem("Assets/Generate Scene Asset", true, priority = -50)]
        public static bool CreateAudioAssetValidation(UnityEditor.MenuCommand menuCommand)
        {
            return UnityEditor.Selection.activeObject is UnityEditor.SceneAsset;
        }


        [UnityEditor.MenuItem("Assets/Generate Scene Asset", priority = -50)]
        public static void CreateAudioAsset(UnityEditor.MenuCommand menuCommand)
        {
            SceneAsset asset = ScriptableObject.CreateInstance<SceneAsset>();
            asset._sceneAsset = UnityEditor.Selection.activeObject as UnityEditor.SceneAsset;
            asset._sceneName = asset._sceneAsset.name;
            var path = UnityEditor.AssetDatabase.GetAssetOrScenePath(asset._sceneAsset);
            path = path.Replace(".unity", ".asset");

            string name = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);
            UnityEditor.AssetDatabase.CreateAsset(asset, name);
            UnityEditor.AssetDatabase.SaveAssets();

            UnityEditor.EditorUtility.FocusProjectWindow();

            UnityEditor.Selection.activeObject = asset;
        }
#endif
    }
}