using UnityEngine;
using AudioDetection.Interfaces;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayAction : IVoiceAction {
    public void Execute(object[] parameters) {
        // TODO: Implementar lógica para 'play'
        // Parámetros esperados: 
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            GameObject.Find("PlayButton").GetComponent<Button>().onClick.Invoke();
        }
    }
}