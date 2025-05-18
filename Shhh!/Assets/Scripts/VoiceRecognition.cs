using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
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
            { "voice", () => Voice() },
            { "play", () => Play() },
            { "quit", () => QuitGame() },
            { "back", () => BackMenu() }
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
        GameManager.Instance.LoadScene("Menu");
    }

    private void Play()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            GameManager.Instance.LoadScene("Iglesia");
        }
    }
    private void QuitGame()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            GameManager.Instance.QuitGame();
        }
    }
    private void BackMenu()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            GameObject guia = GameObject.Find("Guia");
            if (guia != null)
            {
                guia.GetComponent<UIFade>().LlamarCorrutinaFadeOut(0);
            }
            
        }
    }
}
