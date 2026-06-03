using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AudioDetection.Interfaces;
using UnityEngine;
using UAJ.Telemetry;

public class VoiceInputEngineWhisper : BaseVoiceInputEngine
{
    private string whisperFolder => Path.Combine(Application.streamingAssetsPath, "Whisper");
    private string audioFilePath;

    public override event Action<string, object[]> OnCommandRecognized;

    [SerializeField] private string modelo = "ggml-tiny.en.bin";
    [SerializeField] private float durationSeconds = 5.0f;
    [SerializeField] private float waitNextCommandSeconds = 1.0f;

    private string[] commandsBase;
    private AudioClip recordingClip;
    private int sampleRate = 16000;

    public override void Initialize(string[] commands)
    {
        commandsBase = commands;
        StartCoroutine(ContinuousRecordingAndRecognition());
    }

    private IEnumerator ContinuousRecordingAndRecognition()
    {
        while (true)
        {
            if (hypothesisText != null)
            {
                hypothesisText.text = "Escuchando...";
            }

            StartRecording();
            yield return WaitAndProcessRecording(durationSeconds);
        }
    }

    private void StartRecording()
    {
        if (Microphone.devices.Length == 0)
        {
            UnityEngine.Debug.LogError("No hay microfono disponible.");
            if (hypothesisText != null)
            {
                hypothesisText.text = "No hay microfono disponible";
            }
            return;
        }

        audioFilePath = Path.Combine(whisperFolder, "recorded.wav");
        recordingClip = Microphone.Start(null, false, Mathf.CeilToInt(durationSeconds), sampleRate);

        if (recordingClip == null)
        {
            UnityEngine.Debug.LogError("No se pudo iniciar la grabacion.");
            return;
        }

        UnityEngine.Debug.Log("Grabacion iniciada");
    }

    private IEnumerator WaitAndProcessRecording(float duration)
    {
        while (Microphone.IsRecording(null) && Microphone.GetPosition(null) <= 0)
        {
            yield return null;
        }

        while (Microphone.IsRecording(null) && Microphone.GetPosition(null) < sampleRate * duration)
        {
            yield return null;
        }

        Microphone.End(null);

        UnityEngine.Debug.Log("Grabacion terminada. Guardando archivo WAV...");
        WavUtils.Save(audioFilePath, recordingClip);
        UnityEngine.Debug.Log("Archivo guardado en: " + audioFilePath);

        var whisperTask = RunWhisperDllAsync(audioFilePath);
        while (!whisperTask.IsCompleted)
        {
            yield return null;
        }

        ProcessTranscription(whisperTask.Result);
        yield return new WaitForSeconds(waitNextCommandSeconds);
    }

    private Task<string> RunWhisperDllAsync(string audioPath)
    {
        return Task.Run(() =>
        {
            string modelPath = Path.Combine(whisperFolder, "models", modelo);

            if (!File.Exists(modelPath))
            {
                UnityEngine.Debug.LogError("No se encontro el modelo en: " + modelPath);
                return string.Empty;
            }

            if (!File.Exists(audioPath))
            {
                UnityEngine.Debug.LogError("No se encontro el archivo de audio en: " + audioPath);
                return string.Empty;
            }

            try
            {
                return WhisperInterop.Transcribe(modelPath, audioPath);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Error al llamar a la DLL: " + e.Message);
                return string.Empty;
            }
        });
    }

    private void ProcessTranscription(string transcription)
    {
        if (string.IsNullOrEmpty(transcription) || transcription.Trim().Length < 3)
        {
            if (panelImage != null)
            {
                panelImage.sprite = invalid;
            }

            if (hypothesisText != null)
            {
                hypothesisText.text = "No Reconozco el comando de Voz";
            }

            TrackVoiceRecognitionEvent("voice_command_not_recognized", transcription, string.Empty, null);
            return;
        }

        string phrase = transcription.ToLower().Trim().TrimEnd('.', ',', '!', '?');
        if (phrase.Length < 3)
        {
            if (panelImage != null)
            {
                panelImage.sprite = invalid;
            }

            if (hypothesisText != null)
            {
                hypothesisText.text = "No Reconozco el comando de Voz";
            }

            TrackVoiceRecognitionEvent("voice_command_not_recognized", phrase, string.Empty, null);
            return;
        }

        string[] ignoreWords = { "you", "uh", "um", "ah", "mm" };
        if (ignoreWords.Contains(phrase))
        {
            if (panelImage != null)
            {
                panelImage.sprite = invalid;
            }

            if (hypothesisText != null)
            {
                hypothesisText.text = "No Reconozco el comando de Voz";
            }

            TrackVoiceRecognitionEvent("voice_command_not_recognized", phrase, string.Empty, null);
            return;
        }

        string matchedCommand = null;
        foreach (var cmd in commandsBase)
        {
            if (phrase.StartsWith(cmd, StringComparison.OrdinalIgnoreCase))
            {
                matchedCommand = cmd;
                break;
            }
        }

        if (matchedCommand != null)
        {
            if (panelImage != null)
            {
                panelImage.sprite = valid;
            }

            if (hypothesisText != null)
            {
                hypothesisText.text = phrase;
            }

            string paramsString = phrase.Substring(matchedCommand.Length).Trim();
            object[] parameters = string.IsNullOrEmpty(paramsString)
                ? Array.Empty<object>()
                : paramsString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Cast<object>().ToArray();

            TrackVoiceRecognitionEvent("voice_command_recognized", phrase, matchedCommand, parameters);
            OnCommandRecognized?.Invoke(matchedCommand, parameters);
        }
        else
        {
            if (panelImage != null)
            {
                panelImage.sprite = invalid;
            }

            if (hypothesisText != null)
            {
                hypothesisText.text = phrase;
            }

            TrackVoiceRecognitionEvent("voice_command_not_recognized", phrase, string.Empty, null);
            UnityEngine.Debug.LogWarning($"Comando no reconocido: '{phrase}'");
        }

        StartCoroutine(WaitBeforeNextRecognition());
    }

    private IEnumerator WaitBeforeNextRecognition()
    {
        yield return new WaitForSeconds(waitNextCommandSeconds);
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
                    { "engine", nameof(VoiceInputEngineWhisper) },
                    { "scene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name },
                    { "context", VoiceCommandManager.Instance != null ? VoiceCommandManager.Instance.GetCurrentContext() : "UNKNOWN" }
                }
            )
        );
    }
}
