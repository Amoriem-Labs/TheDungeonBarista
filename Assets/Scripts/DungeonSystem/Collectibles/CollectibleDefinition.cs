using TDB.Utils.Misc;
using UnityEngine;

namespace TDB.DungeonSystem.Collectibles
{
    [CreateAssetMenu(menuName = "Data/Collectibles", fileName = "CollectibleDefinition", order = 0)]
    public class CollectibleDefinition : ResourceScriptableObject
    {
        [SerializeField] public ICollectibleSource Definition;
    }
}