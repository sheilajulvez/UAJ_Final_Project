using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool voiceRecogniser = true;
    private GameObject Player;
    public string[] sceneNames;
    private int currentSceneIndex = 0;

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
        if (Input.GetKeyDown(KeyCode.R))
        {
            //LoadNextScene();
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
        Player.GetComponent<PlayerVoiceMove>().AddRotation(30);
    }

    public void RotaIzquierdaPersonaje()
    {
        Player.GetComponent<PlayerVoiceMove>().AddRotation(-30);
    }
    //public void RetrocedePersonaje()
    //{
    //    switch (currentScene)
    //    {
    //        case "Iglesia":
    //            IglesiaPath.Instance.RetrocedeRuta();
    //            IglesiaTexts.Instance.StartTypingR();
    //            break;
    //        case "Mina":
    //            MinePath.Instance.RetrocedeRuta();
    //            MineTexts.Instance.StartTypingR();
    //            break;

    //    }
    //}
    /*public void LoadNextScene()
    {
        if (sceneNames.Length == 0) return;

        currentSceneIndex = (currentSceneIndex + 1) % sceneNames.Length;
        SceneManager.LoadScene(sceneNames[currentSceneIndex]);
        //GameObject playerPos = GameObject.Find("playerpos");
        //if (playerPos != null)
        //{
        //    playerPos.transform.position = new Vector3(199f, 40.55f, -151f);
        //}
        //else
        //{
        //    Debug.LogWarning("No se encontró el objeto 'PlayerPos' en la escena cargada.");
        //}

    }*/

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
