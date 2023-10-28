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

        // Si se encontr� un objetivo, seguirlo
        if (target != null)
        {
            // Obtener la posici�n actual de la c�mara
            Vector3 cameraPosition = transform.position;

            // Obtener la posici�n del objetivo
            Vector3 targetPosition = target.position;

            // Establecer la posici�n de la c�mara para que siga al objetivo
            cameraPosition.x = targetPosition.x;
            cameraPosition.y = targetPosition.y;
            transform.position = cameraPosition;
        }
    }
}

