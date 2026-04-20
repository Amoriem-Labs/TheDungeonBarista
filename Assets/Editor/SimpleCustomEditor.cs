using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SimpleCustomEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/SimpleCustomEditor")]
    public static void ShowExample()
    {
        SimpleCustomEditor wnd = GetWindow<SimpleCustomEditor>();
        wnd.titleContent = new GUIContent("SimpleCustomEditor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        // Make root fill the window
        root.style.flexDirection = FlexDirection.Row;

        // LEFT PANEL
        VisualElement leftPanel = new VisualElement();
        leftPanel.style.width = 200; // fixed width sidebar
        leftPanel.style.height = Length.Percent(100);
        leftPanel.style.flexDirection = FlexDirection.Column;
        leftPanel.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);

        root.Add(leftPanel);

        // Add buttons to left panel
        for (int i = 0; i < 5; i++)
        {
            Button btn = new Button();
            btn.text = "Button " + i;
            leftPanel.Add(btn);
        }

        // RIGHT SIDE (optional content area)
        VisualElement rightPanel = new VisualElement();
        rightPanel.style.flexGrow = 1; // fills remaining space
        root.Add(rightPanel);

        Label label = new Label("Main content area");
        rightPanel.Add(label);
    }
}
