using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenuUI : MonoBehaviour
{
    private VisualElement _emberContainer;
    private List<EmberData> _embers = new List<EmberData>();
    private IVisualElementScheduledItem _emberTick;

    private const int   EMBER_COUNT     = 28;
    private const float EMBER_MIN_SPEED = 28f;  // px per second
    private const float EMBER_MAX_SPEED = 70f;
    private const float EMBER_MIN_SIZE  = 2f;
    private const float EMBER_MAX_SIZE  = 5f;
    private const float DRIFT_STRENGTH  = 18f;  // horizontal sway amplitude
    private const long  TICK_MS         = 16;   // ~60fps

    private class EmberData
    {
        public VisualElement el;
        public float x, y;        // current position (px)
        public float speed;       // fall speed px/s
        public float drift;       // horizontal drift px/s
        public float driftOffset; // phase offset for sine sway
        public float size;
        public float alpha;
        public float alphaSpeed;  // how fast alpha pulses
        public float alphaPhase;
        public float screenH;
        public float screenW;
    }

    void Awake()
    {
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        root.Clear();

        // =========================
        // LOAD ASSETS
        // =========================
        Font pixelFont = Resources.Load<Font>("Prefabs/UI/UIAssets/Fonts/PressStart2P-Regular");

        Texture2D baseBoxTex          = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/box");
        Texture2D baseBoxHighlightTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/boxHighlight");
        Texture2D titleTex            = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/Menu Full Art");

        // =========================
        // ROOT
        // =========================
        root.style.position = Position.Absolute;
        root.style.left     = 0;
        root.style.top      = 0;
        root.style.right    = 0;
        root.style.bottom   = 0;

        // =========================
        // FLAT BACKGROUND
        // =========================
        VisualElement bg = new VisualElement();
        bg.style.position        = Position.Absolute;
        bg.style.left            = 0;
        bg.style.top             = 0;
        bg.style.right           = 0;
        bg.style.bottom          = 0;
        bg.style.backgroundColor = new Color(0.07f, 0.05f, 0.03f, 1f);
        bg.pickingMode           = PickingMode.Ignore;
        root.Add(bg);

        // =========================
        // EMBER CONTAINER
        // Sits above background, below content — pickingMode Ignore so
        // embers never eat mouse events
        // =========================
        _emberContainer = new VisualElement();
        _emberContainer.style.position = Position.Absolute;
        _emberContainer.style.left     = 0;
        _emberContainer.style.top      = 0;
        _emberContainer.style.right    = 0;
        _emberContainer.style.bottom   = 0;
        _emberContainer.pickingMode    = PickingMode.Ignore;
        root.Add(_emberContainer);

        // Spawn embers once layout is known
        _emberContainer.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            float w = evt.newRect.width;
            float h = evt.newRect.height;
            if (w < 1f || h < 1f || _embers.Count > 0) return;
            SpawnEmbers(w, h);
            StartEmberTick();
        });

        // =========================
        // CONTENT CONTAINER
        // =========================
        VisualElement content = new VisualElement();
        content.style.position       = Position.Absolute;
        content.style.left           = 0;
        content.style.right          = 0;
        content.style.top            = 0;
        content.style.bottom         = 0;
        content.style.justifyContent = Justify.Center;
        content.style.alignItems     = Align.Center;
        content.style.flexDirection  = FlexDirection.Column;
        root.Add(content);

        // =========================
        // TITLE IMAGE
        // =========================
        VisualElement titleImage = new VisualElement();
        titleImage.style.width                    = 1600;
        titleImage.style.height                   = 520;
        titleImage.style.backgroundImage          = new StyleBackground(titleTex);
        titleImage.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        titleImage.style.alignSelf                = Align.Center;
        titleImage.style.marginBottom             = 60;
        content.Add(titleImage);

        // =========================
        // DIVIDER
        // =========================
        VisualElement divider = new VisualElement();
        divider.style.width           = 180;
        divider.style.height          = 1;
        divider.style.backgroundColor = new Color(0.80f, 0.50f, 0.15f, 0.40f);
        divider.style.alignSelf       = Align.Center;
        divider.style.marginBottom    = 28;
        content.Add(divider);

        // =========================
        // BUTTON COLUMN
        // =========================
        VisualElement buttonCol = new VisualElement();
        buttonCol.style.flexDirection = FlexDirection.Column;
        buttonCol.style.alignItems    = Align.Center;
        content.Add(buttonCol);

        // =========================
        // HELPER: make a menu button
        // =========================
        VisualElement MakeButton(string labelText)
        {
            VisualElement btn = new VisualElement();
            btn.style.backgroundImage = new StyleBackground(baseBoxTex);
            btn.style.width           = 220;
            btn.style.height          = 58;
            btn.style.justifyContent  = Justify.Center;
            btn.style.alignItems      = Align.Center;
            btn.style.marginBottom    = 18;

            Label lbl = new Label(labelText);
            lbl.style.unityFontDefinition = new StyleFontDefinition(pixelFont);
            lbl.style.fontSize            = 13;
            lbl.style.color               = new Color(0.95f, 0.78f, 0.42f);
            lbl.style.unityTextAlign      = TextAnchor.MiddleCenter;
            lbl.pickingMode               = PickingMode.Ignore;

            TextShadow shadow = new TextShadow
            {
                color      = new Color(0.4f, 0.15f, 0f, 0.9f),
                offset     = new Vector2(2, 2),
                blurRadius = 4f
            };
            lbl.style.textShadow = shadow;
            btn.Add(lbl);

            btn.RegisterCallback<MouseEnterEvent>(_ =>
            {
                btn.style.backgroundImage = new StyleBackground(baseBoxHighlightTex);
                btn.style.translate       = new Translate(0, -3);
                lbl.style.color           = new Color(1f, 0.95f, 0.75f);
            });
            btn.RegisterCallback<MouseLeaveEvent>(_ =>
            {
                btn.style.backgroundImage = new StyleBackground(baseBoxTex);
                btn.style.translate       = new Translate(0, 0);
                lbl.style.color           = new Color(0.95f, 0.78f, 0.42f);
            });

            return btn;
        }

        // PLAY
        VisualElement playBtn = MakeButton("PLAY");
        playBtn.RegisterCallback<MouseDownEvent>(_ => SceneManager.LoadScene("Forest_Boss"));
        buttonCol.Add(playBtn);

        // QUIT
        VisualElement quitBtn = MakeButton("QUIT");
        quitBtn.RegisterCallback<MouseDownEvent>(_ =>
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });
        buttonCol.Add(quitBtn);
    }

    // ─────────────────────────────────────────────
    // EMBER SYSTEM
    // ─────────────────────────────────────────────

    private void SpawnEmbers(float screenW, float screenH)
    {
        for (int i = 0; i < EMBER_COUNT; i++)
        {
            float size = Random.Range(EMBER_MIN_SIZE, EMBER_MAX_SIZE);

            VisualElement el = new VisualElement();
            el.style.position              = Position.Absolute;
            el.style.width                 = size;
            el.style.height                = size;
            el.style.borderTopLeftRadius   = size;
            el.style.borderTopRightRadius  = size;
            el.style.borderBottomLeftRadius  = size;
            el.style.borderBottomRightRadius = size;
            el.pickingMode = PickingMode.Ignore;

            // Embers range from deep orange to bright amber
            float heatLerp = Random.value;
            Color emberColor = Color.Lerp(
                new Color(0.173f, 0.466f, 0.402f),  // deep blue
                new Color(0.345f, 0.933f, 0.804f),   // bright blue
                heatLerp
            );

            float startAlpha = Random.Range(0.4f, 0.9f);
            emberColor.a = startAlpha;
            el.style.backgroundColor = emberColor;

            // Start embers at random positions so screen isn't empty at launch
            float startY = Random.Range(-screenH * 0.1f, screenH * 1.05f);
            float startX = Random.Range(0f, screenW);

            el.style.left = startX;
            el.style.top  = startY;

            _emberContainer.Add(el);

            _embers.Add(new EmberData
            {
                el          = el,
                x           = startX,
                y           = startY,
                speed       = Random.Range(EMBER_MIN_SPEED, EMBER_MAX_SPEED),
                drift       = Random.Range(-DRIFT_STRENGTH, DRIFT_STRENGTH),
                driftOffset = Random.Range(0f, Mathf.PI * 2f),
                size        = size,
                alpha       = startAlpha,
                alphaSpeed  = Random.Range(0.8f, 2.2f),
                alphaPhase  = Random.Range(0f, Mathf.PI * 2f),
                screenH     = screenH,
                screenW     = screenW,
            });
        }
    }

    private float _elapsedSec = 0f;

    private void StartEmberTick()
    {
        // UI Toolkit scheduler: fires every TICK_MS milliseconds
        _emberTick = _emberContainer.schedule
            .Execute(() => TickEmbers(TICK_MS / 1000f))
            .Every(TICK_MS);
    }

    private void TickEmbers(float dt)
    {
        foreach (var e in _embers)
        {
            // Fall downward
            e.y += e.speed * dt;

            // Gentle sine sway side to side
            _elapsedSec += dt / _embers.Count; // stagger phase advance per ember
            float sway = Mathf.Sin(_elapsedSec * e.drift * 0.15f + e.driftOffset) * e.drift;
            e.x += sway * dt;

            // Pulse alpha — embers glow and dim like real cinders
            e.alphaPhase += e.alphaSpeed * dt;
            e.alpha = Mathf.Clamp(0.35f + 0.55f * (0.5f + 0.5f * Mathf.Sin(e.alphaPhase)), 0f, 1f);

            // Recycle off-screen embers back to the top
            if (e.y > e.screenH + e.size)
            {
                e.y = -e.size - Random.Range(0f, 40f);
                e.x = Random.Range(0f, e.screenW);
            }

            // Clamp x so embers don't drift off the sides permanently
            if (e.x < -e.size)   e.x = e.screenW + e.size;
            if (e.x > e.screenW) e.x = -e.size;

            // Apply to element
            e.el.style.left = e.x;
            e.el.style.top  = e.y;

            Color c = e.el.style.backgroundColor.value;
            c.a = e.alpha;
            e.el.style.backgroundColor = c;
        }
    }

    private void OnDisable()
    {
        // Stop the scheduler when the component is disabled to avoid leaks
        _emberTick?.Pause();
        _embers.Clear();
    }
}