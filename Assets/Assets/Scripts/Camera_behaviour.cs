using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_behaviour : MonoBehaviour
{
    private Transform target;
    public float smoothness = 5.0f; // Ajusta la suavidad del movimiento de la cámara
    [SerializeField] float minZoom = 0.5f;
    [SerializeField] float maxZoom = 15.0f;

    Camera cam;

    private void Start()
    {
        FindPlayer();

        cam = GetComponent<Camera>();
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

        PanicScreen();
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

    private void PanicScreen()
    {
        if (target != null)
        {
            Player_behaviour playerBehavior = target.GetComponent<Player_behaviour>();
            if (playerBehavior != null)
            {
                // Calcula el factor de escala en función del tiempo restante del jugador.
                float scale = Mathf.Lerp(minZoom, maxZoom, playerBehavior.currentTime / playerBehavior.timer_suicideMax);

                // Ajusta el tamaño de la cámara.
                cam.orthographicSize = scale;
            }
        }
    }

}
