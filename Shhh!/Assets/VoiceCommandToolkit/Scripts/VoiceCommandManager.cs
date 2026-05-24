using System;
using System.Collections.Generic;
using UnityEngine;
using AudioDetection.Interfaces;

public class VoiceCommandManager : MonoBehaviour
{
    public static VoiceCommandManager Instance;

    // Diccionario principal: comando normalizado -> accion
    private readonly Dictionary<string, IVoiceAction> commands = new();

    // Diccionario de aliases: alias normalizado -> comando principal normalizado
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

    /// <summary>
    /// Registra un comando principal y su accion asociada.
    /// </summary>
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

    /// <summary>
    /// Registra un alias que apunta al comando principal indicado.
    /// </summary>
    public void RegisterAlias(string alias, string primaryCommand)
    {
        string aliasKey = NormalizeCommand(alias);
        string primaryKey = NormalizeCommand(primaryCommand);

        if (string.IsNullOrWhiteSpace(aliasKey) || string.IsNullOrWhiteSpace(primaryKey))
        {
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

    /// <summary>
    /// Devuelve el comando principal asociado a una frase.
    /// </summary>
    public string ResolveCommand(string phrase)
    {
        string key = NormalizeCommand(phrase);
        return aliasToCommand.TryGetValue(key, out string primaryCommand) ? primaryCommand : key;
    }

    // Compatibilidad con el flujo actual del juego.
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

        if (commands.TryGetValue(key, out var action))
        {
            try
            {
                action.Execute(parameters);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error al ejecutar el comando '{phrase}': {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"Comando no reconocido: '{phrase}'");
        }
    }

    private static string NormalizeCommand(string command)
    {
        return string.IsNullOrWhiteSpace(command) ? string.Empty : command.Trim().ToLower();
    }
}
