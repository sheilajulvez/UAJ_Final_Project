using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para SceneManager
using UnityEngine.UI; // Necesario para SceneManager

using AudioDetection.Interfaces;
public class HelpAction : IVoiceAction {
    public void Execute(params object[] parameters) {
        if (SceneManager.GetActiveScene().name != "Selection" && SceneManager.GetActiveScene().name != "Victory")
        {
            GameObject control = GameObject.Find("Control");
            if (control != null)
            {
                control.GetComponent<Button>().onClick.Invoke();
                //control.SetActive(false);
                // controlSave = control;
            }
        }
    }
}