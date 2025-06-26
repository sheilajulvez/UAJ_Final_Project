using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using AudioDetection.Interfaces;
public class UpAction : IVoiceAction
{
    public void Execute(params object[] parameters)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Retiro" || currentScene == "Cine" || currentScene == "Iglesia" || currentScene == "Mina")
        {
            GameManager.Instance.RotaArribaPersonaje();
        }
        else
        {
            Debug.LogWarning($"No se puede rotar hacia arriba en la escena {currentScene}.");
        }
    }
}
