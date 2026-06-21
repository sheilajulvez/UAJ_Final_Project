using System;
using System.Collections.Generic;
using UnityEngine;
using AudioDetection.Interfaces;
using UAJ.Telemetry;

public class VoiceCommandManager : MonoBehaviour
{
    public static VoiceCommandManager Instance;

    private readonly Dictionary<string, IVoiceAction> commands = new();
    private readonly Dictionary<string, string> aliasToCommand = new();

    // Se mantiene por compatibilidad con llamadas existentes del proyecto.
    private string currentContext = "GLOBAL";

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
        string key = NormalizeCommand(phrase);
        if (string.IsNullOrWhiteSpace(key) || action == null)
        {
            return;
        }

        if (!commands.ContainsKey(key))
        {
            commands.Add(key, action);
            Debug.Log($"[VoiceCommandManager] Registrado comando '{key}'");
        }
        else
        {
            Debug.LogWarning($"El comando '{phrase}' ya esta registrado.");
        }
    }

    public void RegisterAlias(string alias, string primaryCommand)
    {
        string aliasKey = NormalizeCommand(alias);
        string primaryKey = NormalizeCommand(primaryCommand);

        if (string.IsNullOrWhiteSpace(aliasKey) || string.IsNullOrWhiteSpace(primaryKey))
        {
            return;
        }

        if (commands.ContainsKey(aliasKey))
        {
            Debug.LogWarning($"El alias '{alias}' no se ha registrado porque ya existe como comando principal.");
            return;
        }

        if (aliasToCommand.ContainsKey(aliasKey))
        {
            Debug.LogWarning($"El alias '{alias}' ya esta registrado (apunta a '{aliasToCommand[aliasKey]}').");
            return;
        }

        aliasToCommand.Add(aliasKey, primaryKey);
        Debug.Log($"[VoiceCommandManager] Alias registrado: '{aliasKey}' -> '{primaryKey}'");
    }

    public string ResolveCommand(string phrase)
    {
        string key = NormalizeCommand(phrase);
        return aliasToCommand.TryGetValue(key, out string primaryCommand) ? primaryCommand : key;
    }

    public void SetContext(string context)
    {
        currentContext = string.IsNullOrWhiteSpace(context) ? "GLOBAL" : context.Trim().ToUpper();
        Debug.Log($"[VoiceCommandManager] Contexto actualizado a '{currentContext}'");
    }

    public string GetCurrentContext()
    {
        return currentContext;
    }

    public void HandleCommand(string phrase)
    {
        HandleCommand(phrase, null);
    }

    public void HandleCommand(string phrase, object[] parameters)
    {
        string key = ResolveCommand(phrase);
        bool usedAlias = !string.Equals(NormalizeCommand(phrase), key, StringComparison.OrdinalIgnoreCase);

        if (commands.TryGetValue(key, out var action))
        {
            try
            {
                action.Execute(parameters);
                TrackVoiceEvent(
                    "voice_command_executed",
                    new Dictionary<string, object>
                    {
                        { "spoken_command", phrase ?? string.Empty },
                        { "resolved_command", key },
                        { "used_alias", usedAlias },
                        { "parameters", SerializeParameters(parameters) },
                        { "parameter_count", parameters?.Length ?? 0 },
                        { "scene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name },
                        { "context", GetCurrentContext() }
                    }
                );
            }
            catch (Exception e)
            {
                TrackVoiceEvent(
                    "voice_command_execution_failed",
                    new Dictionary<string, object>
                    {
                        { "spoken_command", phrase ?? string.Empty },
                        { "resolved_command", key },
                        { "used_alias", usedAlias },
                        { "parameters", SerializeParameters(parameters) },
                        { "parameter_count", parameters?.Length ?? 0 },
                        { "error_reason", e.Message },
                        { "scene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name },
                        { "context", GetCurrentContext() }
                    }
                );
                Debug.LogError($"Error al ejecutar el comando '{phrase}': {e.Message}");
            }
        }
        else
        {
            TrackVoiceEvent(
                "voice_command_execution_failed",
                new Dictionary<string, object>
                {
                    { "spoken_command", phrase ?? string.Empty },
                    { "resolved_command", key },
                    { "used_alias", usedAlias },
                    { "parameters", SerializeParameters(parameters) },
                    { "parameter_count", parameters?.Length ?? 0 },
                    { "error_reason", "command_not_registered" },
                    { "scene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name },
                    { "context", GetCurrentContext() }
                }
            );
            Debug.LogWarning($"Comando no reconocido: '{phrase}'");
        }
    }

    private static string NormalizeCommand(string command)
    {
        return string.IsNullOrWhiteSpace(command) ? string.Empty : command.Trim().ToLower();
    }

    private static string SerializeParameters(object[] parameters)
    {
        if (parameters == null || parameters.Length == 0)
        {
            return string.Empty;
        }

        return string.Join(" ", parameters);
    }

    private static void TrackVoiceEvent(string eventName, Dictionary<string, object> data)
    {
        if (Tracker.Instance.serializer == null || Tracker.Instance.persistence == null)
        {
            return;
        }

        Tracker.Instance.TrackEvent(new TrackerEvent(eventName, "VoiceCommandTracker", data));
    }
}
