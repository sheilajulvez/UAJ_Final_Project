using System;
using System.Collections.Generic;
using UnityEngine;
using AudioDetection.Interfaces;

public class VoiceLoader : MonoBehaviour
{
    public TextAsset commandFile;
    private VoiceInputEngine inputEngine;

    void Start()
    {
        var data = JsonUtility.FromJson<VoiceCommandDefinitionList>(commandFile.text);

        List<string> keywords = new();
        foreach (var def in data.definitions)
        {
            var type = Type.GetType(def.ActionClassName);
            if (type != null && typeof(IVoiceAction).IsAssignableFrom(type))
            {
                var action = (IVoiceAction)Activator.CreateInstance(type);
                VoiceCommandManager.Instance.RegisterCommand(def.Command, action);
                keywords.Add(def.Command);
            }
            else
            {
                Debug.LogWarning($"No se pudo crear instancia de {def.ActionClassName}");
            }
        }

        inputEngine = GetComponent<VoiceInputEngine>();
        inputEngine.Initialize(keywords.ToArray());
        // Adaptamos la llamada al evento para usar la versión con parámetros
        inputEngine.OnCommandRecognized += (command, parameters) =>
        {
            VoiceCommandManager.Instance.HandleCommand(command, parameters);
        };
    }
}
