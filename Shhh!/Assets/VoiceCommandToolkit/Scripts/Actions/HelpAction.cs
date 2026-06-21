using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AudioDetection.Interfaces;

public class HelpAction : IVoiceAction
{
    private static GameObject runtimeHelpOverlay;

    public void Execute(params object[] parameters)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Selection" || currentScene == "Victory")
        {
            return;
        }

        GameObject control = FindInActiveScene("Control");
        if (control != null)
        {
            Button button = control.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.Invoke();
                return;
            }
        }

        ToggleRuntimeHelpOverlay();
    }

    private static GameObject FindInActiveScene(string objectName)
    {
        GameObject activeObject = GameObject.Find(objectName);
        if (activeObject != null)
        {
            return activeObject;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        foreach (GameObject candidate in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (candidate.name == objectName && candidate.scene == activeScene)
            {
                return candidate;
            }
        }

        return null;
    }

    private static void ToggleRuntimeHelpOverlay()
    {
        if (runtimeHelpOverlay == null)
        {
            runtimeHelpOverlay = CreateRuntimeHelpOverlay();
        }

        runtimeHelpOverlay.SetActive(!runtimeHelpOverlay.activeSelf);
    }

    private static GameObject CreateRuntimeHelpOverlay()
    {
        GameObject canvasObject = new GameObject("VoiceHelpOverlay");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject panelObject = new GameObject("Panel");
        panelObject.transform.SetParent(canvasObject.transform, false);

        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.78f);

        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(520f, 320f);
        panelRect.anchoredPosition = Vector2.zero;

        GameObject textObject = new GameObject("CommandsText");
        textObject.transform.SetParent(panelObject.transform, false);

        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 26;
        text.alignment = TextAnchor.MiddleLeft;
        text.color = Color.white;
        text.text = "Voice commands\n\nmove / walk / forward / advance\nlook left / right / up / down\nturn left / right\nrotate left / right\ngo / travel / navigate + place\npick / grab / take\nquit / leave\n\nSay help or commands to hide this panel.";

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 0f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.offsetMin = new Vector2(32f, 24f);
        textRect.offsetMax = new Vector2(-32f, -24f);

        canvasObject.SetActive(false);
        return canvasObject;
    }
}
