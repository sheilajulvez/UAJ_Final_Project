using System;
using System.Collections.Generic;
using UnityEngine;
using AudioDetection.Interfaces;
public class ExamplecommandAction : IVoiceAction {
    public void Execute(params object[] parameters) {
        // TODO: Implementar lógica para 'example command'
        Debug.Log("Comando de ejemplo funcionando");
    }
}