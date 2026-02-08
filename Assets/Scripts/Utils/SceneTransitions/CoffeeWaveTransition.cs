using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TDB.Utils.SceneTransitions
{
    public class CoffeeWaveTransition : MonoBehaviour, ISceneTransition
    {
        [SerializeField] private Image _cover;
        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _introCurve;

        public IEnumerator StartTransitionIntro()
        {
            _cover.gameObject.SetActive(true);

            var anim = _cover.material.DOFloat(1, "_Progress", _duration)
                .SetUpdate(true)
                .SetEase(_introCurve)
                .From(0);
            yield return anim.WaitForCompletion();
        }

        public IEnumerator StartTransitionOutro()
        {
            var anim = _cover.material.DOFloat(0, "_Progress", _duration)
                .SetUpdate(true)
                .SetEase(Ease.OutQuad)
                .From(1);

            yield return anim.WaitForCompletion();
            _cover.gameObject.SetActive(false);
        }
    }
}