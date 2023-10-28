using UnityEngine;

public class NPC_Behaviour : MonoBehaviour
{
    [SerializeField] float walkSpeed = 1.0f;
    [SerializeField] float runSpeed = 4.0f;
    [SerializeField] float minWalkTime = 1.0f;
    [SerializeField] float maxWalkTime = 3.0f;
    [SerializeField] float minIdleTime = 1.0f;
    [SerializeField] float maxIdleTime = 3.0f;

    Rigidbody2D rb;
    Vector2 dir;
    float currentSpeed;
    float maxSpeed;
    float moveTimer;
    bool isWalking = false;
    bool scared = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveTimer = Random.Range(minIdleTime, maxIdleTime);
    }

    void Update()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        //Asignar valores a variables según la situación

        maxSpeed = isWalking ? walkSpeed : 0;
        currentSpeed = scared ? runSpeed : walkSpeed;

        //Arranque y detención

        if (scared)
        {
            if (moveTimer < minWalkTime) moveTimer = Random.Range(minWalkTime, maxWalkTime);
            if (!isWalking) isWalking = true;
        }
        else
        {
            if (moveTimer <= 0)
            {
                if (isWalking) //Le toca estar fokin parado
                {
                    isWalking = false;
                    rb.velocity = Vector2.zero;
                    moveTimer = Random.Range(minIdleTime, maxIdleTime);
                }
                else //Le toca estar fokin moviéndose
                {
                    isWalking = true;
                    GetRandomDirection();
                    moveTimer = Random.Range(minWalkTime, maxWalkTime);
                }
            }
            else moveTimer -= Time.deltaTime;
        }

        //Limitar velocidad

        float limitedXSpeed = Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed);
        float limitedYSpeed = Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed);

        rb.velocity = new Vector2(limitedXSpeed, limitedYSpeed);
    }

    void GetRandomDirection()
    {
        float randomAngle = Random.Range(0f, 360f);
        dir = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
        dir.Normalize();
        rb.velocity = dir * walkSpeed;
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
