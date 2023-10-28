using Unity.VisualScripting;
using UnityEngine;

public class NPC_Behaviour : MonoBehaviour
{
    [Header("VELOCIDADES")]
    [Space(15)]
    [SerializeField] float walkSpeed = 2.0f;
    [SerializeField] float runSpeed = 4.0f;
    [SerializeField] float dragSpeed = 6.0f;
    [Header("TIEMPOS DE ESPERA")]
    [Space(15)]
    [SerializeField] float minWalkTime = 1.0f;
    [SerializeField] float maxWalkTime = 3.0f;
    [SerializeField] float minIdleTime = 1.0f;
    [SerializeField] float maxIdleTime = 3.0f;
    [SerializeField] float minRunningTime = 3.0f;
    [SerializeField] float maxRunningTime = 7.0f;
    [SerializeField] float unsafeSeconds = 0.2f;
    [Header("DISTANCIAS MÍNIMAS")]
    [Space(15)]
    [SerializeField] float safeDistance = 3.0f;
    [SerializeField] float draggedMinDistance = 3.0f;

    //Componentes
    GameObject player;
    Rigidbody2D rb;
    Transform tfPlayer;
    //Movimiento
    Vector2 dir;
    Vector2 runAwayDir;
    int leftOrRight = -1;
    bool isSeparating = false;
    bool inSafeZone = true;
    float unsafeTimer;
    float escapeMult = 1.5f;
    float currentSpeed;
    float maxSpeed;
    float moveTimer;
    //Estados
    bool isWalking = false;
    bool isDragged = false;
    [HideInInspector] public bool scared = false;
    // Animator
    public Animator animatior;

    void Start()
    {
        SetUpShit();
    }

    void Update()
    {
        UpdateMovement();
        UpdateDragging();
        AvoidDeathZones();
    }

    void SetUpShit()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        moveTimer = Random.Range(minIdleTime, maxIdleTime);
        if (player != null) tfPlayer = player.GetComponent<Transform>();
    }

    void UpdateMovement()
    {
        //Si se acaba el tiempo de miedo, el npc se desasusta y entra en modo andar

        if (moveTimer <= 0 && scared)
        {
            scared = false;
            animatior.SetBool("isScared", false);
            moveTimer = Random.Range(minWalkTime, maxWalkTime);
        }

        //Arranque y detención

        if (scared) rb.velocity = runAwayDir * currentSpeed;
        else if (moveTimer <= 0)
        {
            if (isWalking) //PASEANDO
            {
                isWalking = false;
                animatior.SetBool("isWalking", false);
                moveTimer = Random.Range(minIdleTime, maxIdleTime);

                rb.velocity = Vector2.zero;
            }
            else //ESPERANDO
            {
                isWalking = true;
                animatior.SetBool("isWalking", true);
                float randomAngle = Random.Range(0f, 360f);
                dir = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
                dir.Normalize();
                moveTimer = Random.Range(minWalkTime, maxWalkTime);

                rb.velocity = dir * currentSpeed;
            }
        }

        if (rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1.626057f, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        }
        // Si el NPC se está moviendo a la derecha, restauras la escala original
        else if (rb.velocity.x > 0)
        {
            transform.localScale = new Vector3(1.626057f, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        }

        moveTimer -= Time.deltaTime;

        //Asignar valores a variables según la situación

        currentSpeed = scared ? runSpeed : isDragged ? dragSpeed : walkSpeed;
        maxSpeed = isWalking ? currentSpeed : 0;

        //Limitar velocidad

        float limitedXSpeed = Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed);
        float limitedYSpeed = Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed);

        rb.velocity = new Vector2(limitedXSpeed, limitedYSpeed);
    }

    void UpdateDragging()
    {
        if (inSafeZone)
        {
            //Determinar si el jugador está cerca

            float dist2Player;
            dist2Player = Vector2.Distance(transform.position, tfPlayer.position);
            if (dist2Player < draggedMinDistance) isDragged = true;
            else if (dist2Player >= draggedMinDistance + escapeMult) isDragged = false;

            //Apartarse si está con el jugador

            if (isDragged && !scared && inSafeZone)
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
        }
        else
        {
            isDragged = false;
        }
    }

    public void GetScared()
    {
        scared = true;
        animatior.SetBool("isScared", true);
        moveTimer = Random.Range(minRunningTime, maxRunningTime);
        isWalking = true;

        Transform tfNPC = gameObject.GetComponent<Transform>();
        Vector2 dir2Player = tfPlayer.position - tfNPC.position;
        runAwayDir = -dir2Player.normalized;
    }

    void AvoidDeathZones()
    {
        GameObject[] deathZones = GameObject.FindGameObjectsWithTag("DeathZone");

        // Recorre las DeathZones para encontrar la más cercana
        foreach (GameObject deathZone in deathZones)
        {
            if (deathZone.TryGetComponent<Collider2D>(out var deathCollider))
            {
                float distance = Vector2.Distance(transform.position, deathCollider.ClosestPoint(transform.position));

                if (distance < safeDistance && inSafeZone)
                {
                    inSafeZone = false;
                    unsafeTimer = unsafeSeconds; //Temporizador diminuto para no entrar en bucle re loco
                    rb.velocity *= -1; //Si entra en zona peligrosa, gira en sentido opuesto
                    //Si no tiene casi velocidad, se le suma un poco:
                    if (rb.velocity.magnitude <= 1) rb.velocity += new Vector2(Mathf.Sign(rb.velocity.x), Mathf.Sign(rb.velocity.y));
                    break;
                }
            }
        }

        if (unsafeTimer >= 0) unsafeTimer -= Time.deltaTime;
        else inSafeZone = true;
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
