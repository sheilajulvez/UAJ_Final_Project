using UnityEngine;
using AudioDetection.Interfaces;
using UnityEngine.UI;

public class NextAction : IVoiceAction {
    public void Execute(object[] parameters) {
        // TODO: Implementar lógica para 'next'
        // Parámetros esperados: 
        GameObject next = GameObject.Find("Button");
        if (next != null)
        {
            next.GetComponent<Button>().onClick.Invoke();
        }
    }
}