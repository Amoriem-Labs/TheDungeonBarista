using UnityEngine;
using UnityEngine.UI;

namespace TDB.Utils.UI
{
    public class ExitButton : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_WEBGL
            gameObject.SetActive(false);
#endif
            var button = GetComponent<Button>();
            button.onClick.AddListener(() => { Application.Quit(); });
        }
    }
}
