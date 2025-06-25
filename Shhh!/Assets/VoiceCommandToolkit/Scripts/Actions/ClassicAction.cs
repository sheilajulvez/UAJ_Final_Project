using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClassicAction : IVoiceAction
{
    public void Execute()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
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
            Debug.LogWarning("El comando 'classic' solo está disponible desde la escena 'Menu'.");
        }
    }
}
