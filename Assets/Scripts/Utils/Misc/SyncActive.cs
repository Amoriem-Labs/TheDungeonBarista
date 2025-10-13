using System.Collections.Generic;
using UnityEngine;

namespace TDB.Utils.Misc
{
    public class SyncActive : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _targets;

        private void OnEnable()
        {
            foreach (var target in _targets)
            {
                target.SetActive(true);
            }
        }

        private void OnDisable()
        {
            foreach (var target in _targets)
            {
                target.SetActive(false);
            }
        }
    }
}
