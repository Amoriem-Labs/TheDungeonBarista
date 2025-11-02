using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


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
            // inside GridVisualizer.OnDrawGizmos when manager != null
            for (int x = 0; x < manager.width; x++)
                for (int y = 0; y < manager.height; y++)
                {
                    Gizmos.DrawWireCube(new Vector3(x + 0.0f, y + 0.0f, 0), Vector3.one * 0.95f);

                    //// draw cyan visual tile center
                    //Vector3 center = new Vector3(x + 0.5f, y + 0.5f, 0);
                    //// Compute mask using the same function used in autotiler (or call a manager helper)
                    //int mask = manager.GetCornerMaskAt(x, y); // implement a small helper that uses same logic
                    //Handles.Label(center, mask.ToString()); // requires UnityEditor; wrap with #if UNITY_EDITOR
                }


            // Offset grid
            Gizmos.color = Color.cyan;
            for (int x = 0; x < manager.width; x++)
            {
                for (int y = 0; y < manager.height; y++)
                {
                    Vector3 center = new Vector3(x + 0.5f, y + 0.5f, 0);
                    Gizmos.DrawWireCube(center, Vector3.one * 0.8f);

                    //Gizmos.DrawWireCube(new Vector3(x + 0.5f, y + 0.5f, 0), Vector3.one * 0.8f);
#if UNITY_EDITOR
                    if (manager != null)
                    {
                        int mask = manager.GetCornerMaskAt(x, y);
                        Handles.color = Color.white;
                        Handles.Label(center, mask.ToString());
                    }
#endif
                }
            }
        }
    }

}
