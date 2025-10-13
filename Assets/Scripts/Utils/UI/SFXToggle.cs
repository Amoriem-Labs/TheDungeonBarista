using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace TDB.Utils.UI
{
    public class SFXToggle : MonoBehaviour
    {
        private const string MusicVolumeParam = "SFXVolume";
        
        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Sprite _offSprite;
        [SerializeField] private AudioMixer _mixer;
        private float _defaultVolume;
        private Image _image;

        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(HandleToggle);
            _mixer.GetFloat(MusicVolumeParam, out _defaultVolume);
            
            _image = GetComponent<Image>();
        }

        private void HandleToggle()
        {
            _mixer.GetFloat(MusicVolumeParam, out var currentVolume);
            if (currentVolume <= -79f)
                UnmuteMusic();
            else
                MuteMusic();
        }

        // Mute the group by setting it to minimum (typically -80 dB)
        public void MuteMusic()
        {
            _mixer.SetFloat(MusicVolumeParam, -80f);
            _image.sprite = _offSprite;
        }

        // Unmute by setting back to 0 dB (full volume)
        public void UnmuteMusic()
        {
            _mixer.SetFloat(MusicVolumeParam, _defaultVolume);
            _image.sprite = _onSprite;
        }
    }
}