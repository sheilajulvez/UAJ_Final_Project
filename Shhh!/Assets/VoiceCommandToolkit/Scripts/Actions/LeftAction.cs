using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para SceneManager
public class LeftAction : IVoiceAction {
    public void Execute() {
        string n = SceneManager.GetActiveScene().name;
        if (n == "Retiro" || n == "Cine" || n == "Iglesia" || n == "Mina")
        {
            GameManager.Instance.RotaIzquierdaPersonaje();
        }
    }
}