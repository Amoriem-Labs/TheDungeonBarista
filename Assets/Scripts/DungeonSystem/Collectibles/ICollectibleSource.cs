using TDB.GameManagers;
using UnityEngine;

namespace TDB.DungeonSystem.Collectibles
{
    public interface ICollectibleSource
    {
        public Sprite CollectibleSprite { get; }
        void Transfer(GameData gameData, int amount);
    }
}