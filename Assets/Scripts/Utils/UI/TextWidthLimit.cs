using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.Utils.UI
{
    [RequireComponent(typeof(TextMeshProUGUI), typeof(LayoutElement))]
    public class TextWidthLimit : MonoBehaviour
    {
        [SerializeField] private float _maxWidth = 100;
        
        private TextMeshProUGUI _text;
        private LayoutElement _layout;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _layout = GetComponent<LayoutElement>();
        }

        private void Update()
        {
            var targetWidth = Mathf.Min(_text.preferredWidth, _maxWidth);
            if (!Mathf.Approximately(_layout.preferredWidth, targetWidth)) _layout.preferredWidth = targetWidth;
        }
    }
}
