using System;
using System.Collections.Generic;
using UnityEngine;
using AudioDetection.Interfaces;

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

            // Por si el commando no tiene contexto definido
            if (Contexts.Count == 0)
            {
                Contexts.Add("GLOBAL");
            }
        }
    }

    private Dictionary<string, RegisteredVoiceCommand> commands = new();

    [SerializeField] private bool useContextFiltering = true;

    private string currentContext = "SELECTION";

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
            Debug.LogWarning($"El comando '{phrase}' ya está registrado.");
        }
    }


    public void SetContext(string context)
    {
        currentContext = NormalizeContext(context);
        Debug.Log($"[VoiceCommandManager] Contexto actual cambiado a: {currentContext}");
    }

    public string GetCurrentContext()
    {
        return currentContext;
    }


    public void HandleCommand(string phrase, object[] parameters)
    {
        string key = NormalizeCommand(phrase);

        if (commands.TryGetValue(key, out var registeredCommand))
        {
            if (useContextFiltering && !IsCommandAllowedInCurrentContext(registeredCommand))
            {
                Debug.LogWarning($"[VoiceCommandManager] Comando '{phrase}' reconocido pero no válido en el contexto actual: {currentContext}");
                return;
            }

            try
            {
                registeredCommand.Action.Execute(parameters);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error al ejecutar el comando '{phrase}': {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"Comando no reconocido: {phrase}");
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
}