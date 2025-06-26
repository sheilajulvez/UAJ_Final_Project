using System;
using UnityEngine;
using UnityEngine.SceneManagement; // Para SceneManager
using AudioDetection.Interfaces;

public class GoAction : IVoiceAction
{
    public void Execute(params object[] parameters)
    {
        Debug.Log("go to");

        if (parameters == null || parameters.Length == 0)
        {
            Debug.LogWarning("GotoAction: No se proporcionó ningún parámetro para la escena destino.");
            return;
        }
        else
        {
            Debug.Log($"GotoAction: Parámetros recibidos ({parameters.Length}): {string.Join(", ", parameters)}");
        }

        string targetScene = parameters[0] as string;
        if (string.IsNullOrWhiteSpace(targetScene))
        {
            Debug.LogWarning("GotoAction: El parámetro de la escena destino no es válido.");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;

        // Comprobar lista blanca de escenas desde donde se puede cambiar
        string[] allowedScenes = { "Retiro", "Cine", "Iglesia", "Mina" };

        if (!Array.Exists(allowedScenes, scene => scene.Equals(currentScene, StringComparison.OrdinalIgnoreCase)))
        {
            Debug.LogWarning($"GotoAction: No se puede ir a '{targetScene}' desde la escena '{currentScene}'.");
            return;
        }

        // Comprobar si la escena destino existe en Build Settings
        if (!SceneExistsInBuildSettings(targetScene))
        {
            Debug.LogWarning($"GotoAction: La escena destino '{targetScene}' no está incluida en Build Settings o no existe.");
            return;
        }

        Debug.Log($"GotoAction: Cargando escena '{targetScene}' desde '{currentScene}'.");
        GameManager.Instance.LoadScene(targetScene);
    }

    private bool SceneExistsInBuildSettings(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (string.Equals(sceneNameFromPath, sceneName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}
