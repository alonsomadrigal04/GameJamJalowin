using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_behaviour : MonoBehaviour
{
    private Transform target;
    public float smoothness = 5.0f; // Ajusta la suavidad del movimiento de la cámara
    public float maxShakeAmount = 0.005f; // Ajusta la intensidad máxima de la sacudida (valor reducido)
    public float shakeSpeed = 0.5f; // Ajusta la velocidad de la sacudida (valor reducido)

    private Camera cam;
    private float shakeAmount = 0.0f;

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

        if (shakeAmount > 0)
        {
            // Aplica la sacudida a la posición de la cámara
            Vector2 shakeOffset = Random.insideUnitCircle * shakeAmount;
            transform.position += new Vector3(shakeOffset.x, shakeOffset.y, 0);

            // Reduce gradualmente la intensidad de la sacudida
            shakeAmount -= Time.deltaTime * shakeSpeed;
        }
    }

    private void PanicScreen()
    {
        if (target != null)
        {
            Player_behaviour playerBehavior = target.GetComponent<Player_behaviour>();
            if (playerBehavior != null)
            {
                // Calcula el factor de escala en función de currentTime.
                float scale = Mathf.Lerp(5.0f, 15.0f, playerBehavior.currentTime / playerBehavior.timer_suicideMax);

                // Ajusta el tamaño de la cámara.
                cam.orthographicSize = scale;

                // Aumenta la intensidad de la sacudida en función de currentTime.
                shakeAmount = Mathf.Lerp(0, maxShakeAmount, 1.0f - playerBehavior.currentTime / playerBehavior.timer_suicideMax);
            }
        }
    }
}
