using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool voiceRecogniser = true;
    private GameObject Player;
    public string[] sceneNames;
    private int coinsCollected = 0;

    private string currentScene = "";

    private void Awake()
    {
        // Implementar Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No destruir al cambiar de escena
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // Evitar duplicados
        }
    }
   

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckArea();
        }
        // Al pulsar Shift Izquierdo
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Cursor.lockState = CursorLockMode.None; // Libera el cursor
            Cursor.visible = true;                  // Muestra el cursor
        }

        // Al soltar Shift Izquierdo
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor
            Cursor.visible = false;                   // Oculta el cursor
        }
        
    }

    public void CheckArea()
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

                        SetCoinsCollected(GetCoinsCollected() + 1);
                        if (GetCoinsCollected() < 4)
                        {
                            switch (n)
                            {
                                case "Retiro":
                                    LoadScene("Iglesia");
                                    break;
                                case "Iglesia":
                                    LoadScene("Cine");
                                    break;
                                case "Mina":
                                    LoadScene("Retiro");
                                    break;
                                case "Cine":
                                    LoadScene("Mina");
                                    break;
                                default:
                                    Debug.LogWarning("Escena no contemplada para la transici�n.");
                                    break;
                            }
                            Debug.Log("nextscene"); // Aqu� se puede cambiar por el nombre real si se quiere
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
                    Debug.LogWarning("El collider de StarCoin no est� marcado como Trigger.");
                }
            }
            else
            {
                Debug.LogError("No se encontr� el jugador o el objeto StarCoin.");
            }
        }
    }




    public void SetCoinsCollected(int newValue)
    {
        coinsCollected = newValue;
        Debug.Log(coinsCollected);
        if (coinsCollected == 4)
        {
            LoadScene("Victory");
        }
    }

    public int GetCoinsCollected()
    {
        return coinsCollected;
    }

    public void ClassicGameMode()
    {
        Debug.Log("Recogniser desactivado");
        voiceRecogniser = false;
        GameObject.Find("VoiceManager").SetActive(false);
        LoadScene("Menu");
    }

    public bool GetVoiceRecogniser()
    {
        return voiceRecogniser;
    }

    public void SetPlayer(GameObject p)
    {
        Player = p;
    }

    public MoveAgent GetPlayer() { return Player.GetComponent<MoveAgent>(); }

    public void SetVoiceRecogniser(bool newValue)
    {
        voiceRecogniser = newValue;
        Debug.Log(voiceRecogniser);
    }

    public void AvanzaPersonaje()
    {
        Player.GetComponent<MoveAgent>().IniciarMovimiento();
    }

    public void RotaDerechaPersonaje()
    {
        Player.GetComponent<PlayerVoiceMove>().AddRotationY(30);
    }

    public void RotaIzquierdaPersonaje()
    {
        Player.GetComponent<PlayerVoiceMove>().AddRotationY(-30);
    }
    public void RotaArribaPersonaje()
    {
        Player.GetComponent<PlayerVoiceMove>().AddRotationX(30);
    }

    public void RotaAbajoPersonaje()
    {
        Player.GetComponent<PlayerVoiceMove>().AddRotationX(-30);
    }


    public void LoadScene(string scene)
    {
        if (currentScene == scene) return;

        SceneManager.LoadScene(scene);
        currentScene = scene;
        
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
        {
            if (voiceRecogniser)
            {
                GameObject.Find("ControlC").SetActive(false);
            }
            else
            {
                GameObject.Find("CanvasTutorial").SetActive(false);
                GameObject.Find("CanvasVoice").SetActive(false);
                GameObject.Find("Control").SetActive(false);
            }
            GameObject.Find("QuitButton").GetComponent<Button>().onClick.AddListener(() => QuitGame());
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener(() => LoadScene("Iglesia"));
        }
        else if (scene.name == "Victory")
        {
            Cursor.lockState = CursorLockMode.None; // Libera el cursor
            Cursor.visible = true;                  // Muestra el cursor
            GameObject.Find("QuitButton").GetComponent<Button>().onClick.AddListener(() => QuitGame());
        }
    }
}
