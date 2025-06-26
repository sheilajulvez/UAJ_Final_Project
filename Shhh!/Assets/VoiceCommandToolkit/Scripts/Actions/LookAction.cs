using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using AudioDetection.Interfaces;

public class LookAction : IVoiceAction
{
    public void Execute(params object[] parameters)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        string[] validScenes = { "Retiro", "Cine", "Iglesia", "Mina" };

        if (!Array.Exists(validScenes, scene => scene.Equals(currentScene, StringComparison.OrdinalIgnoreCase)))
        {
            Debug.LogWarning($"[LookAction] No se puede ejecutar en la escena '{currentScene}'.");
            return;
        }

        if (parameters == null || parameters.Length == 0)
        {
            Debug.LogWarning("[LookAction] No se recibió ninguna dirección (up/down/left/right).");
            return;
        }

        string direction = parameters[0].ToString().ToLower();
        Debug.Log($"[LookAction] Dirección recibida: {direction}");

        switch (direction)
        {
            case "up":
                GameManager.Instance.RotaArribaPersonaje();
                break;
            case "down":
                GameManager.Instance.RotaAbajoPersonaje();
                break;
            case "left":
                GameManager.Instance.RotaIzquierdaPersonaje();
                break;
            case "right":
                GameManager.Instance.RotaDerechaPersonaje();
                break;
            default:
                Debug.LogWarning($"[LookAction] Dirección desconocida: {direction}");
                break;
        }
    }
}
