using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool voiceRecogniser = true;
    private GameObject Player;
    [SerializeField] GameObject image;
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
        if (Input.GetKeyDown(KeyCode.H))
        {
            GameObject.Find("Control").GetComponent<Button>().onClick.Invoke();//menu de ayuda con los comandos de voz
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            GameObject.Find("Back").GetComponent<Button>().onClick.Invoke(); //retroceder boton menús
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
        LoadScene("Menu");
    }

    public bool GetVoiceRecogniser()
    {
        return voiceRecogniser;
    }

    public void SetPlayer(GameObject p)
    {
        Player = p;
        Player.GetComponent<NavMeshAgent>().updateRotation = false;
        Player.GetComponent<NavMeshAgent>().updatePosition = false;
    }

    public MoveAgent GetPlayer() { return Player.GetComponent<MoveAgent>(); }

    public void SetVoiceRecogniser(bool newValue)
    {
        voiceRecogniser = newValue;
        Debug.Log(voiceRecogniser);
    }

    public void AvanzaPersonaje()
    {
        //switch (currentScene)
        //{
        //    case "Iglesia":
        //        IglesiaPath.Instance.AvanzaRuta();
        //        IglesiaTexts.Instance.StartTyping();
        //        break;
        //    case "Mina":
        //        MinePath.Instance.AvanzaRuta();
        //        MineTexts.Instance.StartTyping();
        //        break;

        //}
        Vector3 destino = Player.transform.position + Player.transform.forward * 3f;

        Debug.Log(destino.ToString());
        Player.GetComponent<MoveAgent>().IniciarMovimimiento(destino);
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
        Debug.Log("Cerrando el juego...");
        Application.Quit();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
        {
            if (voiceRecogniser)
            {
                GameObject.Find("Microphone").SetActive(true);
                
            }
            else
            {
                GameObject.Find("Control").SetActive(false);
            }
            image = GameObject.Find("spaceImage");
            image.SetActive(false);
            GameObject.Find("QuitButton").GetComponent<Button>().onClick.AddListener(() => QuitGame());
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.AddListener(() => LoadScene("Iglesia"));
        }
        else if (scene.name == "Victory")
        {
            if (image != null) image.SetActive(false);
            GameObject.Find("Control").SetActive(false);
        }
        else
        {
            if (image != null) image.SetActive(true);
        }
        //if (Player == null) Player= GameObject.Find("player1");

        //GameObject playerPos = GameObject.Find("playerpos");
        //if (playerPos != null)
        //{
        //    Player.transform.position = playerPos.transform.position;
        //    Debug.LogWarning(" se encontró el objeto 'PlayerPos' en la escena cargada.");
        //}
        //else
        //{
        //    Debug.LogWarning("No se encontró el objeto 'PlayerPos' en la escena cargada.");
        //}
    }
}
