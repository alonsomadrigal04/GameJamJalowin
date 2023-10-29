using UnityEngine;

public class DepthSorterer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("El objeto no tiene un SpriteRenderer.");
        }
    }

    void Update()
    {
        // Ajusta el orden de representaci�n en funci�n de la posici�n Y
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -1);
    }
}
