using UnityEngine;
using UnityEngine.Windows.Speech;
using System;
using System.Linq;

public class DictationVoiceInput : MonoBehaviour
{
    private DictationRecognizer dictationRecognizer;

    // Evento para pasar comando y parámetros
    public event Action<string, object[]> OnCommandRecognized;

    // Lista de comandos base (ejemplo)
    private string[] commandBases = new string[] { "go to", "activate lights", "turn off" };

    void Start()
    {
        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.Log($"[Dictation] Texto reconocido: '{text}'");

            ProcessPhrase(text);
        };

        dictationRecognizer.DictationHypothesis += (text) =>
        {
            Debug.Log($"[Dictation] Hipótesis: {text}");
        };

        dictationRecognizer.DictationComplete += (completionCause) =>
        {
            Debug.Log($"[Dictation] Reconocimiento completado: {completionCause}");

            if (completionCause != DictationCompletionCause.Complete)
            {
                Debug.LogWarning("El reconocimiento no finalizó correctamente, reiniciando...");
                dictationRecognizer.Start();
            }
        };

        dictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogError($"[Dictation] Error: {error}; Código: {hresult}");
        };

        dictationRecognizer.Start();
        Debug.Log("DictationRecognizer iniciado.");
    }

    private void ProcessPhrase(string phrase)
    {
        phrase = phrase.ToLower().Trim();

        // Buscamos comando base que coincida al inicio
        string matchedCommand = null;
        string paramsString = "";

        foreach (var cmd in commandBases)
        {
            if (phrase.StartsWith(cmd))
            {
                matchedCommand = cmd;
                paramsString = phrase.Substring(cmd.Length).Trim();
                break;
            }
        }

        if (matchedCommand == null)
        {
            Debug.LogWarning($"Comando no reconocido en frase: '{phrase}'");
            return;
        }

        object[] parameters = string.IsNullOrEmpty(paramsString)
            ? Array.Empty<object>()
            : paramsString.Split(' ').Cast<object>().ToArray();

        Debug.Log($"Comando detectado: '{matchedCommand}', parámetros: {string.Join(", ", parameters)}");

        OnCommandRecognized?.Invoke(matchedCommand, parameters);
    }

    private void OnDestroy()
    {
        if (dictationRecognizer != null)
        {
            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
                dictationRecognizer.Stop();

            dictationRecognizer.Dispose();
            dictationRecognizer = null;
        }
    }
}
