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
           // { "voice", () => Voice() },
            { "play", () => Play() },
            { "quit", () => QuitGame() },
            { "microphone", () => GoSettings() },
            { "back", () => BackMenu() },
            //{ "classic", () => Classic() },
            { "help", () => Controls() },
            { "move", () => MoveForward() },
            { "stop", () => Stop() },
            { "right", () => RotateRight() },
            { "left", () => RotateLeft() },
             { "up", () => RotateUp() },
            { "down", () => RotateDown() },
            { "pick", () => CheckArea() },

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
        Debug.Log("HELP");
            GameObject control = GameObject.Find("Control");
            if (control != null)
            {
                control.GetComponent<Button>().onClick.Invoke();
            }
        
    }

    private void MoveForward()
    {
       
            GameManager.Instance.AvanzaPersonaje();
        
    }

    private void RotateRight()
    {
      
            GameManager.Instance.RotaDerechaPersonaje();
        
    }

    private void RotateUp()
    {
       
            GameManager.Instance.RotaArribaPersonaje();
        
    }
    private void RotateDown()
    {

        GameManager.Instance.RotaAbajoPersonaje();

    }

    private void RotateLeft()
    {

        GameManager.Instance.RotaIzquierdaPersonaje();

    }

    private void Stop()
    {
       
            //GameManager.Instance.RetrocedePersonaje();
        
    }
    private void CheckArea()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject starCoin = GameObject.Find("StarCoin");

        if (player != null && starCoin != null)
        {
            Collider triggerCollider = starCoin.GetComponent<Collider>();

            if (triggerCollider != null && triggerCollider.isTrigger)
            {
                if (triggerCollider.bounds.Contains(player.transform.position))
                {
                    switch (SceneManager.GetActiveScene().name)
                    {
                        case "Retiro":
                            GameManager.Instance.LoadScene("Iglesia");
                            break;
                        case "Iglesia":
                            GameManager.Instance.LoadScene("Cine");
                            break;
                        case "Mina":
                            GameManager.Instance.LoadScene("Retiro");
                            break;
                        case "Cine":
                            GameManager.Instance.LoadScene("Mina");
                            break;
                        default:
                            Debug.LogWarning("Escena no contemplada para la transición.");
                            break;
                    }
                    Debug.Log("nextscene"); // Aquí se puede cambiar por el nombre real si se quiere
                }
            }
            else
            {
                Debug.LogWarning("El collider de StarCoin no está marcado como Trigger.");
            }
        }
        else
        {
            Debug.LogError("No se encontró el jugador o el objeto StarCoin.");
        }

    }
}
