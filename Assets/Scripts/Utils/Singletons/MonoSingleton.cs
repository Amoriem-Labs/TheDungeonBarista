using UnityEngine;

namespace TDB.Utils.Singletons
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                /*
                if (_instance == null)
                    Debug.LogWarning(typeof(T).ToString() + " is NULL.");
                */
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance)
            {
                if (_instance != this)
                {
                    Debug.LogWarning("Multiple mono-singleton instantiated.");
                    Destroy(this.gameObject);
                }
                return;
            }
            else
            {
                _instance = this as T;
            }
            Initialize();
        }

        public abstract void Initialize();
    }
}