using System;
using UnityEngine;
using UnityEngine.SceneManagement; // Para SceneManager
using UnityEngine.UI;             // Para Button

public class VoiceAction : IVoiceAction
{
    public void Execute()
    {
        if (SceneManager.GetActiveScene().name == "Selection")
        {
            GameManager.Instance.LoadScene("Menu");
        }
    }
}
