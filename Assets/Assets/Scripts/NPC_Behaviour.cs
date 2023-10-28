using UnityEngine;

public class NPC_Behaviour : MonoBehaviour
{
    [SerializeField] float minWalkTime = 1.0f;
    [SerializeField] float maxWalkTime = 3.0f;
    [SerializeField] float minSpeed = 1.0f;
    [SerializeField] float maxSpeed = 3.0f;
    [SerializeField] float minIdleTime = 1.0f;
    [SerializeField] float maxIdleTime = 3.0f;

    Rigidbody2D rb;
    Vector2 dir;
    float moveTimer;
    bool isWalking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GetRandomIdleTime();
    }

    void Update()
    {
        if (isWalking)
        {
            if (moveTimer <= 0)
            {
                GetRandomIdleTime();
                isWalking = false;
                rb.velocity = Vector2.zero;
            }
            else moveTimer -= Time.deltaTime;
        }
        else
        {
            if (moveTimer <= 0)
            {
                GetRandomDirection();
                isWalking = true;
                moveTimer = Random.Range(minWalkTime, maxWalkTime);
            }
            else moveTimer -= Time.deltaTime;
        }
    }

    void GetRandomDirection()
    {
        float randomAngle = Random.Range(0f, 360f);
        dir = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
        dir.Normalize();
        rb.velocity = dir * Random.Range(minSpeed, maxSpeed);
    }

    void GetRandomIdleTime()
    {
        moveTimer = Random.Range(minIdleTime, maxIdleTime);
    }
}
