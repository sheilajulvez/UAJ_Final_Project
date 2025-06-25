using System;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para SceneManager

public class GotochurchAction : IVoiceAction
{
    public void Execute()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Retiro" || currentScene == "Cine" || currentScene == "Iglesia" || currentScene == "Mina")
        {
            GameManager.Instance.LoadScene("Church");
        }
        else
        {
            Debug.LogWarning($"No se puede ir a Church desde la escena {currentScene}.");
        }
    }
}
