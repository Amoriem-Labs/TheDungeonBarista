using TDB.ShopSystem.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.ShopSystem.UI
{
    [RequireComponent(typeof(Image))]
    public class PreviewItem : MonoBehaviour
    {
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void BindItem(IPreviewableShopItemDefinition itemDefinition)
        {
            _image.sprite = itemDefinition.PreviewImage;
        }
    }
}