using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace TDB
{
    public class VolumeChange : MonoBehaviour
    {

        public Slider slider;
        // Start is called before the first frame update
        void Start()
        {
            slider.value = AudioListener.volume;
        }

        // Update is called once per frame
        void Update()
        {
            AudioListener.volume = slider.value; 
        }
    }
}
