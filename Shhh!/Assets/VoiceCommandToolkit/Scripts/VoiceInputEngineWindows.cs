using System;
using System.Collections;
using System.Linq;
using AudioDetection.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UAJ.Telemetry;

public class VoiceInputEngineWindows : BaseVoiceInputEngine
{
    private DictationRecognizer dictationRecognizer;
    private string[] commandsBase;

    public override event Action<string, object[]> OnCommandRecognized;

    public override void Initialize(string[] commands)
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

        string matchedCommand = FindBestMatchingCommand(phrase);

        if (!string.IsNullOrEmpty(matchedCommand))
        {
            Debug.Log($"[VoiceInputEngine] Comando base detectado: '{matchedCommand}'");

            if (hypothesisText != null)
            {
                hypothesisText.text = text;
            }

            if (panelImage != null)
            {
                panelImage.sprite = valid;
            }
        }
        else
        {
            if (hypothesisText != null)
            {
                hypothesisText.text = text;
            }

            if (panelImage != null)
            {
                panelImage.sprite = invalid;
            }
        }

        if (matchedCommand == null)
        {
            TrackVoiceRecognitionEvent(
                "voice_command_not_recognized",
                phrase,
                string.Empty,
                null
            );
            Debug.LogWarning($"[VoiceInputEngine] Comando no reconocido en frase: '{phrase}'");
            return;
        }

        string paramsString = phrase.Substring(matchedCommand.Length).Trim();
        Debug.Log($"[VoiceInputEngine] Parametros extraidos como texto: '{paramsString}'");

        object[] parameters;
        if (string.IsNullOrEmpty(paramsString))
        {
            parameters = Array.Empty<object>();
            Debug.Log("[VoiceInputEngine] No se detectaron parametros.");
        }
        else
        {
            var splitParams = paramsString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            parameters = splitParams.Cast<object>().ToArray();
            Debug.Log($"[VoiceInputEngine] Parametros separados: {string.Join(", ", splitParams)}");
        }

        Debug.Log($"[VoiceInputEngine] Lanzando evento OnCommandRecognized con comando '{matchedCommand}' y {parameters.Length} parametros.");
        TrackVoiceRecognitionEvent(
            "voice_command_recognized",
            phrase,
            matchedCommand,
            parameters
        );
        OnCommandRecognized?.Invoke(matchedCommand, parameters);
    }

    private void DictationRecognizer_DictationHypothesis(string text)
    {
        Debug.Log($"[VoiceInputEngine] Hipotesis dictation: '{text}'");

        if (hypothesisText != null)
        {
            hypothesisText.text = "Escuchando: " + text;
        }
    }

    private bool isRestarting;

    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        Debug.Log($"[VoiceInputEngine] Dictation complete: {cause}");
        if (!isRestarting)
        {
            Debug.LogWarning("[VoiceInputEngine] Dictation finalizo inesperadamente. Reiniciando...");

            isRestarting = true;
            dictationRecognizer.Stop();
            dictationRecognizer.Dispose();
            dictationRecognizer = null;

            StartCoroutine(RestartDictationAfterDelay(1f));
        }
    }

    private IEnumerator RestartDictationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

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

        const int ErrorDictationDisabled = unchecked((int)0x80045509);
        if (hresult == ErrorDictationDisabled)
        {
            Debug.LogError("El reconocimiento por voz no esta activado en el sistema. Ve a Configuracion > Privacidad > Voz y activa el reconocimiento en linea.");
            OpenVoiceSettings();
        }
    }

    private void OpenVoiceSettings()
    {
        try
        {
            System.Diagnostics.Process.Start("ms-settings:privacy-speech");
        }
        catch (Exception e)
        {
            Debug.LogWarning("No se pudo abrir la configuracion de privacidad de voz: " + e.Message);
        }
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
            {
                dictationRecognizer.Stop();
            }

            dictationRecognizer.Dispose();
            dictationRecognizer = null;
        }
    }

    private string FindBestMatchingCommand(string phrase)
    {
        string bestMatch = null;

        foreach (var cmd in commandsBase)
        {
            Debug.Log($"[VoiceInputEngine] Comprobando si '{phrase}' empieza con '{cmd}'...");
            if (!phrase.StartsWith(cmd, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (bestMatch == null || cmd.Length > bestMatch.Length)
            {
                bestMatch = cmd;
            }
        }

        return bestMatch;
    }

    private static void TrackVoiceRecognitionEvent(string eventName, string rawPhrase, string matchedCommand, object[] parameters)
    {
        if (Tracker.Instance.serializer == null || Tracker.Instance.persistence == null)
        {
            return;
        }

        Tracker.Instance.TrackEvent(
            new TrackerEvent(
                eventName,
                "VoiceCommandTracker",
                new System.Collections.Generic.Dictionary<string, object>
                {
                    { "raw_phrase", rawPhrase ?? string.Empty },
                    { "matched_command", matchedCommand ?? string.Empty },
                    { "parameters", parameters == null || parameters.Length == 0 ? string.Empty : string.Join(" ", parameters) },
                    { "parameter_count", parameters?.Length ?? 0 },
                    { "engine", nameof(VoiceInputEngineWindows) },
                    { "scene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name },
                    { "context", VoiceCommandManager.Instance != null ? VoiceCommandManager.Instance.GetCurrentContext() : "UNKNOWN" }
                }
            )
        );
    }
}
