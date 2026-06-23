using System;
using System.Collections.Generic;
using UnityEngine;
using AudioDetection.Interfaces;
using UAJ.Telemetry;

public class VoiceCommandManager : MonoBehaviour
{
    public static VoiceCommandManager Instance;

    private Dictionary<string, IVoiceAction> commands = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterCommand(string phrase, IVoiceAction action)
    {
        string key = phrase.ToLower();
        if (!commands.ContainsKey(key))
        {
            commands.Add(key, action);
        }
        else
        {
            Debug.LogWarning($"El comando '{phrase}' ya está registrado.");
        }
    }

    public void HandleCommand(string phrase)
    {
        HandleCommand(phrase, null); // Llama a la sobrecarga con parámetros nulos
    }

    public void HandleCommand(string phrase, object[] parameters)
    {
        string key = phrase.ToLower();
        if (commands.TryGetValue(key, out var action))
        {
            try
            {
                action.Execute(parameters);
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
            Debug.LogWarning($"Comando no reconocido: {phrase}");
        }
    }

    private void TrackVoiceEvent(string eventName, string phrase, string resolvedCommand, object[] parameters, string error)
    {
        if (Tracker.Instance == null || Tracker.Instance.persistence == null)
        {
            return;
        }

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string context = sceneName == "Selection" ? "SELECTION" : sceneName == "Menu" ? "MENU" : "IN_GAME";

        var data = new Dictionary<string, object>
        {
            { "raw_phrase", phrase ?? string.Empty },
            { "resolved_command", resolvedCommand ?? string.Empty },
            { "parameter_count", parameters?.Length ?? 0 },
            { "parameters", parameters == null ? Array.Empty<string>() : Array.ConvertAll(parameters, p => p?.ToString() ?? string.Empty) },
            { "context", context },
            { "scene", sceneName }
        };

        if (!string.IsNullOrEmpty(error))
        {
            data["error"] = error;
        }

        Tracker.Instance.TrackEvent(new TrackerEvent(eventName, "VoiceCommandTracker", data));
    }
}
