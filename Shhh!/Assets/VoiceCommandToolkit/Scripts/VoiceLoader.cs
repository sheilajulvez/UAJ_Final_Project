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
        switch (engineType)
        {
            case VoiceEngineType.Whisper:
                inputEngine = GetComponent<VoiceInputEngineWhisper>();
                if (inputEngine == null)
                {
                    inputEngine = gameObject.AddComponent<VoiceInputEngineWhisper>();
                }
                break;

            case VoiceEngineType.Windows:
                inputEngine = GetComponent<VoiceInputEngineWindows>();
                if (inputEngine == null)
                {
                    inputEngine = gameObject.AddComponent<VoiceInputEngineWindows>();
                }
                break;
        }

        var data = JsonUtility.FromJson<VoiceCommandDefinitionList>(commandFile.text);

        List<string> keywords = new();
        HashSet<string> uniqueKeywords = new(StringComparer.OrdinalIgnoreCase);

        foreach (var def in data.definitions)
        {
            var type = Type.GetType(def.ActionClassName);
            if (type != null && typeof(IVoiceAction).IsAssignableFrom(type))
            {
                var action = (IVoiceAction)Activator.CreateInstance(type);

                VoiceCommandManager.Instance.RegisterCommand(def.Command, action);
                AddKeyword(def.Command, uniqueKeywords, keywords);

                if (def.Aliases != null)
                {
                    foreach (var alias in def.Aliases)
                    {
                        string trimmedAlias = alias?.Trim();
                        if (string.IsNullOrWhiteSpace(trimmedAlias))
                        {
                            continue;
                        }

                        VoiceCommandManager.Instance.RegisterAlias(trimmedAlias, def.Command);
                        AddKeyword(trimmedAlias, uniqueKeywords, keywords);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"No se pudo crear instancia de {def.ActionClassName}");
            }
        }

        inputEngine.Initialize(keywords.ToArray());

        inputEngine.OnCommandRecognized += (command, parameters) =>
        {
            VoiceCommandManager.Instance.HandleCommand(command, parameters);
        };
    }

    private static void AddKeyword(string value, HashSet<string> uniqueKeywords, List<string> keywords)
    {
        string normalized = value?.Trim().ToLower();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return;
        }

        if (uniqueKeywords.Add(normalized))
        {
            keywords.Add(normalized);
        }
    }
}
