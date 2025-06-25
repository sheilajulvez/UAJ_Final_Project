using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para SceneManager
using UnityEngine.UI; // Necesario para SceneManager
public class QuitAction : IVoiceAction {
    public void Execute() {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            GameObject.Find("QuitButton").GetComponent<Button>().onClick.Invoke();
        }
        else
        {
            GameManager.Instance.QuitGame();
        }
    }
}