using TDB.ShopSystem.UI;
using UnityEngine;

namespace TDB.ShopSystem.Framework
{
    public class PreviewableShopUI<T> : ShopUI<T>
        where T : ScriptableObject, IShopItemDefinition, IPreviewableShopItemDefinition
    {
        [SerializeField] private PreviewPanel _previewPanel;

        public PreviewPanel PreviewPanel => _previewPanel;
    }
    
    public interface IPreviewableShopItemDefinition
    {
        public Sprite PreviewImage { get; }
    }
}