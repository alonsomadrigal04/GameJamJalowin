using Unity.VisualScripting;
using UnityEngine;

public class NPC_Behaviour : MonoBehaviour
{
    [SerializeField] float walkSpeed = 1.0f;
    [SerializeField] float runSpeed = 4.0f;
    [SerializeField] float dragSpeed = 10.0f;
    [SerializeField] float minWalkTime = 1.0f;
    [SerializeField] float maxWalkTime = 3.0f;
    [SerializeField] float minIdleTime = 1.0f;
    [SerializeField] float maxIdleTime = 3.0f;
    [SerializeField] float minRunningTime = 3.0f;
    [SerializeField] float maxRunningTime = 7.0f;
    [SerializeField] float safeDistance = 20.0f;
    [SerializeField] float draggedMinDistance = 1.0f;

    GameObject player;
    Rigidbody2D rb;
    Player_behaviour scPlayer;
    Transform tfPlayer;
    Vector2 dir;
    Vector2 runAwayDir;
    int leftOrRight = -1;
    bool isSeparating = false;
    float escapeMult = 1.5f;
    float currentSpeed;
    float maxSpeed;
    float moveTimer;
    bool isWalking = false;
    bool isDragged = false;
    [HideInInspector] public bool scared = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        moveTimer = Random.Range(minIdleTime, maxIdleTime);

        if (player != null)
        {
            scPlayer = player.GetComponent<Player_behaviour>();
            tfPlayer = player.GetComponent<Transform>();
        }
    }

    void Update()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        //Si se acaba el tiempo de miedo, el npc se desasusta y entra en modo andar

        if (moveTimer <= 0 && scared)
        {
            scared = false;
            moveTimer = Random.Range(minWalkTime, maxWalkTime);
        }

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

        moveTimer -= Time.deltaTime;

        //Determinar si el jugador está cerca

        float dist2Player;
        dist2Player = Vector2.Distance(transform.position, tfPlayer.position);
        if (dist2Player < draggedMinDistance) isDragged = true;
        else if (dist2Player >= draggedMinDistance + escapeMult) isDragged = false;

        //Apartarse si está con el jugador

        if (isDragged && !scared)
        {
            if (leftOrRight == -1) leftOrRight = Random.Range(0, 2);
            isWalking = true;
            if (moveTimer <= 0) moveTimer = Random.Range(minWalkTime, maxWalkTime);

            //Aleatorio si se aparta a la derecha o la izquierda

            Transform tfNPC = gameObject.GetComponent<Transform>();
            Vector2 dir2Player = (tfPlayer.position - tfNPC.position).normalized;

            if (!isSeparating)
            {
                isSeparating = true;
                if (leftOrRight == 0) dir = new Vector2(dir2Player.y, -dir2Player.x);
                else if (leftOrRight == 1) dir = new Vector2(-dir2Player.y, dir2Player.x);
            }

            rb.velocity = dir * currentSpeed;
        }
        else
        {
            leftOrRight = -1;
            isSeparating = false;
        }

        //Asignar valores a variables según la situación

        currentSpeed = scared ? runSpeed : isDragged ? dragSpeed : walkSpeed;
        maxSpeed = isWalking ? currentSpeed : 0;

        //Limitar velocidad

        float limitedXSpeed = Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed);
        float limitedYSpeed = Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed);

        rb.velocity = new Vector2(limitedXSpeed, limitedYSpeed);
    }

    public void GetScared()
    {
        scared = true;
        moveTimer = Random.Range(minRunningTime, maxRunningTime);
        isWalking = true;

        Transform tfNPC = gameObject.GetComponent<Transform>();
        Vector2 dir2Player = tfPlayer.position - tfNPC.position;
        runAwayDir = -dir2Player.normalized;
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
