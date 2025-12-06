using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.UI.Info;
using TDB.GameManagers;
using TDB.Utils.EventChannels;
using TDB.Utils.ObjectPools;
using TDB.Utils.UI.UIHover;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TDB.CraftSystem.UI.RecipeGraph
{
    public class AddedIngredientIconController : MonoBehaviour, IPooledObject<AddedIngredientIconController>,
        IPointerClickHandler, IUIHoverHandler
    {
        private const float CanRemoveTime = .5f;
            
        [SerializeField] private Image _ingredientIcon;

        [Title("Events")]
        [SerializeField] private EventChannel _displayIngredientInfoEvent;
        
        private IngredientNodeUI _nodeUI;
        private IngredientDefinition _ingredient;
        
        private Vector3 _targetPosition;
        private float _animTime;
        private AddedIngredientAnimParam _animParam;
        private Vector3 _velocity;
        private float _angularSpeed;
        private float _hoverRadius;
        private MonoObjectPool<AddedIngredientIconController> _pool;
        private bool _hovered;
        private float _canRemoveTimer;

        public IngredientDefinition Ingredient => _ingredient;
        public bool CanRemove => _canRemoveTimer > CanRemoveTime;

        private void Awake()
        {
            _animParam = GameManager.Instance.GameConfig.AddedIngredientAnimParam;
        }

        public void BindData(IngredientNodeUI nodeUI, IngredientDefinition ingredient)
        {
            _nodeUI = nodeUI;
            _ingredient = ingredient;

            _ingredientIcon.sprite = Ingredient.IngredientSprite;
            
            _angularSpeed = Random.Range(_animParam.AngularSpeedRange.x, _animParam.AngularSpeedRange.y);
            _hoverRadius = Random.Range(_animParam.HoverRadiusRange.x, _animParam.HoverRadiusRange.y);
            _animTime = Random.Range(0, 360 / _angularSpeed);
        }

        private void OnEnable()
        {
            _canRemoveTimer = 0;
            _hovered = false;
        }

        private void Update()
        {
            // need a timer to prevent remove immediately
            if (!CanRemove) _canRemoveTimer += Time.unscaledDeltaTime;
            
            UpdateIconFloating();
        }

        private void UpdateIconFloating()
        {
            if (!_nodeUI) return;

            // stop moving when hovered
            if (!_hovered)
            {
                _animTime += Time.deltaTime;
                var angle = _angularSpeed * _animTime * Mathf.Deg2Rad;
                var dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
                _targetPosition = _nodeUI.transform.position + dir * _hoverRadius;
            }
            
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, _animParam.SmoothTime);
        }

        public void DestroyObject()
        {
            if (_pool)
            {
                _pool.Release(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void SetPool(MonoObjectPool<AddedIngredientIconController> pool)
        {
            _pool = pool;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!CanRemove) return;
            _nodeUI.RemoveIngredient(this);
        }

        public void OnUIHoverEnter()
        {
            _hovered = true;
            // move to top so it is not blocked by other ingredients
            transform.SetAsLastSibling();

            if (CanRemove)
            {
                _displayIngredientInfoEvent.RaiseEvent(new DisplayIngredientInfo
                {
                    Ingredient = _ingredient,
                });
            }
        }

        public void OnUIHoverExit()
        {
            _hovered = false;
            
            if (CanRemove)
            {
                _displayIngredientInfoEvent.RaiseEvent(new DisplayIngredientInfo
                {
                    Ingredient = null,
                });
            }
        }
    }

    [System.Serializable]
    public class AddedIngredientAnimParam
    {
        [SerializeField, MinMaxSlider(10, 360, true)] public Vector2 AngularSpeedRange = new Vector2(50, 80);
        [SerializeField, MinMaxSlider(0, 100, true)] public Vector2 HoverRadiusRange = new Vector2(10, 15);
        [SerializeField] public float SmoothTime = 0.1f;
    }
}