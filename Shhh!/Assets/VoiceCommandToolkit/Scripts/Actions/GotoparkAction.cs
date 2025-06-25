using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoparkAction : IVoiceAction
{
    public void Execute()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Retiro" || currentScene == "Cine" || currentScene == "Iglesia" || currentScene == "Mina")
        {
            GameManager.Instance.LoadScene("Retiro"); // Cambiar por el nombre real si es diferente
        }
        else
        {
            Debug.LogWarning($"No se puede ir al parque desde la escena {currentScene}.");
        }
    }
}
