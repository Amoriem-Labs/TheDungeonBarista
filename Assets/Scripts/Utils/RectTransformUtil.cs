using UnityEngine;

namespace TDB.Utils
{
    public static class RectTransformUtil
    {
        public static Vector3 GetAnchorMinWorldPosition(RectTransform rt)
        {
            return GetAnchorWorldPosition(rt, rt.anchorMin);
        }

        public static Vector3 GetAnchorMaxWorldPosition(RectTransform rt)
        {
            return GetAnchorWorldPosition(rt, rt.anchorMax);
        }

        private static Vector3 GetAnchorWorldPosition(RectTransform rt, Vector2 anchor)
        {
            RectTransform parent = rt.parent as RectTransform;
            if (parent == null)
                return rt.position;

            Rect parentRect = parent.rect;

            // Anchor in parent local space (pivot-relative)
            Vector2 localPos = new Vector2(
                (anchor.x - parent.pivot.x) * parentRect.width,
                (anchor.y - parent.pivot.y) * parentRect.height
            );

            return parent.TransformPoint(localPos);
        }
    }
}