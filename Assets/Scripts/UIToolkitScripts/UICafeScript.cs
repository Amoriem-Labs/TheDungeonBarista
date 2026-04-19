using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class RuntimeUI : MonoBehaviour
{
    private VisualElement pauseMenu;
    private VisualElement gameplayUI;
    private bool isPaused = false;
    private Font pixelFont;

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

        // =========================
        // LOAD TEXTURES
        // =========================
        Texture2D PlayerIconTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/playerIcon");
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

        Texture2D[] buttonTex = { RecipesIconTex, InventoryIconTex, MapIconTex };

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

        VisualElement profile = new VisualElement();
        profile.style.position = Position.Absolute;
        profile.style.width = 135;
        profile.style.height = 135;
        profile.style.backgroundImage = new StyleBackground(PlayerIconTex);

        VisualElement frame = new VisualElement();
        frame.style.position = Position.Absolute;
        frame.style.width = 150;
        frame.style.height = 150;
        frame.style.backgroundImage = new StyleBackground(PlayerOuterFrameTex);

        profileBase.Add(profile);
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