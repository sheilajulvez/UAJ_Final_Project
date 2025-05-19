using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class VoiceRecognition : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer;

    Dictionary<string, Action> wordToAction;

    [SerializeField] GameObject guia;

    void Start()
    {
        DontDestroyOnLoad(gameObject); // No destruir al cambiar de escena

        wordToAction = new Dictionary<string, Action>
        {
            { "go park", () => CambioDeEscena("Retiro") },
            { "go mine", () => CambioDeEscena("Mina") },
            { "go cinema", () => CambioDeEscena("Cine") },
            { "go church", () => CambioDeEscena("Iglesia") },
            { "voice", () => Voice() },
            { "play", () => Play() },
            { "quit", () => QuitGame() },
            { "microphone", () => GoSettings() },
            { "exit", () => BackMenu() },
            { "classic", () => Classic() },
            { "help", () => Controls() },
            { "move forward", () => MoveForward() },
            { "left", () => RotateLeft() },
            { "right", () => RotateRight() },
            { "stop", () => Stop() },
            { "check area", () => CheckArea() }
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
        string n = SceneManager.GetActiveScene().name;
        if (n == "Iglesia" || n == "Mina" || n == "Cine" || n == "Retiro")
        {
            GameManager.Instance.LoadScene(scene);
        }
    }

    private void Voice()
    {
        if (SceneManager.GetActiveScene().name == "Selection")
        {
            GameManager.Instance.LoadScene("Menu");
        }
        else if (SceneManager.GetActiveScene().name == "Menu")
        {
            GameObject voice = GameObject.Find("Voice");
            if (voice != null)
            {
                voice.GetComponent<Button>().onClick.Invoke();
            }

        }
    }

    private void Play()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.Invoke();
        }
    }
    private void QuitGame()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            GameObject.Find("QuitButton").GetComponent<Button>().onClick.Invoke();
        }
    }
    private void BackMenu()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            GameObject back = GameObject.Find("Back");
            if (back != null)
            {
                back.GetComponent<Button>().onClick.Invoke();
            }
            else {
                GameObject hide = GameObject.Find("Hide");
                if (hide != null)
                {
                    hide.GetComponent<Button>().onClick.Invoke();
                }
            }
        }
    }

    private void GoSettings()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            GameObject micro = GameObject.Find("Microphone");
            if (micro != null)
            {
                micro.GetComponent<Button>().onClick.Invoke();
            }
        }
    }
    private void Classic()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            GameObject classic = GameObject.Find("Classic");
            if (classic != null)
            {
                classic.GetComponent<Button>().onClick.Invoke();
            }
        }
    }

    private void Controls()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            GameObject control = GameObject.Find("Control");
            if (control != null)
            {
                control.GetComponent<Button>().onClick.Invoke();
            }
        }
    }

    private void MoveForward()
    {
        string n = SceneManager.GetActiveScene().name;
        if (n == "Iglesia" || n == "Mina" || n == "Cine" || n == "Retiro")
        {
            // TO DO
        }
    }

    private void RotateRight()
    {
        string n = SceneManager.GetActiveScene().name;
        if (n == "Iglesia" || n == "Mina" || n == "Cine" || n == "Retiro")
        {
            // TO DO
        }
    }

    private void RotateLeft()
    {
        string n = SceneManager.GetActiveScene().name;
        if (n == "Iglesia" || n == "Mina" || n == "Cine" || n == "Retiro")
        {
            // TO DO
        }
    }

    private void Stop()
    {
        string n = SceneManager.GetActiveScene().name;
        if (n == "Iglesia" || n == "Mina" || n == "Cine" || n == "Retiro")
        {
            // TO DO
        }
    }
    private void CheckArea()
    {
        string n = SceneManager.GetActiveScene().name;
        if (n == "Iglesia" || n == "Mina" || n == "Cine" || n == "Retiro")
        {
            // TO DO
        }
    }
}
