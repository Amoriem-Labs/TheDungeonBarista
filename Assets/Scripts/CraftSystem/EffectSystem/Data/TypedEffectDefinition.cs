using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TDB.CraftSystem.EffectSystem.Data
{
    /// <summary>
    /// Typed effect definition.
    /// Must generate and accepts specific derived types of EffectData and EffectParameterDefinition.
    /// </summary>
    /// <typeparam name="T">
    ///     The type parameter is only for defining the behavior of ingredient effects not the actual effect.
    /// </typeparam>
    public abstract class TypedEffectDefinition<T> : EffectDefinition where T : EffectData
    {
        /// <summary>
        /// Aggregates all effect instances defined by the list of parameters to generate the data. 
        /// </summary>
        protected abstract T GenerateEffectData(List<TypedEffectParameter<T>> typedParams);
        protected abstract string GetIngredientEffectString(TypedEffectParameter<T> typedParam);

        public sealed override EffectData GenerateEffectData(List<EffectParameterDefinition> parameters)
        {
            if (parameters.Any(p => p is not TypedEffectParameter<T>))
            {
                throw new ArgumentException(
                    $"Typed effect for {typeof(T)} can only take parameters associated the same type.");
            }

            var data = GenerateEffectData(parameters.Select(p => p as TypedEffectParameter<T>).ToList());
            data.SetDefinition(this);
            return data;
        }

        public sealed override string GetIngredientEffectString(EffectParameterDefinition parameter)
        {
            if (parameter is not TypedEffectParameter<T> typedParam)
            {
                throw new ArgumentException(
                    $"Typed effect for {typeof(T)} can only take parameters associated the same type.");
            }
            
            return GetIngredientEffectString(typedParam);
        }
        
#if UNITY_EDITOR
        // ----- Odin UI for creating a new parameter asset (not a sub-asset) -----

        // 1) Dropdown: all concrete parameter types that match this definition's T
        private IEnumerable<ValueDropdownItem<Type>> _ParamTypesDropdown()
        {
            // UnityEditor.TypeCache is fast and editor-only
            var types = TypeCache.GetTypesDerivedFrom<TypedEffectParameter<T>>();
            foreach (var tp in types)
            {
                if (tp.IsAbstract || tp.IsGenericTypeDefinition) continue;
                yield return new ValueDropdownItem<Type>(tp.Name, tp);
            }
        }

        [HideInTables]
        [TitleGroup("Create Parameter Asset")]
        [PropertyOrder(999)]
        [ValueDropdown(nameof(_ParamTypesDropdown)), LabelText("Parameter Type")]
        [SerializeField] private Type _newParamType;

        [HideInTables]
        [TitleGroup("Create Parameter Asset")]
        [PropertyOrder(999)]
        [Button(ButtonSizes.Medium)]
        private void CreateParameterAsset()
        {
            if (_newParamType == null)
            {
                Debug.LogWarning("Pick a parameter type first.");
                return;
            }
            
            // Create the instance in memory
            var instance = ScriptableObject.CreateInstance(_newParamType) as TypedEffectParameter<T>;
            if (instance == null)
            {
                Debug.LogError($"Failed to create instance of {_newParamType} (not a TypedEffectParameter<{typeof(T).Name}>?).");
                return;
            }

            // Suggest a default name
            var suggestedName = _newParamType.Name;
            if (!suggestedName.EndsWith(".asset", StringComparison.OrdinalIgnoreCase))
                suggestedName += ".asset";

            // Default directory: same folder as this definition asset (fallback: "Assets")
            var defPath = AssetDatabase.GetAssetPath(this);
            var dir = string.IsNullOrEmpty(defPath) ? "Assets" : Path.GetDirectoryName(defPath);
            if (string.IsNullOrEmpty(dir)) dir = "Assets";

            // Show Save File panel inside the project
            var savePath = EditorUtility.SaveFilePanelInProject(
                title: "Create Parameter Asset",
                defaultName: suggestedName,
                extension: "asset",
                message: $"Pick a location/name for a new {_newParamType.Name} asset.",
                path: dir);

            if (string.IsNullOrEmpty(savePath))
            {
                // user cancelled
                DestroyImmediate(instance);
                return;
            }

            // Ensure unique path (SaveFilePanelInProject already handles conflicts, but this is safe)
            savePath = AssetDatabase.GenerateUniqueAssetPath(savePath);

            // Create asset on disk
            try
            {
                instance.name = Path.GetFileNameWithoutExtension(savePath);
                AssetDatabase.CreateAsset(instance, savePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(savePath);
                EditorGUIUtility.PingObject(instance);
                Selection.activeObject = instance;

                Debug.Log($"Created parameter asset: {savePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create asset at '{savePath}': {e}");
                // cleanup the in-memory object if creation failed
                DestroyImmediate(instance);
            }
        }
#endif
    }

    /// <summary>
    /// Typed effect parameter.
    /// Associated with specific TypedEffectDefinition.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TypedEffectParameter<T> : EffectParameterDefinition where T : EffectData
    {
        
    }

    public abstract class TypedEffectData<T> : EffectData where T : EffectData
    {
        internal override void SetDefinition(EffectDefinition definition)
        {
            if (definition is not TypedEffectDefinition<T>)
            {
                throw new ArgumentException(
                    $"TypedEffectData for {typeof(T)} can only bind to TypedEffectDefinition of the same type.");
            }
            
            base.SetDefinition(definition);
        }
    }
}