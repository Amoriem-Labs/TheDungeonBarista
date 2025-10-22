using UnityEngine;

namespace TDB.Utils.Testing
{
    public class WhoDisabledMe : MonoBehaviour
    {
        private void OnDisable()
        {
            Debug.Log("Who disabled me?");
        }
    }
}