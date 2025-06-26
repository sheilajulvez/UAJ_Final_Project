using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using AudioDetection.Interfaces;
public class DownAction : IVoiceAction
{
    public void Execute(params object[] parameters)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Retiro" || currentScene == "Cine" || currentScene == "Iglesia" || currentScene == "Mina")
        {
            GameManager.Instance.RotaAbajoPersonaje();
        }
        else
        {
            Debug.LogWarning($"No se puede rotar hacia abajo en la escena {currentScene}.");
        }
    }
}
