using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para SceneManager
using  AudioDetection.Interfaces;
public class LeftAction : IVoiceAction {
    public void Execute(params object[] parameters) {
        string n = SceneManager.GetActiveScene().name;
        if (n == "Retiro" || n == "Cine" || n == "Iglesia" || n == "Mina")
        {
            GameManager.Instance.RotaIzquierdaPersonaje();
        }
    }
}