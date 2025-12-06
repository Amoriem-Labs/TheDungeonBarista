using UnityEngine;
using UnityEngine.UI;

namespace TDB.Utils.UI
{
    [AddComponentMenu("Layout/Horizontal Wrap Layout Group")]
    public class HorizontalWrapLayoutGroup : LayoutGroup
    {
        public int maxItemsPerRow = 3;
        public float spacing = 0f;
        public float rowSpacing = 0f;
        public bool controlChildWidth = true;
        public bool controlChildHeight = true;

        private void GetCellAndGrid(out float cellW, out float cellH, out int rows, out int cols, out int count)
        {
            count = rectChildren.Count;
            cols = Mathf.Max(1, maxItemsPerRow);
            rows = count == 0 ? 0 : Mathf.CeilToInt((float)count / cols);

            float maxW = 0f, maxH = 0f;
            for (int i = 0; i < rectChildren.Count; i++)
            {
                var child = rectChildren[i];
                if (!child || !child.gameObject.activeInHierarchy) continue;

                float w = controlChildWidth  ? LayoutUtility.GetPreferredSize(child, 0) : child.sizeDelta.x;
                float h = controlChildHeight ? LayoutUtility.GetPreferredSize(child, 1) : child.sizeDelta.y;
                if (w > maxW) maxW = w;
                if (h > maxH) maxH = h;
            }

            // Your runtime layout can still scale these to fit the available rect,
            // but for PREFERRED SIZE reporting we return the natural cell size.
            cellW = maxW;
            cellH = maxH;
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal(); // populates rectChildren
            GetCellAndGrid(out float cellW, out _, out _, out int cols, out int count);

            float preferredWidth =
                padding.left + padding.right +
                (count == 0 ? 0f : (cols * cellW + (cols - 1) * spacing));

            // min == preferred is fine for wrap groups; flexible -1 means "don't stretch me"
            SetLayoutInputForAxis(preferredWidth, preferredWidth, -1, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            GetCellAndGrid(out _, out float cellH, out int rows, out _, out int count);

            float preferredHeight =
                padding.top + padding.bottom +
                (count == 0 ? 0f : (rows * cellH + (rows - 1) * rowSpacing));

            SetLayoutInputForAxis(preferredHeight, preferredHeight, -1, 1);
        }

        public override void SetLayoutHorizontal()
        {
            LayoutRows();
        }

        public override void SetLayoutVertical()
        {
            LayoutRows();
        }

        private void LayoutRows()
        {
            float totalWidth = rectTransform.rect.width;
            float totalHeight = rectTransform.rect.height;
            float usableWidth = totalWidth - padding.left - padding.right;
            float usableHeight = totalHeight - padding.top - padding.bottom;

            int itemCount = rectChildren.Count;
            int itemsPerRow = Mathf.Max(1, maxItemsPerRow);
            int rowCount = Mathf.CeilToInt((float)itemCount / itemsPerRow);

            // Step 1: Max preferred size across all children
            float preferredWidth = 0f, preferredHeight = 0f;
            foreach (var child in rectChildren)
            {
                if (!child.gameObject.activeInHierarchy) continue;
                float w = controlChildWidth ? LayoutUtility.GetPreferredSize(child, 0) : child.sizeDelta.x;
                float h = controlChildHeight ? LayoutUtility.GetPreferredSize(child, 1) : child.sizeDelta.y;
                preferredWidth = Mathf.Max(preferredWidth, w);
                preferredHeight = Mathf.Max(preferredHeight, h);
            }

            // Step 2: Fit to layout
            float totalContentWidth = preferredWidth * itemsPerRow + spacing * (itemsPerRow - 1);
            float totalContentHeight = preferredHeight * rowCount + rowSpacing * (rowCount - 1);
            float scaleX = totalContentWidth > usableWidth
                ? (usableWidth - spacing * (itemsPerRow - 1)) / (preferredWidth * itemsPerRow)
                : 1f;
            float scaleY = totalContentHeight > usableHeight
                ? (usableHeight - rowSpacing * (rowCount - 1)) / (preferredHeight * rowCount)
                : 1f;

            float cellWidth = preferredWidth * scaleX;
            float cellHeight = preferredHeight * scaleY;

            // Step 3: Vertical alignment offset
            float totalFinalHeight = cellHeight * rowCount + rowSpacing * (rowCount - 1);
            float yOffsetGroup = childAlignment switch
            {
                TextAnchor.UpperLeft or TextAnchor.UpperCenter or TextAnchor.UpperRight => 0,
                TextAnchor.MiddleLeft or TextAnchor.MiddleCenter or TextAnchor.MiddleRight => (usableHeight -
                    totalFinalHeight) / 2f,
                TextAnchor.LowerLeft or TextAnchor.LowerCenter or TextAnchor.LowerRight => (usableHeight -
                    totalFinalHeight),
                _ => 0
            };

            // Step 4: Layout
            for (int row = 0; row < rowCount; row++)
            {
                int rowStart = row * itemsPerRow;
                int rowEnd = Mathf.Min(rowStart + itemsPerRow, rectChildren.Count);
                int actualCount = rowEnd - rowStart;

                float totalRowWidth = cellWidth * actualCount + spacing * (actualCount - 1);

                float xOffsetRow = childAlignment switch
                {
                    TextAnchor.UpperCenter or TextAnchor.MiddleCenter or TextAnchor.LowerCenter => (usableWidth -
                        totalRowWidth) / 2f,
                    TextAnchor.UpperRight or TextAnchor.MiddleRight or TextAnchor.LowerRight => (usableWidth -
                        totalRowWidth),
                    _ => 0
                };

                float y = padding.top + yOffsetGroup + row * (cellHeight + rowSpacing);

                for (int i = 0; i < actualCount; i++)
                {
                    int index = rowStart + i;
                    var child = rectChildren[index];
                    float x = padding.left + xOffsetRow + i * (cellWidth + spacing);

                    SetChildAlongAxis(child, 0, x, cellWidth);
                    SetChildAlongAxis(child, 1, y, cellHeight);
                }
            }
        }






        private float GetPreferredWidth(RectTransform child) =>
            LayoutUtility.GetPreferredSize(child, 0);

        private float GetPreferredHeight(RectTransform child) =>
            LayoutUtility.GetPreferredSize(child, 1);
    }
}
