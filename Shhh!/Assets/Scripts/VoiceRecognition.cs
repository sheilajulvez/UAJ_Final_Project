using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;

public class VoiceRecognition : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer;

    Dictionary<string, Action> wordToAction;

    void Start()
    {
        DontDestroyOnLoad(gameObject); // No destruir al cambiar de escena

        wordToAction = new Dictionary<string, Action>
        {
            { "park", () => CambioDeEscena("Retiro") },
            { "mine", () => CambioDeEscena("Mina") },
            { "cinema", () => CambioDeEscena("Cine") },
            { "church", () => CambioDeEscena("Iglesia") },
            { "voice", () => Voice() }
        };


        keywordRecognizer = new KeywordRecognizer(wordToAction.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += WordRecognized;
        keywordRecognizer.Start();
    }

    private void WordRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log($"Comando reconocido: {args.text} (Confianza: {args.confidence})");

        if (wordToAction.ContainsKey(args.text))
        {
            if (GameManager.Instance.GetVoiceRecogniser())
            {
                wordToAction[args.text].Invoke();
            }
            else
            {
                Debug.Log("Reconocimentoo de voz está desactivado");
            }
        }
        else
        {
            Debug.LogWarning("Palabra reconocida no está registrada en el diccionario.");
        }
    }


    private void CambioDeEscena(string scene)
    {
        GameManager.Instance.LoadScene(scene);
    }

    private void Voice()
    {
        
    }
}
