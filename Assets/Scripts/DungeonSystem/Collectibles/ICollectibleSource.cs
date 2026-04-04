using TDB.GameManagers;
using UnityEngine;

namespace TDB.DungeonSystem.Collectibles
{
    public interface ICollectibleSource
    {
        public Sprite CollectibleSprite { get; }
        string ItemName { get; }
        void Transfer(GameData gameData, int amount);
    }
}