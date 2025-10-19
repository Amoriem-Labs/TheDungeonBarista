using TDB.CraftSystem.EffectSystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace TDB.CraftSystem.EffectSystem.UI
{
    public class EffectPairItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _effectPairText;
        
        public void BindEffectParamPair(EffectParamPair effect)
        {
            _effectPairText.text = effect.ToString();
        }
    }
}