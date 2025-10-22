using UnityEngine;

namespace TDB.Utils.UI.Tooltip
{
    [System.Serializable]
    public class TooltipData
    {
        [TextArea] public string TooltipText;
        public TooltipType TooltipType;
        public Transform TriggerTransform;
    }

    public enum TooltipType
    {
        Floating
    }
}