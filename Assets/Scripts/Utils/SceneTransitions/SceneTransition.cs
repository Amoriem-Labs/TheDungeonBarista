using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

//using DG.Tweening;

namespace TDB.Utils.SceneTransitions
{
    [ExecuteAlways]
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] protected Image _cover;
        // [SerializeField] private float _fadeTime;

        public virtual IEnumerator StartTransitionIntro()
        {
            _cover.gameObject.SetActive(true);
            
            // var anim = _cover.DOFade(1, _fadeTime)
            //     .SetUpdate(true);
            // yield return anim.WaitForCompletion();
            yield return new WaitForSeconds(.5f);
        }

        public virtual IEnumerator StartTransitionOutro()
        {
            //var anim = _cover.DOFade(0, _fadeTime)
            //    .SetUpdate(true)
            //    .OnComplete(delegate
            //    {
            //        gameObject.SetActive(false);
            //    });
            // yield return anim.WaitForCompletion();
            
            _cover.gameObject.SetActive(false);
            yield return new WaitForSeconds(.5f);
        }
    }
}