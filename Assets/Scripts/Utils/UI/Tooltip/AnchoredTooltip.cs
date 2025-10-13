// using System;
// using TMPro;
// using UnityEngine;
//
// namespace STLT.UI.Tooltip
// {
//     public class AnchoredTooltip : MonoBehaviour
//     {
//         [SerializeField]
//         private EventChannel _displayTooltipEvent;
//         private TextMeshProUGUI _text;
//         
//         private void Awake()
//         {
//             _text = GetComponentInChildren<TextMeshProUGUI>();
//             
//             // _displayTooltipEvent = Resources.Load<EventChannel>("Events/UnitInspector/DisplayTooltipEvent");
//             _displayTooltipEvent.AddListener<TooltipData>(HandleDisplayTooltip);
//             
//             gameObject.SetActive(false);
//         }
//
//         private void OnDestroy()
//         {
//             _displayTooltipEvent.RemoveListener<TooltipData>(HandleDisplayTooltip);
//         }
//
//         private void HandleDisplayTooltip(TooltipData data)
//         {
//             if (data is not { TooltipType: TooltipType.Anchored })
//             {
//                 gameObject.SetActive(false);
//                 return;
//             }
//             
//             gameObject.SetActive(true);
//             _text.text = data.TooltipText;
//         }
//     }
// }