using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using TDB;  

public class DungeonUI : MonoBehaviour
{
    private VisualElement pauseMenu;
    private VisualElement gameplayUI;
    private bool isPaused = false;
    private Font pixelFont;
    private Font bossFont;

    // boss ui
    private VisualElement healthFill;
    private VisualElement bossHealthContainer;
    private float _trackWidth = 0f;
    public EntityData boss;

    // player ui variables
    public EntityData player;
    private VisualElement playerHealthFill;
    private float _playerTrackHeight = 0f;
    private VisualElement playerProfileIcon; 
    private Texture2D _playerIconTex;
    private Texture2D _playerIconInjuredTex;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // =========================
        // GAMEPLAY ROOT CONTAINER
        // =========================
        gameplayUI = new VisualElement();
        gameplayUI.style.flexDirection = FlexDirection.Row;
        gameplayUI.style.flexGrow = 1;

        root.Add(gameplayUI);

        // load font
        pixelFont = Resources.Load<Font>("Prefabs/UI/UIAssets/Fonts/PressStart2P-Regular");
        bossFont = Resources.Load<Font>("Prefabs/UI/UIAssets/Fonts/Cinzel-Bold");

        // =========================
        // LOAD TEXTURES
        // =========================
        _playerIconTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/playerIcon");
        _playerIconInjuredTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/playerIconInjured");
        Texture2D PlayerFrameTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/PlayerFrame");
        Texture2D PlayerOuterFrameTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/PlayerOuterFrame");

        Texture2D RecipesIconTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/RecipesUIButton");
        Texture2D InventoryIconTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/InventoryUIButton");
        Texture2D MapIconTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/MapUIButton");

        Texture2D baseButtonTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/ButtonBase");
        Texture2D baseButtonHighlightTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/ButtonHighlight");
        Texture2D baseBoxTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/box");
        Texture2D baseBoxHighlightTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/boxHighlight");
        Texture2D PauseTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/PauseIcon");

        Texture2D[] buttonTex = { RecipesIconTex, InventoryIconTex };

        // =========================
        // LEFT SIDEBAR
        // =========================
        VisualElement left = new VisualElement();
        left.style.width = 140;
        left.style.height = Length.Percent(100);
        left.style.flexDirection = FlexDirection.Column;
        left.style.paddingTop = 10;
        left.style.paddingLeft = 10;

        gameplayUI.Add(left);

        // Profile
        VisualElement profileBase = new VisualElement();
        profileBase.style.backgroundImage = new StyleBackground(baseButtonTex);
        profileBase.style.width = 150;
        profileBase.style.height = 150;
        profileBase.style.justifyContent = Justify.Center;
        profileBase.style.alignItems = Align.Center;

        playerProfileIcon = new VisualElement();
        playerProfileIcon.style.position = Position.Absolute;
        playerProfileIcon.style.width = 135;
        playerProfileIcon.style.height = 135;
        playerProfileIcon.style.backgroundImage = new StyleBackground(_playerIconTex);

        VisualElement frame = new VisualElement();
        frame.style.position = Position.Absolute;
        frame.style.width = 150;
        frame.style.height = 150;
        frame.style.backgroundImage = new StyleBackground(PlayerOuterFrameTex);

        profileBase.Add(playerProfileIcon);
        profileBase.Add(frame);
        profileBase.style.marginBottom = 20;

        left.Add(profileBase);

        // BUTTONS
        for (int i = 0; i < buttonTex.Length; i++)
        {
            Button btn = new Button();

            btn.style.backgroundColor = Color.clear;
            btn.style.borderTopWidth = 0;
            btn.style.borderBottomWidth = 0;
            btn.style.borderLeftWidth = 0;
            btn.style.borderRightWidth = 0;
            btn.style.justifyContent = Justify.Center;
            btn.style.alignItems = Align.Center;

            btn.style.backgroundImage = new StyleBackground(baseButtonTex);
            btn.style.width = 80;
            btn.style.height = 80;
            btn.style.marginBottom = 15;

            VisualElement icon = new VisualElement();
            icon.style.width = 64;
            icon.style.height = 64;
            icon.style.backgroundImage = new StyleBackground(buttonTex[i]);

            btn.Add(icon);
            left.Add(btn);

            // hover
            btn.RegisterCallback<MouseEnterEvent>(_ =>
            {
                btn.style.backgroundImage = new StyleBackground(baseButtonHighlightTex);
                btn.style.translate = new Translate(0, -2);
            });

            btn.RegisterCallback<MouseLeaveEvent>(_ =>
            {
                btn.style.backgroundImage = new StyleBackground(baseButtonTex);
                btn.style.translate = new Translate(0, 0);
            });
        }

        // =========================
        // PLAYER HEALTH BAR (vertical)
        // =========================
        VisualElement playerHealthTrack = new VisualElement();
        playerHealthTrack.style.width = 20;
        playerHealthTrack.style.flexGrow = 0;
        playerHealthTrack.style.height = 150;
        playerHealthTrack.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.85f);
        playerHealthTrack.style.borderTopLeftRadius = 4;
        playerHealthTrack.style.borderTopRightRadius = 4;
        playerHealthTrack.style.borderBottomLeftRadius = 4;
        playerHealthTrack.style.borderBottomRightRadius = 4;
        playerHealthTrack.style.borderTopWidth = 2;
        playerHealthTrack.style.borderBottomWidth = 2;
        playerHealthTrack.style.borderLeftWidth = 2;
        playerHealthTrack.style.borderRightWidth = 2;
        playerHealthTrack.style.borderTopColor = new Color(0.345f, 0.933f, 0.804f); 
        playerHealthTrack.style.borderBottomColor = new Color(0.345f, 0.933f, 0.804f); 
        playerHealthTrack.style.borderLeftColor = new Color(0.345f, 0.933f, 0.804f); 
        playerHealthTrack.style.borderRightColor = new Color(0.345f, 0.933f, 0.804f); 
        playerHealthTrack.style.overflow = Overflow.Hidden;
        playerHealthTrack.style.marginTop = 10;
        playerHealthTrack.style.marginLeft = 10;

        // fill sits at the BOTTOM and shrinks upward
        playerHealthFill = new VisualElement();
        playerHealthFill.style.position = Position.Absolute;
        playerHealthFill.style.left = 0;
        playerHealthFill.style.right = 0;
        playerHealthFill.style.bottom = 0;
        playerHealthFill.style.height = Length.Percent(100);
        playerHealthFill.style.backgroundColor = new Color(0.345f, 0.933f, 0.804f); 

        playerHealthTrack.Add(playerHealthFill);

        playerHealthTrack.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            _playerTrackHeight = evt.newRect.height;
        });

        left.Add(playerHealthTrack);

        // =========================
        // PAUSE BUTTON
        // =========================
        VisualElement pauseBox = new VisualElement();
        pauseBox.style.backgroundImage = new StyleBackground(baseButtonTex);
        pauseBox.style.position = Position.Absolute;
        pauseBox.style.right = 20;
        pauseBox.style.top = 20;
        pauseBox.style.width = 60;
        pauseBox.style.height = 60;
        pauseBox.style.justifyContent = Justify.Center;
        pauseBox.style.alignItems = Align.Center;

        VisualElement pauseIcon = new VisualElement();
        pauseIcon.style.width = 48;
        pauseIcon.style.height = 48;
        pauseIcon.style.backgroundImage = new StyleBackground(PauseTex);

        pauseBox.Add(pauseIcon);

        pauseBox.RegisterCallback<MouseDownEvent>(_ =>
        {
            TogglePause();
        });

        pauseBox.RegisterCallback<MouseEnterEvent>(_ =>
        {
            pauseBox.style.backgroundImage = new StyleBackground(baseButtonHighlightTex);
            pauseBox.style.translate = new Translate(0, -2);
        });

        pauseBox.RegisterCallback<MouseLeaveEvent>(_ =>
        {
            pauseBox.style.backgroundImage = new StyleBackground(baseButtonTex);
            pauseBox.style.translate = new Translate(0, 0);
        });

        gameplayUI.Add(pauseBox);

    // =========================
    // BOSS TITLE
    // =========================
    VisualElement bossOverlay = new VisualElement();
    bossOverlay.style.position = Position.Absolute;
    bossOverlay.style.left = 0;
    bossOverlay.style.top = 0;
    bossOverlay.style.right = 0;
    bossOverlay.style.bottom = 0;

    bossOverlay.style.justifyContent = Justify.FlexStart;
    bossOverlay.style.alignItems = Align.Center;
    bossOverlay.style.paddingTop = 50;

    // block clicks through it if needed
    bossOverlay.pickingMode = PickingMode.Ignore;

    // label
    Label bossTitle = new Label("CERVITUS, LORD OF THE DARK");

    //  STYLING
    bossTitle.style.unityFontDefinition = new StyleFontDefinition(bossFont);
    bossTitle.style.fontSize = 64;
    bossTitle.style.color = Color.white;
    bossTitle.style.unityTextAlign = TextAnchor.MiddleCenter;

    // spacing makes it feel epic
    bossTitle.style.letterSpacing = 4;

    // optional: max width so it wraps nicely
    bossTitle.style.maxWidth = Length.Percent(80);

    // outline/glow effect (UI Toolkit trick)
    TextShadow shadow = new TextShadow();
    shadow.color = new Color(0, 0, 0, 0.8f);
    shadow.offset = new Vector2(2, 2);
    shadow.blurRadius = 4f;

    bossTitle.style.textShadow = shadow;

    bossOverlay.Add(bossTitle);
    root.Add(bossOverlay);
    // =========================
        // BOSS HEALTH BAR
    // =========================
        bossHealthContainer = new VisualElement();
        bossHealthContainer.style.position = Position.Absolute;
        bossHealthContainer.style.bottom = 40;
        bossHealthContainer.style.left = Length.Percent(15);
        bossHealthContainer.style.right = Length.Percent(15);
        bossHealthContainer.style.height = 50;
        bossHealthContainer.style.flexDirection = FlexDirection.Column;

        // Boss name label above bar
        Label bossBarLabel = new Label("CERVITUS");
        bossBarLabel.style.unityFontDefinition = new StyleFontDefinition(bossFont);
        bossBarLabel.style.fontSize = 14;
        bossBarLabel.style.color = Color.white;
        bossBarLabel.style.marginBottom = 4;

        TextShadow barLabelShadow = new TextShadow();
        barLabelShadow.color = new Color(0f, 0f, 0f, 0.9f);
        barLabelShadow.offset = new Vector2(1, 1);
        barLabelShadow.blurRadius = 3f;
        bossBarLabel.style.textShadow = barLabelShadow;

        // Outer track
        VisualElement healthTrack = new VisualElement();
        healthTrack.style.width = Length.Percent(100);
        healthTrack.style.height = 20;
        healthTrack.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.85f);
        healthTrack.style.borderTopLeftRadius = 4;
        healthTrack.style.borderTopRightRadius = 4;
        healthTrack.style.borderBottomLeftRadius = 4;
        healthTrack.style.borderBottomRightRadius = 4;
        healthTrack.style.borderTopWidth = 2;
        healthTrack.style.borderBottomWidth = 2;
        healthTrack.style.borderLeftWidth = 2;
        healthTrack.style.borderRightWidth = 2;
        healthTrack.style.borderTopColor = new Color(0.4f, 0.0f, 0.0f);
        healthTrack.style.borderBottomColor = new Color(0.4f, 0.0f, 0.0f);
        healthTrack.style.borderLeftColor = new Color(0.4f, 0.0f, 0.0f);
        healthTrack.style.borderRightColor = new Color(0.4f, 0.0f, 0.0f);
        healthTrack.style.overflow = Overflow.Hidden;
        healthTrack.style.alignSelf = Align.Stretch;


        // Inner fill - use scale instead of width
        healthFill = new VisualElement();
        healthFill.style.position = Position.Absolute;
        healthFill.style.left = 0;
        healthFill.style.top = 0;
        healthFill.style.bottom = 0;
        healthFill.style.width = Length.Percent(100);
        healthFill.style.backgroundColor = new Color(0.45f, 0.02f, 0.02f);

        healthTrack.Add(healthFill);

        // Grab the real pixel width once layout resolves
        healthTrack.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            _trackWidth = evt.newRect.width;
        });

        bossHealthContainer.Add(bossBarLabel);
        bossHealthContainer.Add(healthTrack);

        root.Add(bossHealthContainer);
        if (boss != null)
            boss.OnDeath += HideHealthBar;

        if (player != null)
            player.OnDeath += EmptyPlayerBar;

        // =========================
        // PAUSE MENU
        // =========================
        pauseMenu = new VisualElement();
        pauseMenu.style.position = Position.Absolute;
        pauseMenu.style.left = 0;
        pauseMenu.style.top = 0;
        pauseMenu.style.right = 0;
        pauseMenu.style.bottom = 0;

        pauseMenu.style.justifyContent = Justify.Center;
        pauseMenu.style.alignItems = Align.Center;
        pauseMenu.style.display = DisplayStyle.None;

        Label title = new Label("PAUSED");
        title.style.fontSize = 40;
        title.style.color = Color.white;

        VisualElement resumeButton = new VisualElement();
        resumeButton.style.backgroundImage = new StyleBackground(baseBoxTex);
        Label rtext = new Label("Resume");
        resumeButton.Add(rtext);
        resumeButton.style.color = Color.white;
        resumeButton.style.width = 160;
        resumeButton.style.height = 64;
        resumeButton.style.justifyContent = Justify.Center;
        resumeButton.style.alignItems = Align.Center;
        resumeButton.RegisterCallback<MouseDownEvent>(_ => {
            TogglePause();
        });

        VisualElement quitButton = new VisualElement(); new Button(() => Application.Quit());
        quitButton.style.backgroundImage = new StyleBackground(baseBoxTex);
        Label text = new Label("Quit");
        quitButton.Add(text);
        quitButton.style.color = Color.white;
        quitButton.style.width = 120;
        quitButton.style.height = 48;
        quitButton.style.justifyContent = Justify.Center;
        quitButton.style.alignItems = Align.Center;
        quitButton.RegisterCallback<MouseDownEvent>(_ => {
            TogglePause();
        });

        title.style.unityFontDefinition = new StyleFontDefinition(pixelFont);
        resumeButton.style.unityFontDefinition = new StyleFontDefinition(pixelFont);
        quitButton.style.unityFontDefinition = new StyleFontDefinition(pixelFont);

        // hover
        resumeButton.RegisterCallback<MouseEnterEvent>(_ =>
        {
            resumeButton.style.backgroundImage = new StyleBackground(baseBoxHighlightTex);
            resumeButton.style.translate = new Translate(0, -2);
        });

        resumeButton.RegisterCallback<MouseLeaveEvent>(_ =>
        {
            resumeButton.style.backgroundImage = new StyleBackground(baseBoxTex);
            resumeButton.style.translate = new Translate(0, 0);
        });

        quitButton.RegisterCallback<MouseEnterEvent>(_ =>
        {
            quitButton.style.backgroundImage = new StyleBackground(baseBoxHighlightTex);
            quitButton.style.translate = new Translate(0, -2);
        });

        quitButton.RegisterCallback<MouseLeaveEvent>(_ =>
        {
            quitButton.style.backgroundImage = new StyleBackground(baseBoxTex);
            quitButton.style.translate = new Translate(0, 0);
        });

        pauseMenu.Add(title);
        pauseMenu.Add(resumeButton);
        pauseMenu.Add(quitButton);

        root.Add(pauseMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        UpdateHealthBar();
    }
    private void UpdateHealthBar()
    {
        // boss bar
        if (boss != null && healthFill != null)
        {
            float bossPct = Mathf.Clamp01((float)boss.CurrentHealth / (float)boss.MaxHealth);
            if (_trackWidth > 0f)
                healthFill.style.width = _trackWidth * bossPct;
        }

        if (player != null && playerProfileIcon != null)
        {
            float playerPct = Mathf.Clamp01((float)player.CurrentHealth / (float)player.MaxHealth);
            if (playerPct < 0.5f)
                playerProfileIcon.style.backgroundImage = new StyleBackground(_playerIconInjuredTex);
            else
                playerProfileIcon.style.backgroundImage = new StyleBackground(_playerIconTex);
        }

        // player bar
        if (player != null && playerHealthFill != null)
        {
            float playerPct = Mathf.Clamp01((float)player.CurrentHealth / (float)player.MaxHealth);
            if (_playerTrackHeight > 0f)
                playerHealthFill.style.height = _playerTrackHeight * playerPct;
        }   
    }

    private void EmptyPlayerBar()
    {
        if (playerHealthFill != null)
            playerHealthFill.style.height = 0;
    }

    private void HideHealthBar()
    {
        if (bossHealthContainer != null)
            bossHealthContainer.style.display = DisplayStyle.None;
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        pauseMenu.style.display = isPaused
            ? DisplayStyle.Flex
            : DisplayStyle.None;

        // block gameplay UI clicks while paused
        gameplayUI.pickingMode = isPaused
            ? PickingMode.Ignore
            : PickingMode.Position;

        Time.timeScale = isPaused ? 0f : 1f;
    }
}