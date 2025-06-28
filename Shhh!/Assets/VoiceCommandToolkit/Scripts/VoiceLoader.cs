using System;
using System.Collections.Generic;
using UnityEngine;
using AudioDetection.Interfaces;

public class VoiceLoader : MonoBehaviour
{
    public enum VoiceEngineType
    {
        Whisper,
        Windows
    }

    [SerializeField]
    private VoiceEngineType engineType = VoiceEngineType.Whisper;

    private IVoiceInputEngine inputEngine;

    public TextAsset commandFile;

    void Start()
    {
        // Según la opción elegida, obtenemos o añadimos el componente correspondiente
        switch (engineType)
        {
            case VoiceEngineType.Whisper:
                inputEngine = GetComponent<VoiceInputEngineWhisper>();
                if (inputEngine == null)
                    inputEngine = gameObject.AddComponent<VoiceInputEngineWhisper>();
                break;

            case VoiceEngineType.Windows:
                inputEngine = GetComponent<VoiceInputEngineWindows>();
                if (inputEngine == null)
                    inputEngine = gameObject.AddComponent<VoiceInputEngineWindows>();
                break;
        }

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

        inputEngine.Initialize(keywords.ToArray());
        // Adaptamos la llamada al evento para usar la versión con parámetros
        inputEngine.OnCommandRecognized += (command, parameters) =>
        {
            VoiceCommandManager.Instance.HandleCommand(command, parameters);
        };
    }
}
