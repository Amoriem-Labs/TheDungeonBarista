using TDB.GameManagers;
using TDB.Utils.Misc;
using UnityEngine;

namespace TDB.DungeonSystem.Collectibles
{
    [CreateAssetMenu(menuName = "Data/Collectibles", fileName = "CollectibleDefinition", order = 0)]
    public class CollectibleDefinition : ResourceScriptableObject
    {
        [SerializeField] public ICollectibleSource Definition;
        public string ItemName => Definition.ItemName;

        public void Transfer(GameData gameData, int amount) => Definition.Transfer(gameData, amount);
    }
}