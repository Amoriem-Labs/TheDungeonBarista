using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace TDB.Utils.Misc
{
    [ExecuteAlways]
    public class YSortByPosition : MonoBehaviour
    {
        [SerializeField] private bool onlyAffectTilemap = true;
        [SerializeField] private int orderOffset = 0;
        [SerializeField] private int orderMultiplier = 100;
        [SerializeField] private bool useRendererBounds = true;

        private TilemapRenderer _tilemapRenderer;
        private SortingGroup _sortingGroup;
        private SpriteRenderer[] _renderers;

        private void Awake()
        {
            Cache();
            ApplySorting();
        }

        private void OnEnable()
        {
            Cache();
            ApplySorting();
        }

        private void LateUpdate()
        {
            ApplySorting();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Cache();
            ApplySorting();
        }
#endif

        private void Cache()
        {
            _tilemapRenderer = GetComponent<TilemapRenderer>();
            _sortingGroup = GetComponent<SortingGroup>();
            _renderers = GetComponentsInChildren<SpriteRenderer>();
        }

        public void SetOnlyAffectTilemap(bool value)
        {
            onlyAffectTilemap = value;
            ApplySorting();
        }

        private void ApplySorting()
        {
            if (_tilemapRenderer != null)
            {
                _tilemapRenderer.mode = TilemapRenderer.Mode.Individual;
                _tilemapRenderer.sortOrder = TilemapRenderer.SortOrder.TopLeft;
                return;
            }

            if (onlyAffectTilemap) return;

            float y = transform.position.y;
            if (useRendererBounds && _renderers != null && _renderers.Length > 0)
            {
                float minY = _renderers[0].bounds.min.y;
                for (int i = 1; i < _renderers.Length; i++)
                {
                    float candidate = _renderers[i].bounds.min.y;
                    if (candidate < minY)
                        minY = candidate;
                }
                y = minY;
            }

            int order = orderOffset + Mathf.RoundToInt(-y * orderMultiplier);

            if (_sortingGroup != null)
            {
                _sortingGroup.sortingOrder = order;
                return;
            }

            if (_renderers == null) return;
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].sortingOrder = order;
            }
        }
    }
}
