using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para SceneManager
public class GotocinemaAction : IVoiceAction
{
    public void Execute()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Retiro" || currentScene == "Cine" || currentScene == "Iglesia" || currentScene == "Mina")
        {
            GameManager.Instance.LoadScene("Cine");
        }
        else
        {
            Debug.LogWarning($"No se puede ir a Cine desde la escena {currentScene}.");

        }
    }
}