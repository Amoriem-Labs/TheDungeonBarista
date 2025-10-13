using Sirenix.OdinInspector;
using TDB.Utils.ObjectPools;
using TDB.Utils.ObjectPools.ConcretePools;
using TDB.Utils.ObjectPools.PooledObject;
using TDB.Utils.Singletons;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace TDB.Audio
{
    public class AudioManager : PassiveSingleton<AudioManager>
    {
        [field: SerializeField, HideLabel, InlineProperty, BoxGroup("SFX List")]
        public AudioList SFXList { get; private set; }
        
        [SerializeField] private float _randomVolumeRange = 0.1f;
        [SerializeField] private float _randomPitchRange = 0.1f;

        [SerializeField] private PooledAudioSource _sfxSourcePrefab;
        private AudioSourcePool _audioSourcePool;
        
        public override void Initialize()
        {
            base.Initialize();
        }

        private void Start()
        {
            _audioSourcePool = ObjectPoolManager.Instance.GetPool<PooledAudioSource, AudioSourcePool>(_sfxSourcePrefab);
            _audioSourcePool.transform.SetParent(transform);
        }

        public void PlaySFX(AudioClip clip, float? overrideBaseVolume = null)
        {
            if (!clip) return;
            
            var volume = Random.Range(-_randomVolumeRange, _randomVolumeRange) + (overrideBaseVolume ?? 1);
            var pitch = Random.Range(-_randomPitchRange, _randomPitchRange) + 1;
            var source = _audioSourcePool.Get();
            source.PlayClip(clip, volume, pitch, source.Source.outputAudioMixerGroup);
        }
    }

    [System.Serializable]
    public struct AudioList
    {
        public AudioClip ButtonClick;
        
        public AudioClip EnemyDie;
        
        public AudioClip EnemyHit;
        
        public AudioClip LevelUp;
    }

    public static class AudioManagerExtensions
    {
        public static void PlaySFX(this AudioClip clip, float? overrideBaseVolume = null)
        {
            AudioManager.Instance.PlaySFX(clip, overrideBaseVolume);
        }
    }
}