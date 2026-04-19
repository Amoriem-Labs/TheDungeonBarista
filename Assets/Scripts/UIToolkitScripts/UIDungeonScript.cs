using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class DungeonUI : MonoBehaviour
{
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        root.style.flexDirection = FlexDirection.Row;

        // load in ui textures
        Texture2D PlayerIconTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/playerIcon");
        Texture2D PlayerFrameTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/PlayerFrame");
        Texture2D PlayerOuterFrameTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/PlayerOuterFrame");
        Texture2D RecipesIconTex =  Resources.Load<Texture2D>("Prefabs/UI/UIAssets/RecipesUIButton");
        Texture2D InventoryIconTex =  Resources.Load<Texture2D>("Prefabs/UI/UIAssets/InventoryUIButton");
        Texture2D baseButtonTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/ButtonBase");
        Texture2D baseButtonHighlightTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/ButtonHighlight");
        Texture2D PauseTex = Resources.Load<Texture2D>("Prefabs/UI/UIAssets/PauseIcon");
        Texture2D[] buttonTex = {RecipesIconTex, InventoryIconTex};


        // =========================
        // LEFT SIDEBAR
        // =========================
        
        VisualElement left = new VisualElement();
        left.style.width = 140;
        left.style.height = Length.Percent(100);
        left.style.flexDirection = FlexDirection.Column;
        left.style.paddingTop = 10;
        left.style.paddingLeft = 10;

        root.Add(left);

        // Profile Image
        VisualElement profileBase = new VisualElement();
        profileBase.style.backgroundImage = new StyleBackground(PlayerFrameTex);
        profileBase.style.width = 150;
        profileBase.style.height = 150;
        profileBase.style.justifyContent = Justify.Center;
        profileBase.style.alignItems = Align.Center;

        // Player icon
        VisualElement profile = new VisualElement();
        profile.style.position = Position.Absolute;
        profile.style.width = 135;
        profile.style.height = 135;
        profile.style.backgroundImage = new StyleBackground(PlayerIconTex);

        // Frame (on top)
        VisualElement frame = new VisualElement();
        frame.style.position = Position.Absolute;
        frame.style.left = 0;
        frame.style.top = 0;
        frame.style.width = 150;
        frame.style.height = 150;
        frame.style.backgroundImage = new StyleBackground(PlayerOuterFrameTex);

        profileBase.Add(profile);
        profileBase.Add(frame);
        profileBase.style.marginBottom = 20;
        left.Add(profileBase);

        // TODO: Add later
        // // Money Bar
         VisualElement moneyBox = new VisualElement();
        // moneyBox.style.width = 100;
        // moneyBox.style.height = 30;
        // moneyBox.style.backgroundColor = new Color(0.2f, 0.15f, 0.2f);
        // moneyBox.style.marginBottom = 30;
         moneyBox.style.marginBottom = 200;

        // Label moneyText = new Label("100");
        // moneyText.style.unityTextAlign = TextAnchor.MiddleCenter;
        // moneyBox.Add(moneyText);

         left.Add(moneyBox);

        // BUTTON COLUMN

        for (int i = 0; i < buttonTex.Length; i++)
        {
            Button btn = new Button();
            // clear button default styling
            btn.style.backgroundColor = Color.clear;
            btn.style.borderTopWidth = 0;
            btn.style.borderBottomWidth = 0;
            btn.style.borderLeftWidth = 0;
            btn.style.borderRightWidth = 0;

            // background tex
            btn.style.backgroundImage = new StyleBackground(baseButtonTex);
            btn.style.width = 80;
            btn.style.height = 80;
            btn.style.marginBottom = 15;
            btn.style.justifyContent = Justify.Center;
            btn.style.alignItems = Align.Center;


            // add center icon element
            VisualElement icon = new VisualElement();
            icon.style.width = Length.Percent(100);
            icon.style.height = Length.Percent(100);

            // inner formatting stuff
            icon.style.backgroundImage = new StyleBackground(buttonTex[i]);

            // button interactive elems
            btn.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue>{ new TimeValue(0.12f, TimeUnit.Second)});
            btn.RegisterCallback<MouseEnterEvent>(evt =>
            {
                btn.style.backgroundImage = new StyleBackground(baseButtonHighlightTex);
                btn.style.translate = new Translate(0, -3);
                btn.style.scale = new StyleScale(new Scale(new Vector2(1.06f, 1.06f)));
            });

            btn.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                btn.style.backgroundImage = new StyleBackground(baseButtonTex);
                btn.style.translate = new Translate(0, 0);
                btn.style.scale = new StyleScale(new Scale(new Vector2(1f, 1f)));
            });
            

            btn.Add(icon);
            left.Add(btn);
        }



        // =========================
        // RIGHT SIDE OVERLAY
        // =========================
        VisualElement rightOverlay = new VisualElement();
        rightOverlay.style.position = Position.Absolute;
        root.Add(rightOverlay);

        // Small square (mid-right)
        VisualElement pauseBox = new VisualElement();
        pauseBox.style.backgroundImage = new StyleBackground(baseButtonTex);
        pauseBox.style.position = Position.Absolute;
        pauseBox.style.right = 10;
        pauseBox.style.top = 10; 
        pauseBox.style.width = 60;
        pauseBox.style.height = 60;
        pauseBox.style.justifyContent = Justify.Center;
        pauseBox.style.alignItems = Align.Center;

        VisualElement PauseIcon = new VisualElement();
        PauseIcon.style.width = Length.Percent(80);
        PauseIcon.style.height = Length.Percent(80);
        PauseIcon.style.backgroundImage = new StyleBackground(PauseTex);

        // interactive elems
        pauseBox.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue>{ new TimeValue(0.12f, TimeUnit.Second)});
        pauseBox.RegisterCallback<MouseEnterEvent>(evt =>
        {
            pauseBox.style.backgroundImage = new StyleBackground(baseButtonHighlightTex);
            pauseBox.style.translate = new Translate(0, -3);
            pauseBox.style.scale = new StyleScale(new Scale(new Vector2(1.06f, 1.06f)));
        });

        pauseBox.RegisterCallback<MouseLeaveEvent>(evt =>
        {
            pauseBox.style.backgroundImage = new StyleBackground(baseButtonTex);
            pauseBox.style.translate = new Translate(0, 0);
            pauseBox.style.scale = new StyleScale(new Scale(new Vector2(1f, 1f)));
        });
        
        pauseBox.Add(PauseIcon);
        root.Add(pauseBox);
    }
}