using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB.TileSystem
{
    [ExecuteAlways]
    public class GridVisualizer : MonoBehaviour
    {
        public TileManager manager;

        void OnDrawGizmos()
        {
            if (manager == null) return;
            Gizmos.color = Color.green;
            for (int x = 0; x < manager.width; x++)
            {
                for (int y = 0; y < manager.height; y++)
                {
                    Gizmos.DrawWireCube(new Vector3(x, y, 0), Vector3.one);
                }
            }

            // Offset grid
            Gizmos.color = Color.cyan;
            for (int x = 0; x < manager.width; x++)
            {
                for (int y = 0; y < manager.height; y++)
                {
                    Gizmos.DrawWireCube(new Vector3(x + 0.5f, y + 0.5f, 0), Vector3.one * 0.8f);
                }
            }
        }
    }

}
