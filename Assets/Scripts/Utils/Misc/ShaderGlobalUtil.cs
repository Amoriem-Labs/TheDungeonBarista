using UnityEngine;

namespace TDB.Utils.Misc
{
    [ExecuteAlways]
    public class ShaderGlobalUtil : MonoBehaviour
    {
        [SerializeField] private bool _useUnscaledTime;
        [SerializeField] private bool _useCursorPosition;

        void Update()
        {
            if (_useUnscaledTime) Shader.SetGlobalFloat("_UnscaledTime", Time.unscaledTime);
            if (_useCursorPosition)
                Shader.SetGlobalVector("_CursorPosition",
                    Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition));
        }
    }
}