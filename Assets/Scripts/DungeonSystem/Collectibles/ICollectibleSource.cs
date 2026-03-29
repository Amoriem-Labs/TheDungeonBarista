using UnityEngine;

namespace TDB.DungeonSystem.Collectibles
{
    public interface ICollectibleSource
    {
        public Sprite CollectibleSprite { get; }
    }
}