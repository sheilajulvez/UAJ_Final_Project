using UnityEngine;

public class MoveAgent : MonoBehaviour
{
    private Vector3 direccionMovimiento;
    private float distanciaTotal = 5f;
    private float distanciaRecorrida = 0f;
    private bool enMovimiento = false;
    public float velocidad = 3f;

    void Start()
    {
        GameManager.Instance.SetPlayer(this.gameObject);
    }

    void Update()
    {
        if (enMovimiento)
        {
            float paso = velocidad * Time.deltaTime;
            transform.position += direccionMovimiento * paso;
            distanciaRecorrida += paso;

            if (distanciaRecorrida >= distanciaTotal)
            {
                enMovimiento = false;
                distanciaRecorrida = 0f;
            }
        }
    }

    public void IniciarMovimiento()
    {
        if (!enMovimiento)
        {
            direccionMovimiento = transform.forward.normalized;
            distanciaRecorrida = 0f;
            enMovimiento = true;
        }
    }
}
