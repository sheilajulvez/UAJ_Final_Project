using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para SceneManager
using UnityEngine.UI;
public class PlayAction : IVoiceAction
{
    public void Execute()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.Invoke();
        }
    }
}