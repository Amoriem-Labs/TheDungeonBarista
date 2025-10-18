using System;
using Sirenix.OdinInspector;
using TDB.CraftSystem.Data;
using TDB.GameManagers;
using TDB.Utils.ObjectPools;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TDB.CraftSystem.UI.RecipeGraph
{
    public class AddedIngredientIconController : MonoBehaviour, IPooledObject<AddedIngredientIconController>
    {
        [SerializeField] private Image _ingredientIcon;
        
        private IngredientNodeUI _nodeUI;
        private IngredientDefinition _ingredient;
        
        private Vector3 _targetPosition;
        private float _animTime;
        private AddedIngredientAnimParam _animParam;
        private Vector3 _velocity;
        private float _angularSpeed;
        private float _hoverRadius;
        private MonoObjectPool<AddedIngredientIconController> _pool;

        private void Awake()
        {
            _animParam = GameManager.Instance.GameConfig.AddedIngredientAnimParam;
        }

        public void BindData(IngredientNodeUI nodeUI, IngredientDefinition ingredient)
        {
            _nodeUI = nodeUI;
            _ingredient = ingredient;

            _ingredientIcon.sprite = _ingredient.IngredientSprite;
            
            _angularSpeed = Random.Range(_animParam.AngularSpeedRange.x, _animParam.AngularSpeedRange.y);
            _hoverRadius = Random.Range(_animParam.HoverRadiusRange.x, _animParam.HoverRadiusRange.y);
            _animTime = Random.Range(0, 360 / _angularSpeed);
        }

        private void Update()
        {
            UpdateIconFloating();
        }

        private void UpdateIconFloating()
        {
            if (!_nodeUI) return;

            _animTime += Time.deltaTime;
            
            var angle = _angularSpeed * _animTime * Mathf.Deg2Rad;
            var dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
            _targetPosition = _nodeUI.transform.position + dir * _hoverRadius;
            
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
    }

    [System.Serializable]
    public class AddedIngredientAnimParam
    {
        [SerializeField, MinMaxSlider(10, 360, true)] public Vector2 AngularSpeedRange = new Vector2(50, 80);
        [SerializeField, MinMaxSlider(0, 100, true)] public Vector2 HoverRadiusRange = new Vector2(10, 15);
        [SerializeField] public float SmoothTime = 0.1f;
    }
}