using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.Utils.UI
{
    public class FlexibleGridLayout : LayoutGroup
    {
        public enum GridLayoutType
        {
            Uniform,
            Height,
            Width,
            FixedRow,
            FixedColumn,
            FixedRowColumn
        }

        [SerializeField] private Vector2 _spacing;
        [SerializeField] private GridLayoutType _gridLayoutType = GridLayoutType.Uniform;

        [SerializeField, EnableIf(nameof(_gridLayoutType), GridLayoutType.FixedRow),
         EnableIf(nameof(_gridLayoutType), GridLayoutType.FixedRowColumn)]
        private int _rows;
        [SerializeField, EnableIf(nameof(_gridLayoutType), GridLayoutType.FixedColumn),
         EnableIf(nameof(_gridLayoutType), GridLayoutType.FixedRowColumn)]
        private int _columns;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            if (_gridLayoutType != GridLayoutType.FixedColumn &&
                _gridLayoutType != GridLayoutType.FixedRow &&
                _gridLayoutType != GridLayoutType.FixedRowColumn)
            {
                float sqrt = Mathf.Sqrt(rectChildren.Count);
                _rows = Mathf.CeilToInt(sqrt);
                _columns = Mathf.CeilToInt(sqrt);
            }

            if (_gridLayoutType == GridLayoutType.Height || _gridLayoutType == GridLayoutType.FixedRow)
            {
                _columns = Mathf.CeilToInt(rectChildren.Count / (float)_rows);
            }

            if (_gridLayoutType == GridLayoutType.Width || _gridLayoutType == GridLayoutType.FixedColumn)
            {
                _rows = Mathf.CeilToInt(rectChildren.Count / (float)_columns);
            }

            float parentWidth = rectTransform.rect.width - padding.horizontal - (_spacing.x * (_columns - 1));
            float parentHeight = rectTransform.rect.height - padding.vertical - (_spacing.y * (_rows - 1));


            float defaultCellWidth = parentWidth / _columns;
            float defaultCellHeight = parentHeight / _rows;
            
            // compute preferred width/height for each column/row
            List<float> columnYPos = Enumerable.Repeat((float)padding.top, _columns).ToList();
            List<float> rowXPos = Enumerable.Repeat((float)padding.left, _rows).ToList();
            
            
            // for (int i = 0; i < rectChildren.Count; i++)
            // {
            //     int rowIdx = i / _columns;
            //     int colIdx = i % _columns;
            //     
            //     var item = rectChildren[i];
            //     var layoutElement = item.GetComponent<LayoutElement>();
            //
            //     
            //     if (layoutElement != null)
            //     {
            //         if (layoutElement.preferredWidth > 0) cellWidth = layoutElement.preferredWidth;
            //         if (layoutElement.minWidth > 0) cellWidth = Mathf.Max(cellWidth, layoutElement.minWidth);
            //
            //         if (layoutElement.preferredHeight > 0) cellHeight = layoutElement.preferredHeight;
            //         if (layoutElement.minHeight > 0) cellHeight = Mathf.Max(cellHeight, layoutElement.minHeight);
            //     }
            // }
            
            

            for (int i = 0; i < rectChildren.Count; i++)
            {
                int rowIdx = i / _columns;
                int colIdx = i % _columns;

                var item = rectChildren[i];
                var layoutElement = item.GetComponent<LayoutElement>();

                float cellWidth = defaultCellWidth;
                float cellHeight = defaultCellHeight;

                if (layoutElement != null)
                {
                    if (layoutElement.preferredWidth > 0) cellWidth = layoutElement.preferredWidth;
                    if (layoutElement.minWidth > 0) cellWidth = Mathf.Max(cellWidth, layoutElement.minWidth);

                    if (layoutElement.preferredHeight > 0) cellHeight = layoutElement.preferredHeight;
                    if (layoutElement.minHeight > 0) cellHeight = Mathf.Max(cellHeight, layoutElement.minHeight);
                }
                
                float xPos = rowXPos[rowIdx];
                float yPos = columnYPos[colIdx];

                SetChildAlongAxis(item, 0, xPos, cellWidth);
                SetChildAlongAxis(item, 1, yPos, cellHeight);
                
                rowXPos[rowIdx] += cellWidth + _spacing.x;
                columnYPos[colIdx] += cellHeight + _spacing.y;
            }
        }

        public override void CalculateLayoutInputVertical()
        {
            // Tell Unity the preferred size so ContentSizeFitter can adjust
            
        }
        
        public override void SetLayoutHorizontal() { }
        public override void SetLayoutVertical() { }
    }
}
