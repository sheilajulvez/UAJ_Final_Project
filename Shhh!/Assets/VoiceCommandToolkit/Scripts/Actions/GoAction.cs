using System;
using UnityEngine;
using UnityEngine.SceneManagement; // Para SceneManager
using AudioDetection.Interfaces;


public class GoAction : IVoiceAction {
    public void Execute(params object[] parameters)
    {
        Debug.Log("go to");
        // Loguear todos los parámetros recibidos
        if (parameters == null || parameters.Length == 0)
        {
            Debug.LogWarning("GotoAction: No se proporcionó ningún parámetro para la escena destino.");
            return;
        }
        else
        {
            Debug.Log($"GotoAction: Parámetros recibidos ({parameters.Length}): {string.Join(", ", parameters)}");
        }

        // Intentamos obtener el nombre de la escena destino como string
        string targetScene = parameters[0] as string;
        if (string.IsNullOrWhiteSpace(targetScene))
        {
            Debug.LogWarning("GotoAction: El parámetro de la escena destino no es válido.");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;

        // Comprobar si la escena actual permite el cambio (lista blanca)
        string[] allowedScenes = { "Retiro", "Cine", "Iglesia", "Mina" };

        if (Array.Exists(allowedScenes, scene => scene.Equals(currentScene, StringComparison.OrdinalIgnoreCase)))
        {
            Debug.Log($"GotoAction: Cargando escena '{targetScene}' desde '{currentScene}'.");
            GameManager.Instance.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning($"GotoAction: No se puede ir a '{targetScene}' desde la escena '{currentScene}'.");
        }
    }
}