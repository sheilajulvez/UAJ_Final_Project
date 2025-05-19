using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVoiceMove : MonoBehaviour
{

    private float rotToMove;

    void Start()
    {
        rotToMove = 0f;
    }

    void Update()
    {
        if (Mathf.Abs(rotToMove) > 0f)
        {
            float rotStep = 90f * Time.deltaTime; // velocidad de rotación (grados por segundo)
            float direction = Mathf.Sign(rotToMove); // +1 o -1

            float rotation = direction * Mathf.Min(Mathf.Abs(rotToMove), rotStep);
            transform.Rotate(0, rotation, 0);
            rotToMove -= rotation;
        }
    }

    public void AddRotation(float rotation)
    {
        rotToMove += rotation;
    }

}
