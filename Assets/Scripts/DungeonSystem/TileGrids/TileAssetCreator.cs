using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace TDB
{
    public class TileAssetCreator
    {
        [MenuItem("Assets/Create/Tile Asset")]
        public static void CreateTile()
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            string path = AssetDatabase.GenerateUniqueAssetPath("Assets/NewTile.asset");
            AssetDatabase.CreateAsset(tile, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = tile;
        }
    }

}
