using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB.TileSystem
{
    public static class TileExtensions
        {
            public static bool IsSolid(this TileType type)
            {
                switch (type)
                {
                    case TileType.Wall:
                    case TileType.Floor:
                        return true;
                    default:
                        return false;
                }
            }
        }

    }

