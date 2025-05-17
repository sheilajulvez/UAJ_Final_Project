using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] GameObject Player;
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
            LoadNextScene();
        }
    }

    public void LoadNextScene()
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
        if(Player== null) Player= GameObject.Find("player1");
        Debug.Log(Player.name);

        GameObject playerPos = GameObject.Find("playerpos");
        if (playerPos != null)
        {
            Player.transform.position = playerPos.transform.position;
            Debug.LogWarning(" se encontró el objeto 'PlayerPos' en la escena cargada.");
        }
        else
        {
            Debug.LogWarning("No se encontró el objeto 'PlayerPos' en la escena cargada.");
        }
    }
}
