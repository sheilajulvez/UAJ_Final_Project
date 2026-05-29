using UnityEngine;
using AudioDetection.Interfaces;
using UnityEngine.SceneManagement;

public class VoiceAction : IVoiceAction {
    public void Execute(object[] parameters) {
        // TODO: Implementar lógica para 'voice'
        // Parámetros esperados: 
        GameManager.Instance.LoadScene("Menu");
    }
}