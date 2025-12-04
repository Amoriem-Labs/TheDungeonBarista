using System;
using DG.Tweening;
using TDB.Utils.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.ShopSystem.UI
{
    [RequireComponent(typeof(Image))]
    public class ItemPurchasableMask : MonoBehaviour
    {
        private ShopItemUIBase _itemUI;
        private Image _image;
        private float _disabledAlpha;

        private void Awake()
        {
            _itemUI = GetComponentInParent<ShopItemUIBase>();
            _itemUI.OnPurchasableUpdate += HandlePurchasableUpdate;

            _image = GetComponent<Image>();
            _disabledAlpha = _image.color.a;
            _image.color = _image.color.SetAlpha(0);
        }

        private void HandlePurchasableUpdate(bool purchasable)
        {
            _image.DOKill();
            _image.DOFade(purchasable ? 0 : _disabledAlpha, 0.2f);
        }
    }
}