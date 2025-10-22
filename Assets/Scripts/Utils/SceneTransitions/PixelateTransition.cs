// using System.Collections;
// using DG.Tweening;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace GMTK25.Utils.SceneTransitions
// {
//     public class PixelateTransition : SceneTransition
//     {
//         [SerializeField]
//         private FullScreenPassRendererFeature _pixelateFeature;
//         [SerializeField]
//         private Material _pixelateMaterial;
//         
//         [SerializeField] private float _transitionDuration = 1f;
//         [SerializeField, MinMaxSlider(1, 512)]
//         private Vector2 _pixelSizeRange = new Vector2(1f, 512f);
//
//         private void Awake()
//         {
//             _cover.gameObject.SetActive(false);
//             _pixelateFeature.SetActive(false);
//         }
//
//         public override IEnumerator StartTransitionIntro()
//         {
//             // _cover.gameObject.SetActive(true);
//             // _cover.DOFade(1, _transitionDuration)
//             //     .From(0).SetUpdate(true);
//             _pixelateFeature.SetActive(true);
//             _pixelateMaterial.DOFloat(_pixelSizeRange.y, "_PixelSize", _transitionDuration)
//                 .From(_pixelSizeRange.x).SetUpdate(true);
//             _pixelateMaterial.DOFloat(1, "_TransitionProgress", _transitionDuration)
//                 .From(0).SetUpdate(true);
//             yield return new WaitForSecondsRealtime(_transitionDuration);
//         }
//
//         public override IEnumerator StartTransitionOutro()
//         {
//             // _cover.DOFade(0, _transitionDuration)
//             //     .From(1).SetUpdate(true)
//             //     .OnComplete(() =>
//             //     {
//             //         _cover.gameObject.SetActive(false);
//             //     });
//             _pixelateMaterial.DOFloat(_pixelSizeRange.x, "_PixelSize", _transitionDuration)
//                 .From(_pixelSizeRange.y).SetUpdate(true)
//                 .OnComplete(() =>
//                 {
//                     _pixelateFeature.SetActive(false);
//                 });
//             _pixelateMaterial.DOFloat(0, "_TransitionProgress", _transitionDuration)
//                 .From(1).SetUpdate(true);
//             yield return new WaitForSecondsRealtime(_transitionDuration);
//         }
//     }
// }