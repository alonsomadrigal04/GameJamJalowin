using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_behaviour : MonoBehaviour
{
    private Transform target; 

    void Update()
    {

        if (target == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }

        // Si se encontró un objetivo, seguirlo
        if (target != null)
        {
            // Obtener la posición actual de la cámara
            Vector3 cameraPosition = transform.position;

            // Obtener la posición del objetivo
            Vector3 targetPosition = target.position;

            // Establecer la posición de la cámara para que siga al objetivo
            cameraPosition.x = targetPosition.x;
            cameraPosition.y = targetPosition.y;
            transform.position = cameraPosition;
        }
    }
}

