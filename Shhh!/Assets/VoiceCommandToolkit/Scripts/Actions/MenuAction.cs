using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAction : IVoiceAction {
    public void Execute()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != "Selection")
        {
            GameManager.Instance.LoadScene("Menu"); // Cambiar por el nombre real si es diferente
        }
        else
        {
            Debug.LogWarning($"No se puede ir al parque desde la escena {currentScene}.");
        }
    }
}