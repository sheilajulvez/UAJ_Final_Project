using UnityEngine;
using AudioDetection.Interfaces;

public class MoveAction : IVoiceAction
{
    public void Execute(params object[] parameters)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var mover = player.GetComponent<MoveAgent>();
            if (mover != null)
            {
                mover.IniciarMovimiento();
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

