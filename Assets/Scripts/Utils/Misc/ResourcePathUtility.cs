using UnityEditor;
using UnityEngine;

namespace TDB.Utils.Misc
{
    public static class ResourcePathUtility
    {
#if UNITY_EDITOR
        public static string GetResourcesPath(Object asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            string resourcesKeyword = "/Resources/";
            int index = assetPath.IndexOf(resourcesKeyword);
            if (index == -1)
            {
                Debug.LogError("Asset is not inside a Resources folder.");
                return null;
            }

            // Trim everything before and including "/Resources/"
            string path = assetPath.Substring(index + resourcesKeyword.Length);
            // Remove file extension
            path = System.IO.Path.ChangeExtension(path, null);
            return path;
        }
#endif
    }
}