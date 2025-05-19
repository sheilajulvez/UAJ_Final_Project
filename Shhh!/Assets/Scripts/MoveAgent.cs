using UnityEngine;
using UnityEngine.AI;

public class MoveAgent : MonoBehaviour
{
    private NavMeshAgent agente;
    private Transform[] puntosRuta;
    private int indiceActual = 0;
    private bool rutaActiva = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        GameManager.Instance.SetPlayer(this.gameObject);
    }

    void Update()
    {
        if (!rutaActiva || puntosRuta == null || puntosRuta.Length == 0)
            return;

        if (!agente.pathPending && agente.remainingDistance < 0.5f)
        {
            AvanzarAlSiguientePunto();
        }
    }

    public void IniciarRuta(Transform[] nuevaRuta)
    {
        puntosRuta = nuevaRuta;
        indiceActual = 0;
        rutaActiva = true;
        agente.SetDestination(puntosRuta[indiceActual].position);
    }

    private void AvanzarAlSiguientePunto()
    {
        indiceActual++;
        if (indiceActual < puntosRuta.Length)
        {
            agente.SetDestination(puntosRuta[indiceActual].position);
        }
        else
        {
            rutaActiva = false; // Ruta terminada
        }
    }
}
