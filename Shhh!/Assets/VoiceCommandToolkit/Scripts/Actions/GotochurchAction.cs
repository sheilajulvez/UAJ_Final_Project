using System;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para SceneManager
using AudioDetection.Interfaces;
public class GotochurchAction : IVoiceAction
{
    public void Execute(params object[] parameters)
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
