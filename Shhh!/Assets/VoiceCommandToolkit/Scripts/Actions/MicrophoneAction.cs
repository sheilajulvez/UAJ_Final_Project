using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AudioDetection.Interfaces;
public class MicrophoneAction : IVoiceAction
{
    public void Execute(params object[] parameters)
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
}
