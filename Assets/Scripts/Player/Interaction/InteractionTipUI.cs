using System;
using TMPro;
using UnityEngine;

namespace TDB.Player.Interaction
{
    public class InteractionTipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _tipText;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void Display(string text)
        {
            _tipText.text = text;
            
            if (gameObject.activeSelf) return;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (!gameObject.activeSelf) return;
            gameObject.SetActive(false);
        }
    }
}