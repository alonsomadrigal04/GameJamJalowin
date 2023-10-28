using UnityEngine;

public class NPC_Behaviour : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    [SerializeField] float minWalkTime = 1.0f;
    [SerializeField] float maxWalkTime = 3.0f;
    [SerializeField] float minIdleTime = 1.0f;
    [SerializeField] float maxIdleTime = 3.0f;
    
    Rigidbody2D rb;
    Vector2 dir;
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
        if (moveTimer <= 0) //Le toca estar fokin parado
        {
            if (isWalking)
            {
                isWalking = false;
                rb.velocity = Vector2.zero;
                moveTimer = Random.Range(minIdleTime, maxIdleTime);
            }
            else
            {
                isWalking = true;
                GetRandomDirection();
                moveTimer = Random.Range(minWalkTime, maxWalkTime);
            }
        }
        else moveTimer -= Time.deltaTime;
    }

    void GetRandomDirection()
    {
        float randomAngle = Random.Range(0f, 360f);
        dir = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
        dir.Normalize();
        rb.velocity = dir * speed;
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
