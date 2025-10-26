// BarTimingMinigame.cs

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace TDB.MinigameSystem.Minigames
{
    public class BarTimingMinigame : MonoBehaviour, IMinigame
    {
        [Header("Refs")]
        [SerializeField] private RectTransform rail;
        [SerializeField] private RectTransform marker;
        [SerializeField] private Slider fillBar;

        [SerializeField] private RectTransform zoneOne;
        [SerializeField] private RectTransform zoneTwo;
        [SerializeField] private RectTransform zoneThree;

        [Header("Tuning")]
        [SerializeField, Range(0.01f, 5f)] private float easySpeedPctPerSec = 0.2f;
        [SerializeField, Range(0.01f, 5f)] private float hardSpeedPctPerSec = 2.0f;
        [SerializeField] private int passes = 3;

        [SerializeField] private Vector2 goodHeightPercentRange = new Vector2(0.30f, 0.08f);

        [SerializeField, Range(0.05f, 1f)] private float awesomeOfGood = 0.6f;

        [SerializeField, Range(0.05f, 1f)] private float perfectOfGood = 0.3f;

        [Header("Scoring (per click)")]
        [SerializeField, Range(0f, 1f)] private float scoreGood = 0.40f;
        [SerializeField, Range(0f, 1f)] private float scoreAwesome = 0.70f;
        [SerializeField, Range(0f, 1f)] private float scorePerfect = 1.00f;

        private InputAction _submit;
        private Action<MinigameResult> _onComplete;

        private System.Random _rng;
        private float _difficulty;
        private float _speed;
        private float _elapsed;
        private int _direction = 1;
        private int _completedPasses;
        private float _scoreTotal;

        private Rect _railRect;

        private float _centerY;
        private float _zoneOneH;
        private float _zoneTwoH;
        private float _zoneThreeH;

        private bool _armed;

        [Button("Run Minigame")]
        private void RunMinigameEditorButton()
        {
            var asset = ScriptableObject.CreateInstance<InputActionAsset>();
            var map = new InputActionMap("Minigame");
            var submit = map.AddAction("Submit", InputActionType.Button);
            submit.AddBinding("<Mouse>/leftButton");
            submit.AddBinding("<Keyboard>/space");
            submit.AddBinding("<Gamepad>/buttonSouth");
            asset.AddActionMap(map);
            map.Enable();

            Initialize(0.5f, map, r =>
            {
                Debug.Log($"[BarTimingMinigame] Success={r.Success} Score={r.Score:0.000} Duration={r.Duration:0.000}s");
                map.Disable();
            });
            Begin();
        }

        
        public void Initialize(float diff, InputActionMap input, Action<MinigameResult> onComplete) {
            _onComplete = onComplete;
            _difficulty = Mathf.Clamp01(diff);

            _rng = new System.Random(UnityEngine.Random.Range(int.MinValue, int.MaxValue));

            Canvas.ForceUpdateCanvases();
            if (rail != null) UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rail);

            var r = rail.rect;
            _railRect = new Rect(0f, 0f, r.width, r.height);
            if (_railRect.height < 1f) _railRect.height = 1f;

            float speedPct = Mathf.Lerp(easySpeedPctPerSec, hardSpeedPctPerSec, _difficulty);
            _speed = speedPct * _railRect.height;

            float goodPct = Mathf.Lerp(goodHeightPercentRange.x, goodHeightPercentRange.y, _difficulty);
            goodPct = Mathf.Clamp(goodPct, 0.01f, 0.95f);

            _zoneOneH = _railRect.height * goodPct;
            _zoneTwoH = Mathf.Clamp(_zoneOneH * awesomeOfGood, 1f, _zoneOneH);
            _zoneThreeH = Mathf.Clamp(_zoneOneH * perfectOfGood, 1f, _zoneOneH);

            float minCenter = _zoneOneH * 0.5f;
            float maxCenter = _railRect.height - _zoneOneH * 0.5f;
            _centerY = Mathf.Lerp(minCenter, maxCenter, (float)_rng.NextDouble());

            PlaceZone(zoneOne, _zoneOneH);
            PlaceZone(zoneTwo, _zoneTwoH);
            PlaceZone(zoneThree, _zoneThreeH);

            marker.anchoredPosition = new Vector2(0f, -_railRect.height * 0.5f + 1f);

            _submit = input?.FindAction("Submit");
            if (_submit != null) _submit.performed += OnSubmit;

            if (fillBar) fillBar.value = 0f;

            _elapsed = 0f;
            _direction = 1;
            _completedPasses = 0;
            _scoreTotal = 0f;
            _armed = false;
        }

        public void Begin() => _armed = true;
        public void Abort() => Finish(false);

        private void Update()
        {
            if (_completedPasses >= passes) return;

            float dt = Time.unscaledDeltaTime;

            float y = marker.anchoredPosition.y + _direction * _speed * dt;
            float minY = -_railRect.height * 0.5f;
            float maxY =  _railRect.height * 0.5f;

            if (y >= maxY) {
                y = maxY;
                _direction = -1;
                NextPass();
            }
            else if (y <= minY) {
                y = minY;
                _direction =  1;
                NextPass();
            }

            marker.anchoredPosition = new Vector2(0f, y);
            _elapsed += dt;
        }

        private void NextPass() {
            if (_completedPasses >= passes) return;
            _completedPasses++;
            _armed = true;
            _speed *= 1.06f;

            if (_completedPasses == passes) {
                if (_onComplete != null) Finish(true);
            }
        }

        private void OnSubmit(InputAction.CallbackContext ctx) {
            if (!_armed) return;
            _armed = false;

            float y0H = marker.anchoredPosition.y + _railRect.height * 0.5f;

            float dy = Mathf.Abs(y0H - _centerY);

            float contrib = 0f;
            float halfGood = _zoneOneH * 0.5f;
            float halfAwesome = _zoneTwoH * 0.5f;
            float halfPerfect = _zoneThreeH * 0.5f;

            if (dy <= halfPerfect) {
                contrib = scorePerfect;
                Debug.Log("Perfect!");
            } else if (dy <= halfAwesome) {
                contrib = scoreAwesome;
                Debug.Log("Awesome!");
            } else if (dy <= halfGood) {
                contrib = scoreGood;
                Debug.Log("Good!");
            }

            _scoreTotal += contrib;
            if (fillBar) fillBar.value = Mathf.Clamp01(_scoreTotal / (passes * scorePerfect));
        }

        private void Finish(bool completedNaturally) {
            var cb = _onComplete;
            _onComplete = null;
            if (_submit != null) _submit.performed -= OnSubmit;

            float norm = Mathf.Clamp01(_scoreTotal / (passes * scorePerfect));
            cb?.Invoke(new MinigameResult {
                Success  = completedNaturally,
                Score    = norm,
                Duration = _elapsed
            });
        }
        private void PlaceZone(RectTransform zone, float heightPixels) {
            if (zone == null) return;

            float centeredY = _centerY - _railRect.height * 0.5f;
            zone.anchoredPosition = new Vector2(0f, centeredY);
            zone.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, heightPixels);
        }
    }
}
