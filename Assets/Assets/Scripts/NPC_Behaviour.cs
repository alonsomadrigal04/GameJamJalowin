using UnityEngine;

public class NPC_Behaviour : MonoBehaviour
{
    [SerializeField] float walkSpeed = 1.0f;
    [SerializeField] float runSpeed = 4.0f;
    [SerializeField] float minWalkTime = 1.0f;
    [SerializeField] float maxWalkTime = 3.0f;
    [SerializeField] float minIdleTime = 1.0f;
    [SerializeField] float maxIdleTime = 3.0f;
    [SerializeField] GameObject player;

    Rigidbody2D rb;
    Player_behaviour scPlayer;
    Transform tfPlayer;
    Vector2 dir;
    Vector2 runAwayDir;
    float currentSpeed;
    float maxSpeed;
    float moveTimer;
    bool isWalking = false;
    bool isDragged = false;
    public bool scared = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scPlayer = player.GetComponent<Player_behaviour>();
        tfPlayer = player.GetComponent<Transform>();
        moveTimer = Random.Range(minIdleTime, maxIdleTime);
    }

    void Update()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        //Arranque y detención

        if (scared) rb.velocity = runAwayDir * currentSpeed;
        else if (moveTimer <= 0)
        {
            if (isWalking) //PASEANDO
            {
                isWalking = false;
                moveTimer = Random.Range(minIdleTime, maxIdleTime);

                rb.velocity = Vector2.zero;
            }
            else //ESPERANDO
            {
                isWalking = true;
                float randomAngle = Random.Range(0f, 360f);
                dir = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
                dir.Normalize();
                moveTimer = Random.Range(minWalkTime, maxWalkTime);

                rb.velocity = dir * currentSpeed;
            }
        }
        else moveTimer -= Time.deltaTime;

        //Asignar valores a variables según la situación

        currentSpeed = scared ? runSpeed : walkSpeed;
        maxSpeed = isDragged ? scPlayer.moveSpeed : (isWalking ? currentSpeed : 0);

        //Limitar velocidad

        float limitedXSpeed = Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed);
        float limitedYSpeed = Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed);

        rb.velocity = new Vector2(limitedXSpeed, limitedYSpeed);
    }

    public void GetScared()
    {
        scared = true;
        moveTimer = Random.Range(minWalkTime, maxWalkTime);
        isWalking = true;

        Transform tfNPC = gameObject.GetComponent<Transform>();
        Vector2 dir2Player = tfPlayer.position - tfNPC.position;
        runAwayDir = -dir2Player.normalized;
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    //Si choca con el jugador, se puede arrastrar a la velocidad de este
    void OnCollisionEnter(Collision other) { if (other.gameObject.CompareTag("Player")) isDragged = true; }
    //Si no choca, no
    void OnCollisionExit(Collision other) { if (other.gameObject.CompareTag("Player")) isDragged = false; }

}
