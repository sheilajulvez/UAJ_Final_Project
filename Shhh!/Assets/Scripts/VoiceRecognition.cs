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

    GameObject controlSave;

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
            { "back", () => BackMenu() },
            { "classic", () => Classic() },
            { "help", () => Controls() },
            { "move", () => MoveForward() },
            { "move forward", () => MoveForward() },
            { "stop", () => Stop() },
            { "right", () => RotateRight() },
            { "left", () => RotateLeft() },
            { "look up", () => RotateUp() },
            { "look down", () => RotateDown() },
            { "up", () => RotateUp() },
            { "down", () => RotateDown() },
            { "pick", () => CheckArea() },
            { "coin", () => CheckArea() },

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
        if (n == "Retiro" || n == "Cine" || n == "Iglesia" || n == "Mina")
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
        else
        {
            GameManager.Instance.QuitGame();
        }
    }
    private void BackMenu()
    {
        if (SceneManager.GetActiveScene().name != "Selection" && SceneManager.GetActiveScene().name != "Victory")
        {
            GameObject back = GameObject.Find("Back");
            if (back != null)
            {
                back.GetComponent<Button>().onClick.Invoke();
                controlSave.SetActive(true);
            }
            else
            {
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
        if (SceneManager.GetActiveScene().name != "Selection" && SceneManager.GetActiveScene().name != "Victory")
        {
            GameObject control = GameObject.Find("Control");
            if (control != null)
            {
                control.GetComponent<Button>().onClick.Invoke();
                control.SetActive(false);
                controlSave = control;
            }
        }
    }

    private void MoveForward()
    {
        string n = SceneManager.GetActiveScene().name;
        if (n == "Retiro" || n == "Cine" || n == "Iglesia" || n == "Mina")
        {
            GameManager.Instance.AvanzaPersonaje();
        }
    }

    private void RotateRight()
    {
        string n = SceneManager.GetActiveScene().name;
        if (n == "Retiro" || n == "Cine" || n == "Iglesia" || n == "Mina")
        {
            GameManager.Instance.RotaDerechaPersonaje();
        }
    }

    private void RotateUp()
    {
        string n = SceneManager.GetActiveScene().name;
        if (n == "Retiro" || n == "Cine" || n == "Iglesia" || n == "Mina")
        {
            GameManager.Instance.RotaArribaPersonaje();
        }
    }
    private void RotateDown()
    {
        string n = SceneManager.GetActiveScene().name;
        if (n == "Retiro" || n == "Cine" || n == "Iglesia" || n == "Mina")
        {
            GameManager.Instance.RotaAbajoPersonaje();
        }
    }

    private void RotateLeft()
    {
        string n = SceneManager.GetActiveScene().name;
        if (n == "Retiro" || n == "Cine" || n == "Iglesia" || n == "Mina")
        {
            GameManager.Instance.RotaIzquierdaPersonaje();
        }
    }

    private void Stop()
    {
        string n = SceneManager.GetActiveScene().name;
        if (n == "Retiro" || n == "Cine" || n == "Iglesia" || n == "Mina")
        {
            //GameManager.Instance.RetrocedePersonaje();
        }
    }
    private void CheckArea()
    {
        string n = SceneManager.GetActiveScene().name;
        if (n == "Retiro" || n == "Cine" || n == "Iglesia" || n == "Mina")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            GameObject starCoin = GameObject.Find("StarCoin");

            

            if (player != null && starCoin != null)
            {
                AudioSource[] sources = player.GetComponents<AudioSource>();

                

                Collider triggerCollider = starCoin.GetComponent<Collider>();

                if (triggerCollider != null && triggerCollider.isTrigger)
                {
                    if (triggerCollider.bounds.Contains(player.transform.position))
                    {
                        foreach (AudioSource source in sources)
                        {
                            if (source.clip != null && source.clip.name == "coin")
                            {
                                source.Play();
                                break;
                            }
                        }

                        GameManager.Instance.SetCoinsCollected(GameManager.Instance.GetCoinsCollected() + 1);
                        if (GameManager.Instance.GetCoinsCollected() < 4)
                        {
                            switch (n)
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
                        foreach (AudioSource source in sources)
                        {
                            if (source.clip != null && source.clip.name == "empty")
                            {
                                source.Play();
                                break;
                            }
                        }
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
}
