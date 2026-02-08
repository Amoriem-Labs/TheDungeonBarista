using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TDB.CafeSystem.Customers;
using TDB.CafeSystem.UI.OrderUI;
using TDB.CraftSystem.Data;
using TDB.CraftSystem.EffectSystem.Data;
using TDB.CraftSystem.EffectSystem.LevelUpEffect;
using TDB.GameManagers;
using TDB.Utils.Misc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.CafeSystem.UI.ProductUI
{
    public class ProductItemUI : DynamicItemUI<FinalRecipeData>
    {
        [Title("References")]
        [SerializeField] private EffectDefinition _qualityEffect;
        
        [SerializeField] private LayoutElement _anchorLayout;
        
        [SerializeField] private TextMeshProUGUI _productNameText;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private TextMeshProUGUI _qualityText;
        [SerializeField] private List<AttributeItemUI> _flavorTexts;
        [SerializeField] private TextMeshProUGUI _priceBonusText;
        
        [SerializeField] private Vector2 _extraSizeForRecipe = new Vector2(10, 20);
        [SerializeField] private string _starRichText;
        
        [SerializeField] private DOTweenVisualManager _bonusAnim;
        [SerializeField] private DOTweenVisualManager _punishAnim;
        
        private FinalRecipeData _data;
        private List<FlavorDefinition> _allFlavors;
        private Canvas _canvas;
        private ImageOutlineController _outline;
        private CanvasGroup _canvasGroup;

        public override RectTransform Anchor => _anchorLayout.transform as RectTransform;

        public ImageOutlineController Outline => _outline;

        private void Awake()
        {
            _allFlavors = GameManager.Instance.GameConfig.AllFlavors;

            _canvas = GetComponentInParent<Canvas>();
            _outline = GetComponentInChildren<ImageOutlineController>();

            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _anchorLayout.ignoreLayout = false;
            _priceBonusText.color = Color.clear;
            _canvasGroup.alpha = 1;
            
            _bonusAnim.gameObject.SetActive(false);
            _punishAnim.gameObject.SetActive(false);
        }

        public void SetStartPosition()
        {
            IEnumerator SetStartPositionCoroutine()
            {
                yield return null;
                transform.position = _anchorLayout.transform.position +
                                     new Vector3(
                                         (transform as RectTransform)!.sizeDelta.x * _canvas.transform.localScale.x, 0);
                Velocity = Vector3.zero;
            }
            
            StartCoroutine(SetStartPositionCoroutine());
        }

        private void OnDisable()
        {
            if (_data != null)
            {
                _data = null;
            }
        }

        public override void BindData(FinalRecipeData data)
        {
            _data = data;
            
            UpdateUI();
        }

        private void UpdateUI()
        {
            // set name
            _productNameText.text = _data?.RecipeName ?? RecipeNotConfigured;

            // hide everything else if no recipe
            var hasRecipe = _data != null;
            _priceText.gameObject.SetActive(hasRecipe);
            _qualityText.gameObject.SetActive(hasRecipe);
            foreach (var flavor in _flavorTexts)
            {
                flavor.gameObject.SetActive(hasRecipe);
            }

            if (!hasRecipe)
            {
                StartCoroutine(UpdateAnchor());
                return;
            }
            
            // set product specific text
            if (_data is ProductData productData)
            {
                _priceText.text = string.Format(ProductPriceTemplate, productData.Price);
                _qualityText.gameObject.SetActive(false);
            }
            // set recipe specific text
            else
            {
                _priceText.text = string.Format(BasicPriceTemplate, _data.GetBasicPrice());
                _qualityText.gameObject.SetActive(true);
                var quality = _data.GetQualityLevel(_qualityEffect);
                _qualityText.text = $"{_qualityEffect.EffectName} {GetStars(quality)}";
            }
            
            // set flavors
            var effects = _data.GetAllEffectData()
                .ToDictionary(e => e.Definition, e => e);
            for (int i = 0; i < _flavorTexts.Count; i++)
            {
                var flavorText = _flavorTexts[i];
                if (i >= _allFlavors.Count)
                {
                    flavorText.gameObject.SetActive(false);
                    continue;
                }
                var flavor = _allFlavors[i];
                if (!effects.TryGetValue(flavor.EffectDefinition, out var effect)
                    || effect is not LevelUpEffectData levelData)
                {
                    flavorText.gameObject.SetActive(false);
                    continue;
                }
                
                flavorText.gameObject.SetActive(true);
                flavorText.SetText($"{flavor.Name} {GetStars(levelData.Level)}");
            }
            
            StartCoroutine(UpdateAnchor());
        }

        public List<FlavorDefinition> GetActiveFlavors() =>
            _allFlavors.Where((_, i) => i < _flavorTexts.Count && _flavorTexts[i].gameObject.activeSelf).ToList();

        private IEnumerator UpdateAnchor()
        {
            // wait one frame for layout group to update
            yield return null;
            var isRecipe = _data is not ProductData;
            var rect = transform as RectTransform;
            _anchorLayout.preferredWidth = rect!.sizeDelta.x + (isRecipe ? _extraSizeForRecipe.x : 0);
            _anchorLayout.preferredHeight = rect!.sizeDelta.y + (isRecipe ? _extraSizeForRecipe.y : 0);
        }

        private string GetStars(int number) =>
            number <= 5
                ? string.Join("", Enumerable.Repeat(_starRichText, number))
                : $"{_starRichText} x {number}";

        private static string ProductPriceTemplate => "Price: ${0}";
        private static string BasicPriceTemplate => "Basic Price: ${0}";
        private static string RecipeNotConfigured => "Recipe Not Configured";

        public override void DestroyItem()
        {
            _anchorLayout.transform.SetParent(transform);
            base.DestroyItem();
        }

        public void DestroyItemInProductList()
        {
            StartCoroutine(OutroCoroutine());
        }

        public void FadeOut(float duration, Vector2 fadeOffset)
        {
            _anchorLayout.ignoreLayout = true;
            _anchorLayout.transform.SetParent(transform);
            _canvasGroup.DOFade(0, duration);
            transform.DOMove(fadeOffset, duration)
                .SetRelative(true);
        }
        
        private IEnumerator OutroCoroutine()
        {
            _anchorLayout.ignoreLayout = true;
            _anchorLayout.transform.position +=
                new Vector3(_anchorLayout.preferredWidth * _canvas.transform.localScale.x * 2, 0);
            yield return new WaitUntil(() => (transform.position - _anchorLayout.transform.position).sqrMagnitude < .1f);
            DestroyItem();
        }

        #region Calculate Income Animations

        public void AnchorToOrderItem(Transform parent, float offset)
        {
            _anchorLayout.transform.SetParent(parent);
            _anchorLayout.transform.position = parent.position +
                                               new Vector3(offset + _anchorLayout.preferredWidth * _canvas.transform.localScale.x / 2, 0);
        }

        public void DisplayFlavorBonus(FlavorDefinition flavor, string text, Color color, float animTime)
        {
            var idx = _allFlavors.IndexOf(flavor);
            _flavorTexts[idx].DisplayRightText(text, color, animTime);
        }

        public void DisplayTotalBonus(string text, Color color, float animTime)
        {
            _priceBonusText.color = color;
            _priceBonusText.text = text;
            _priceBonusText.transform.DOScale(1, animTime)
                .From(0).SetEase(Ease.OutBack);
            _priceBonusText.DOFade(1, animTime).From(0);
        }

        public IEnumerator DisplayFinalPrice(int initialPrice, int finalPrice, float animTime)
        {
            var price = initialPrice;
            var anim = DOTween.To(() => price, x =>
            {
                price = x;
                _priceText.text = string.Format(ProductPriceTemplate, price);
            }, finalPrice, animTime);
            yield return anim.WaitForCompletion();
        }

        #endregion

        public void BonusAnimation(float bonus)
        {
            if (bonus > 0)
            {
                Debug.Log("play bonus");
                _bonusAnim.gameObject.SetActive(false);
                _bonusAnim.gameObject.SetActive(true);
            }

            if (bonus < 0)
            {
                Debug.Log("play punish");
                _punishAnim.gameObject.SetActive(false);
                _punishAnim.gameObject.SetActive(true);
            }
        }
    }
}