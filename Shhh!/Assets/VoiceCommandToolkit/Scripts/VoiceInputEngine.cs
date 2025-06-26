using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class VoiceInputEngine : MonoBehaviour
{
    private DictationRecognizer dictationRecognizer;
    private string[] commandsBase;
    public TextMeshProUGUI hypothesisText;
    
    // Evento con comando y parámetros
    public event Action<string, object[]> OnCommandRecognized;

    /// <summary>
    /// Inicializa el dictation recognizer con los comandos base para parseo.
    /// </summary>
    /// <param name="commands">Lista de comandos base (ej: "activar luces")</param>
    public void Initialize(string[] commands)
    {
        if (commands == null || commands.Length == 0)
        {
            Debug.LogWarning("No se proporcionaron comandos para reconocimiento.");
            return;
        }

        commandsBase = commands;

        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
        dictationRecognizer.DictationError += DictationRecognizer_DictationError;

        dictationRecognizer.Start();

        Debug.Log("[VoiceInputEngine] DictationRecognizer iniciado para reconocimiento libre.");
    }

    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log("cheking");
        string phrase = text.Trim();
        Debug.Log($"[VoiceInputEngine] Dictation resultado: '{phrase}' (confianza: {confidence})");

        string matchedCommand = null;

        foreach (var cmd in commandsBase)
        {
            Debug.Log($"[VoiceInputEngine] Comprobando si '{phrase}' empieza con '{cmd}'...");
            if (phrase.StartsWith(cmd, StringComparison.OrdinalIgnoreCase))
            {
                matchedCommand = cmd;
                Debug.Log($"[VoiceInputEngine] Comando base detectado: '{matchedCommand}'");
                break;
            }
        }

        if (matchedCommand == null)
        {
            Debug.LogWarning($"[VoiceInputEngine] Comando no reconocido en frase: '{phrase}'");
            return;
        }

        string paramsString = phrase.Substring(matchedCommand.Length).Trim();
        Debug.Log($"[VoiceInputEngine] Parámetros extraídos como texto: '{paramsString}'");

        object[] parameters;

        if (string.IsNullOrEmpty(paramsString))
        {
            parameters = Array.Empty<object>();
            Debug.Log("[VoiceInputEngine] No se detectaron parámetros.");
        }
        else
        {
            var splitParams = paramsString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            parameters = splitParams.Cast<object>().ToArray();
            Debug.Log($"[VoiceInputEngine] Parámetros separados: {string.Join(", ", splitParams)}");
        }

        Debug.Log($"[VoiceInputEngine] Lanzando evento OnCommandRecognized con comando '{matchedCommand}' y {parameters.Length} parámetros.");
        OnCommandRecognized?.Invoke(matchedCommand, parameters);
    }

    private void DictationRecognizer_DictationHypothesis(string text)
    {
        Debug.Log($"[VoiceInputEngine] Hipótesis dictation: '{text}'");

        if (hypothesisText != null)
        {
            hypothesisText.text = "Escuchando: " + text;
        }
        // Llamamos al mismo método que procesa el resultado,
        // pero con un nivel de confianza medio para distinguir
        DictationRecognizer_DictationResult(text, ConfidenceLevel.Medium);
    }


    private bool isRestarting = false;

    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        Debug.Log($"[VoiceInputEngine] Dictation complete: {cause}");
        if (cause != DictationCompletionCause.Complete && !isRestarting)
        {
            Debug.LogWarning("[VoiceInputEngine] Dictation finalizó inesperadamente. Reiniciando...");

            isRestarting = true;

            // Parar y liberar la instancia actual
            dictationRecognizer.Stop();
            dictationRecognizer.Dispose();
            dictationRecognizer = null;

            // Reiniciar con un pequeño retraso para evitar conflictos
            StartCoroutine(RestartDictationAfterDelay(1f));
        }
    }

    private IEnumerator RestartDictationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Recrear el DictationRecognizer y volver a inicializarlo
        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
        dictationRecognizer.DictationError += DictationRecognizer_DictationError;

        dictationRecognizer.Start();

        Debug.Log("[VoiceInputEngine] DictationRecognizer reiniciado.");

        isRestarting = false;
    }



    private void DictationRecognizer_DictationError(string error, int hresult)
    {
        Debug.LogError($"[VoiceInputEngine] Dictation error: {error}; HResult = {hresult}");
    }

    private void OnDestroy()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.DictationResult -= DictationRecognizer_DictationResult;
            dictationRecognizer.DictationHypothesis -= DictationRecognizer_DictationHypothesis;
            dictationRecognizer.DictationComplete -= DictationRecognizer_DictationComplete;
            dictationRecognizer.DictationError -= DictationRecognizer_DictationError;

            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
                dictationRecognizer.Stop();

            dictationRecognizer.Dispose();
            dictationRecognizer = null;
        }
    }
}
