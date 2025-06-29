using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para SceneManager
using UnityEngine.UI; // Necesario para SceneManager
using AudioDetection.Interfaces;
public class QuitAction : IVoiceAction {
    public void Execute(params object[] parameters) {
        
        GameManager.Instance.QuitGame();
        
    }
}