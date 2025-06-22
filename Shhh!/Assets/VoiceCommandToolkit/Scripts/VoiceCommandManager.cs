using System;
using System.Collections.Generic;
using UnityEngine;

public class VoiceCommandManager : MonoBehaviour { // Clase que se encarga de registrar y ejecutar los comandos
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

    public void RegisterCommand(string phrase, IVoiceAction action) {
        string key = phrase.ToLower();
        if (!commands.ContainsKey(key)) {
            commands.Add(key, action);
        } else {
            Debug.LogWarning($"El comando '{phrase}' ya está registrado.");
        }
    }

    public void HandleCommand(string phrase) {
        string key = phrase.ToLower();
        if (commands.TryGetValue(key, out var action)) {
            action.Execute();
        } else {
            Debug.LogWarning($"Comando no reconocido: {phrase}");
        }
    }
}
