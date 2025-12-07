using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace TDB.TileGrids
{
    [CreateAssetMenu(menuName = "DualGrid/TileClass")]
    public class TileClass : ScriptableObject
    {
        public TileType tileType;

        //Rule objects 
        public List<RuleEntry> rules = new();
        private Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile> ruleDict;

        [Serializable]
        public class RuleEntry
        {
            public TileType topLeft;
            public TileType topRight;
            public TileType bottomLeft;
            public TileType bottomRight;
            public Tile resultingTile;
        }

        //
        private void OnEnable()
        {
            ruleDict = new Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile>();
            foreach (var rule in rules)
            {
                var tuple = new Tuple<TileType, TileType, TileType, TileType>(
                    rule.topLeft, rule.topRight, rule.bottomLeft, rule.bottomRight);

                ruleDict[tuple] = rule.resultingTile;
            }
        }

        // Lookup function the GridManager will call
        public Tile GetTileForNeighbours(
            TileType tl, TileType tr, TileType bl, TileType br)
        {
            Debug.Log($"Looking for tile pattern: TL={tl}, TR={tr}, BL={bl}, BR={br}");
            var key = new Tuple<TileType, TileType, TileType, TileType>(tl, tr, bl, br);

            if (ruleDict.TryGetValue(key, out Tile tile))
                return tile;

            Debug.LogWarning($"No rule for pattern {key} in {name}");
            return null;
        }
    }
}
