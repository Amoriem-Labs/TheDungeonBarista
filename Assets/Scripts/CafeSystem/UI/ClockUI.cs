using TDB.CafeSystem.Managers;
using UnityEngine;

namespace TDB.CafeSystem.UI
{
    public class ClockUI : MonoBehaviour
    {
        [SerializeField] private Transform _clockBody;
        [SerializeField] private CafeTimeController _cafeTimeController;
        
        private void Awake()
        {
            if (_cafeTimeController == null)
            {
                _cafeTimeController = FindObjectOfType<CafeTimeController>();
            }
        }

        private void Update()
        {
            if (_clockBody == null || _cafeTimeController == null) return;

            var progress = _cafeTimeController.OperationProgress;
            _clockBody.right = Quaternion.Euler(0f, 0f, 180f * progress) * Vector3.down;
        }
    }
}
