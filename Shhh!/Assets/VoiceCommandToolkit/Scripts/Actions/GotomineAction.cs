using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para SceneManager
using AudioDetection.Interfaces;
public class GotomineAction : IVoiceAction {
    public void Execute(params object[] parameters)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Retiro" || currentScene == "Cine" || currentScene == "Iglesia" || currentScene == "Cine")
        {
            GameManager.Instance.LoadScene("Mina");
        }
        else
        {
            Debug.LogWarning($"No se puede ir a Mina desde la escena {currentScene}.");

        }
    }
}