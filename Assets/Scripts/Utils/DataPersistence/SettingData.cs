using System;
using UnityEngine;

namespace TDB.Utils.DataPersistence
{
    [Serializable]
    public class SettingData
    {
        [SerializeField] public float MasterVolume;
        [SerializeField] public float SFXVolume;
        [SerializeField] public float BGMVolume;
        [SerializeField] public bool FullscreenEnabled;
        [SerializeField] public int LocaleKey;

        public SettingData()
        {
        }

        public void LoadDataFromPlayerPref()
        {
            // volumes
            MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
            BGMVolume = PlayerPrefs.GetFloat("BGMVolume", 0.8f);

            FullscreenEnabled = PlayerPrefs.GetInt("FullscreenEnabled", 1) == 1;
            LocaleKey = PlayerPrefs.GetInt("LocaleKey", 0);
        }

        public void SaveDataToPlayerPref()
        {
            // volumes
            PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
            PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
            PlayerPrefs.SetFloat("BGMVolume", BGMVolume);

            PlayerPrefs.SetInt("FullscreenEnabled", FullscreenEnabled ? 1 : 0);
            PlayerPrefs.SetInt("LocaleKey", LocaleKey);

            PlayerPrefs.Save();
        }
    }
}