using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AudioDetection.Interfaces;
public class ClassicAction : IVoiceAction
{
    public void Execute(params object[] parameters)
    {
        if (SceneManager.GetActiveScene().name == "Selection")
        {
            GameObject classic = GameObject.Find("Classic");
            if (classic != null)
            {
                Button btn = classic.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.Invoke();
                }
                else
                {
                    Debug.LogWarning("El objeto 'Classic' no tiene un componente Button.");
                }
            }
            else
            {
                Debug.LogWarning("No se encontró el objeto 'Classic'.");
            }
        }
        else
        {
            Debug.LogWarning("El comando 'classic' solo está disponible desde la escena 'Selection'.");
        }
    }
}
