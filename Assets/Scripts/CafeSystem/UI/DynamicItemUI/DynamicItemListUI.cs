using UnityEngine;

namespace TDB.CafeSystem.UI.OrderUI
{
    public class DynamicItemListUI<T> : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private Transform _headerSpace;
        [SerializeField] private Transform _footerSpace;
        [SerializeField] private DynamicItemUIPool<T> _orderItemPool;

        protected DynamicItemUI<T> AddItem(Vector3 position, T data)
        {
            var itemUI = _orderItemPool.Get(position, Quaternion.identity);
            itemUI.transform.SetAsLastSibling();
            itemUI.Anchor.SetParent(_content);
            // UI animation may affect local scale, reset it here
            itemUI.Anchor.localScale = Vector3.one;
            _headerSpace.SetAsFirstSibling();
            _footerSpace.SetAsLastSibling();
            itemUI.BindData(data);
            return itemUI;
        }
    }
}