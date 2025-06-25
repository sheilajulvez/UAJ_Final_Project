using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : IVoiceAction
{
    public void Execute()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Vector3 destino = player.transform.position + player.transform.forward * 5f;
            Debug.Log("Destino: " + destino);
            var agente = player.GetComponent<MoveAgent>();
            if (agente != null)
            {
                agente.IniciarMovimimiento(destino);
            }
            else
            {
                Debug.LogWarning("El Player no tiene el componente MoveAgent.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró ningún objeto con el tag 'Player'.");
        }
    }
}
