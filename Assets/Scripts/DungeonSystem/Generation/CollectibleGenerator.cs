using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDB
{
    public class CollectibleGenerator : MonoBehaviour
    {
        public GameObject[] collectiblePrefabs;
        public int collectibleCount = 5;

        private HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

        public void SpawnCollectibles(HashSet<Vector2Int> floorPositions)
        {
            List<Vector2Int> floorList = new List<Vector2Int>(floorPositions);

            for (int i = 0; i < collectibleCount; i++)
            {
                Vector2Int origin = floorList[Random.Range(0, floorList.Count)];
                GameObject prefab = collectiblePrefabs[Random.Range(0, collectiblePrefabs.Length)];

                Collectible collectible = prefab.GetComponent<Collectible>();
                Vector2Int size = collectible.size;

                if (CanPlace(origin, size, floorPositions))
                {
                    Place(prefab, origin, size);
                }
            }
    }

    bool CanPlace(Vector2Int origin, Vector2Int size, HashSet<Vector2Int> floor)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int p = origin + new Vector2Int(x, y);

                if (!floor.Contains(p) || occupied.Contains(p))
                {
                    return false;
                }
            }
        }
        return true;
    }

    void Place(GameObject prefab, Vector2Int origin, Vector2Int size)
    {
        GameObject obj = Instantiate(prefab, new Vector3(origin.x, origin.y, 0), Quaternion.identity);

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                occupied.Add(origin + new Vector2Int(x, y));
            }
        }
    }
}
}
