using AudioDetection.Interfaces;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoiceInputEngineWhisper : BaseVoiceInputEngine
{
    private string whisperFolder => Path.Combine(Application.streamingAssetsPath, "Whisper");
    private string audioFilePath;

    public override event Action<string, object[]> OnCommandRecognized;

    [SerializeField]
    private string modelo = "ggml-tiny.en.bin";
    [SerializeField]
    private float durationSeconds = 5.0f;
    [SerializeField]
    private float waitNextCommandSeconds = 1.0f;

    private string[] commandsBase;

    private AudioClip recordingClip;
    private int sampleRate = 16000;  // whisper funciona bien con 16kHz

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
                hypothesisText.text = "Escuchando...";

            StartRecording();
            yield return WaitAndProcessRecording(durationSeconds);
        }
    }

    private void StartRecording()
    {
        if (Microphone.devices.Length == 0)
        {
            UnityEngine.Debug.LogError("No hay micr�fono disponible.");
            if (hypothesisText != null)
                hypothesisText.text = "No hay micr�fono disponible";
            return;
        }

        audioFilePath = Path.Combine(whisperFolder, "recorded.wav");
        recordingClip = Microphone.Start(null, false, Mathf.CeilToInt(durationSeconds), sampleRate);

        if (recordingClip == null)
        {
            UnityEngine.Debug.LogError("No se pudo iniciar la grabaci�n.");
            return;
        }

        UnityEngine.Debug.Log("Grabaci�n iniciada");
    }

    private IEnumerator WaitAndProcessRecording(float duration)
    {
        // Espera a que el mic termine de grabar
        while (Microphone.IsRecording(null) && Microphone.GetPosition(null) <= 0)
            yield return null;

        while (Microphone.IsRecording(null) && Microphone.GetPosition(null) < sampleRate * duration)
            yield return null;

        Microphone.End(null);

        UnityEngine.Debug.Log("Grabaci�n terminada. Guardando archivo WAV...");

        WavUtils.Save(audioFilePath, recordingClip);

        UnityEngine.Debug.Log("Archivo guardado en: " + audioFilePath);

        var whisperTask = RunWhisperDllAsync(audioFilePath);
        while (!whisperTask.IsCompleted)
            yield return null;

        string transcription = whisperTask.Result;

        ProcessTranscription(transcription);


        // Espera 1 segundo para mostrar el resultado antes de la siguiente grabaci�n
        yield return new WaitForSeconds(waitNextCommandSeconds);
    }

    private Task<string> RunWhisperDllAsync(string audioPath)
    {
        return Task.Run(() =>
        {
            string modelPath = Path.Combine(whisperFolder, "models", modelo);

            if (!File.Exists(modelPath))
            {
                UnityEngine.Debug.LogError("No se encontr� el modelo en: " + modelPath);
                return "";
            }

            if (!File.Exists(audioPath))
            {
                UnityEngine.Debug.LogError("No se encontr� el archivo de audio en: " + audioPath);
                return "";
            }

            try
            {
                return WhisperInterop.Transcribe(modelPath, audioPath);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Error al llamar a la DLL: " + e.Message);
                return "";
            }
        });
    }


    private void ProcessTranscription(string transcription)
    {
        if (string.IsNullOrEmpty(transcription) || transcription.Trim().Length < 3)
        {
            if (panelImage != null)
                panelImage.sprite = invalid;
            if (hypothesisText != null)
                hypothesisText.text = "No Reconozco el comando de Voz";
            return;
        }

        // Limpiar puntuaci�n al final, ejemplo quitando punto, coma, signo de interrogaci�n, etc.
        string phrase = transcription.ToLower().Trim().TrimEnd('.', ',', '!', '?');

        if (phrase.Length < 3)
        {
            if (panelImage != null)
                panelImage.sprite = invalid;
            if (hypothesisText != null)
                hypothesisText.text = "No Reconozco el comando de Voz";
            return;
        }

        // Opcional: filtro para palabras irrelevantes t�picas (como "you", "uh", etc)
        string[] ignoreWords = { "you", "uh", "um", "ah", "mm" };
        if (ignoreWords.Contains(phrase.ToLower()))
        {
            if (panelImage != null)
                panelImage.sprite = invalid;
            if (hypothesisText != null)
                hypothesisText.text = "No Reconozco el comando de Voz";
            return;
        }

        // Resto del c�digo para buscar comandos...
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
                panelImage.sprite = valid;
            if (hypothesisText != null)
                hypothesisText.text = phrase;

            string paramsString = phrase.Substring(matchedCommand.Length).Trim();
            object[] parameters = string.IsNullOrEmpty(paramsString)
                ? Array.Empty<object>()
                : paramsString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Cast<object>().ToArray();

            OnCommandRecognized?.Invoke(matchedCommand, parameters);
        }
        else
        {
            if (panelImage != null)
                panelImage.sprite = invalid;
            if (hypothesisText != null)
                hypothesisText.text = phrase;
            UnityEngine.Debug.LogWarning($"Comando no reconocido: '{phrase}'");
        }

        StartCoroutine(WaitBeforeNextRecognition());
    }

    private IEnumerator WaitBeforeNextRecognition()
    {
        yield return new WaitForSeconds(waitNextCommandSeconds);  // Espera para mostrar el comando
    }

}
