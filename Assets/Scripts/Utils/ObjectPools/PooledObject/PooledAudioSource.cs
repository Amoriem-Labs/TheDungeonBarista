using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace TDB.Utils.ObjectPools.PooledObject
{
    [RequireComponent(typeof(AudioSource))]
    public class PooledAudioSource: MonoBehaviour, IPooledObject<PooledAudioSource>
    {
        private MonoObjectPool<PooledAudioSource> _pool;
        public AudioSource Source { get; private set; }

        [field: SerializeField]
        public PooledObjectTag PooledObjectTag { get; private set; }

        // Start is called before the first frame update
        void Awake()
        {
            Source = GetComponent<AudioSource>();
        }

        public void SetPool(MonoObjectPool<PooledAudioSource> pool)
        {
            _pool = pool;
        }

        public void PlayClip(AudioClip clip, float volume, float pitch, AudioMixerGroup group)
        {
            Source.clip = clip;
            Source.volume = volume;
            Source.pitch = pitch;
            Source.outputAudioMixerGroup = group;
            Source.loop = false;

            Source.Play();

            float delay = Source.clip.length;
            StartCoroutine(OnAudioClipFinished(delay));
        }

        public IEnumerator OnAudioClipFinished(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);

            if (_pool != null)
            {
                _pool.Release(this);
            }
            else
            {
                Destroy(this);
            }
        }
    }
}

