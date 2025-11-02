using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB.TileSystem
{
    public static class TileExtensions
        {
            public static bool IsSolid(this TileType type)
            {
                var def = TileDatabase.GetDefinition(type);
                if (def == null) return false;
                return def.isSolid;
            }
        }

    }

