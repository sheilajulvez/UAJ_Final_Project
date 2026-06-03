using System;
using System.Collections.Generic;
using AudioDetection.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoiceCommandManager : MonoBehaviour
{
    public static VoiceCommandManager Instance;

    private class RegisteredVoiceCommand
    {
        public IVoiceAction Action;
        public HashSet<string> Contexts;

        public RegisteredVoiceCommand(IVoiceAction action, IEnumerable<string> contexts)
        {
            Action = action;
            Contexts = new HashSet<string>();

            if (contexts != null)
            {
                foreach (string context in contexts)
                {
                    if (!string.IsNullOrWhiteSpace(context))
                    {
                        Contexts.Add(NormalizeContext(context));
                    }
                }
            }

            if (Contexts.Count == 0)
            {
                Contexts.Add("GLOBAL");
            }
        }
    }

    private Dictionary<string, RegisteredVoiceCommand> commands = new();

    [SerializeField] private bool useContextFiltering = true;

    private string currentContext = "SELECTION";
    private string previousContext = "SELECTION";

    private TextMeshProUGUI hypothesisText;
    private TextMeshProUGUI contextText;
    private Image panelImage;
    private Sprite outOfContextSprite;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            outOfContextSprite = Resources.Load<Sprite>("Sprites/outOfContext");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterCommand(string phrase, IVoiceAction action, IEnumerable<string> contexts)
    {
        string key = NormalizeCommand(phrase);

        if (!commands.ContainsKey(key))
        {
            commands.Add(key, new RegisteredVoiceCommand(action, contexts));
            Debug.Log($"[VoiceCommandManager] Registrado comando '{key}' con contextos: {string.Join(", ", commands[key].Contexts)}");
        }
        else
        {
            Debug.LogWarning($"El comando '{phrase}' ya esta registrado.");
        }
    }

    public void SetContext(string context)
    {
        currentContext = NormalizeContext(context);
        Debug.Log($"[VoiceCommandManager] Contexto actual cambiado a: {currentContext}");
        UpdateContextText();
    }

    public string GetCurrentContext()
    {
        return currentContext;
    }

    public void PushContext(string context)
    {
        previousContext = currentContext;
        SetContext(context);
    }

    public void PopContext()
    {
        SetContext(previousContext);
    }

    public void HandleCommand(string phrase, object[] parameters)
    {
        string key = NormalizeCommand(phrase);

        if (commands.TryGetValue(key, out var registeredCommand))
        {
            if (useContextFiltering && !IsCommandAllowedInCurrentContext(registeredCommand))
            {
                Debug.LogWarning($"[VoiceCommandManager] Comando '{phrase}' reconocido pero no valido en el contexto actual: {currentContext}");
                ShowFeedback($"{phrase}", outOfContextSprite);
                return;
            }

            try
            {
                registeredCommand.Action.Execute(parameters);
                TrackVoiceEvent("voice_command_executed", phrase, key, parameters, null);
            }
            catch (Exception e)
            {
                TrackVoiceEvent("voice_command_execution_failed", phrase, key, parameters, e.Message);
                Debug.LogError($"Error al ejecutar el comando '{phrase}': {e.Message}");
            }
        }
        else
        {
            TrackVoiceEvent("voice_command_execution_failed", phrase, key, parameters, "command_not_registered");
            Debug.LogWarning($"Comando no reconocido: '{phrase}'");
        }
    }

    private bool IsCommandAllowedInCurrentContext(RegisteredVoiceCommand command)
    {
        return command.Contexts.Contains("GLOBAL") || command.Contexts.Contains(currentContext);
    }

    private static string NormalizeCommand(string command)
    {
        return command.Trim().ToLower();
    }

    private static string NormalizeContext(string context)
    {
        return context.Trim().ToUpper();
    }

    private void RefreshFeedbackReferences()
    {
        if (hypothesisText == null)
        {
            hypothesisText = GameObject.Find("MessageText")?.GetComponent<TextMeshProUGUI>();
        }

        if (contextText == null)
        {
            contextText = GameObject.Find("ContextText")?.GetComponent<TextMeshProUGUI>();
        }

        if (panelImage == null)
        {
            panelImage = GameObject.Find("UnrecognizedCommandPanel")?.GetComponent<Image>();
        }
    }

    private void ShowFeedback(string message, Sprite sprite)
    {
        RefreshFeedbackReferences();

        if (hypothesisText != null)
        {
            hypothesisText.text = message;
        }

        if (panelImage != null && sprite != null)
        {
            panelImage.sprite = sprite;
        }

        UpdateContextText();
    }

    private void UpdateContextText()
    {
        RefreshFeedbackReferences();

        if (contextText != null)
        {
            contextText.text = $"Context: {currentContext}";
        }
    }

    private void TrackVoiceEvent(string eventName, string phrase, string resolvedCommand, object[] parameters, string error)
    {
        if (Tracker.Instance == null || Tracker.Instance.persistence == null)
        {
            return;
        }

        var data = new Dictionary<string, object>
        {
            { "raw_phrase", phrase ?? string.Empty },
            { "resolved_command", resolvedCommand ?? string.Empty },
            { "parameter_count", parameters?.Length ?? 0 },
            { "parameters", parameters == null ? Array.Empty<string>() : Array.ConvertAll(parameters, p => p?.ToString() ?? string.Empty) },
            { "context", currentContext ?? string.Empty },
            { "scene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name }
        };

        if (!string.IsNullOrEmpty(error))
        {
            data["error"] = error;
        }

        Tracker.Instance.TrackEvent(new TrackerEvent(eventName, "VoiceCommandTracker", data));
    }
}
