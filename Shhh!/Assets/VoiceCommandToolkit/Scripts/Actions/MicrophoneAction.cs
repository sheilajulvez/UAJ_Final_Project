using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AudioDetection.Interfaces;
public class MicrophoneAction : IVoiceAction
{
    public void Execute(params object[] parameters)
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            GameObject micro = GameObject.Find("Tutorial");
            if (micro != null)
            {
                Button btn = micro.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.Invoke();
                }
                else
                {
                    Debug.LogWarning("El objeto 'Microphone' no tiene un componente Button.");
                }
            }
            else
            {
                Debug.LogWarning("No se encontr� el objeto 'Microphone'.");
            }
        }
        else
        {
            Debug.LogWarning("El comando 'microphone' solo est� disponible desde la escena 'Menu'.");
        }
    }
}
