using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceInputEngine : MonoBehaviour { // Clase que inicializa el KeywordRecognizer
    private KeywordRecognizer recognizer;
    private string[] keywords;

    public event Action<string> OnCommandRecognized;

    public void Initialize(string[] commands) {
        if (commands == null || commands.Length == 0) {
            Debug.LogWarning("No se proporcionaron comandos para reconocimiento.");
            return;
        }

        keywords = commands;
        recognizer = new KeywordRecognizer(keywords);
        recognizer.OnPhraseRecognized += args => OnCommandRecognized?.Invoke(args.text);
        recognizer.Start();

        Debug.Log("Reconocimiento por voz iniciado con comandos: " + string.Join(", ", keywords));
    }

    private void OnDestroy() {
        if (recognizer != null && recognizer.IsRunning) {
            recognizer.Stop();
            recognizer.Dispose();
        }
    }
}