using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_behaviour : MonoBehaviour
{
    private Transform target;
    public float smoothness = 5.0f; // Ajusta la suavidad del movimiento de la cámara

    private void Start()
    {
        FindPlayer();
    }

    void Update()
    {
        if (target == null)
        {
            FindPlayer();
        }

        if (target != null)
        {
            UpdateCameraPosition();
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
        }
    }

    private void UpdateCameraPosition()
    {
        Vector3 cameraPosition = transform.position;
        Vector3 targetPosition = target.position;
        Vector3 smoothPosition = Vector3.Lerp(cameraPosition, targetPosition, smoothness * Time.deltaTime);
        transform.position = new Vector3(smoothPosition.x, smoothPosition.y, cameraPosition.z);
    }
}
