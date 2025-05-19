using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVoiceMove : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    private float rotToMoveY;
    private float rotToMoveX;

    private float currentXRotation = 0f;

    void Start()
    {
        rotToMoveY = 0f;
        rotToMoveX = 0f;
    }

    void Update()
    {
        if ((Mathf.Abs(rotToMoveX) > 0f) || (Mathf.Abs(rotToMoveY) > 0f))
        {
            float rotStep = 90f * Time.deltaTime;
            float directionX = Mathf.Sign(rotToMoveX);
            float directionY = Mathf.Sign(rotToMoveY);

            float rotationX = directionX * Mathf.Min(Mathf.Abs(rotToMoveX), rotStep);
            float rotationY = directionY * Mathf.Min(Mathf.Abs(rotToMoveY), rotStep);

            // Rota el cuerpo del jugador en Y (izquierda/derecha)
            transform.Rotate(0, rotationY, 0);


            currentXRotation -= rotationX;
            currentXRotation = Mathf.Clamp(currentXRotation, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);

            rotToMoveX -= rotationX;
            rotToMoveY -= rotationY;
        }
    }

    public void AddRotationY(float rotation)
    {
        rotToMoveY += rotation;
    }

    public void AddRotationX(float rotation)
    {
        rotToMoveX += rotation;
    }
}
