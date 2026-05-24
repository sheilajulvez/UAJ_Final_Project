using System;
using System.Collections.Generic;
using UnityEngine;
using AudioDetection.Interfaces;

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
}
