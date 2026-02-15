using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements.Experimental;

namespace TDB
{
    public class Show_Scroll_Wheel : MonoBehaviour
    {
        public Slider slider;
        public TextMeshProUGUI SliderTextComponent;
        public void WhenChanged()
        {
            int ScrollValue;
            ScrollValue = (int)(slider.value*100);
            SliderTextComponent.text = ScrollValue.ToString();
        }
    }
}
