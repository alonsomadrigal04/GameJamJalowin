using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player_behaviour : MonoBehaviour
{
    // ------- MOVEMENT -------
    public float moveSpeed;
    float currentMoveSpeed;

    // ------- SUICIDE -------
    public bool suiciding;
    public float firts_suicide;
    public float after_suicide;
    private List<GameObject> npcsInScreen = new List<GameObject>();
    private bool firstTime = true;

    // ------- SUICIDE_BAR -------
    [SerializeField] float timer;
    [HideInInspector] public float currentTime;
    [SerializeField] float maxProgressBarWidth = 1.0f;
    [SerializeField] Image progressBar;
    [SerializeField] Image cristal;
    public float timer_suicideMax;

    // ------- RIGIDBODY -------
    private Rigidbody2D rb;

    // ------- SPRITERENDERER -------
    private SpriteRenderer spriteRenderer;

    // ------- MENU -------
    public Menu_Behaviour scMenu;

    // ------- GAME_LOGIC -------
    public bool gameOver = false;

    // ------- SOUNDS -------
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public AudioClip deathMusic;
    public AudioClip[] scaredSounds; // Array de efectos de sonido de NPC asustados
    public AudioClip[] shootSounds;
    public AudioClip chainsaw;
    private List<int> playedSoundIndices = new List<int>(); // Lista para realizar un seguimiento de los �ndices de sonidos ya reproducidos

    //------- CAMERA -------
    public Camera_behaviour scCamera;
    public Camera cam;

    //------- ANIMATOR ------- 
    public Animator animatior;
    public Animator bloodAnimator;
    public GameObject cuerpo;
    public GameObject bloodAnimation;
    public GameObject[] blood;
    public Color baseColor;
    public Color finalColor;
    public GameObject particlePrefab; // Asigna el prefab de part�culas en el Inspector


    public float timerBandEnding;
    public float maxtimerBadEnding;
    public bool isBadEnding = false;  // Estado de la BadEnding




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent <SpriteRenderer>();
        currentTime = timer_suicideMax;
        timerBandEnding = maxtimerBadEnding;

        // Establece el color inicial del personaje
        spriteRenderer.color = baseColor;

        if (scMenu.gameStarted)
        {
            currentTime = timer_suicideMax;
            progressBar.gameObject.SetActive(true);
        }
        else
        {
            progressBar.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        MovePlayer();

        if (!gameOver)
        {
            Suicide_bar();
            CheckWin();
            ChangeColorBasedOnTime(); 

        }
        if (isBadEnding)
        {
            timerBandEnding -= Time.deltaTime;

            // Cuando el temporizador llega a cero, cambia a la pantalla "GameOver"
            if (timerBandEnding <= 0)
            {
                SceneManager.LoadScene("GameOver");
            }
        }
    }

    void ChangeColorBasedOnTime()
    {
        // Aseg�rate de que currentTime est� en el rango de [0, timer_suicideMax]
        currentTime = Mathf.Clamp(currentTime, 0, timer_suicideMax);

        // Calcula el progreso como un valor entre 0 y 1 en funci�n del tiempo restante.
        float progress = 1.0f - (currentTime / timer_suicideMax); // Invierte el progreso para que est� relacionado con el tiempo restante

        // Interpola gradualmente entre el color base y el color final
        Color lerpedColor = Color.Lerp(baseColor, finalColor, progress);

        // Establece el color del spriteRenderer
        spriteRenderer.color = lerpedColor;
    }

    void MovePlayer()
    {
        if (!suiciding && !isBadEnding) // Agrega !isBadEnding a la condici�n
        {
            if (scMenu.gameStarted)
            {
                Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

                // Verifica si el jugador se est� moviendo
                bool isWalking = dir != Vector2.zero;

                // Establece el valor de la animaci�n
                animatior.SetBool("isWalking", isWalking);

                // Calcula la direcci�n vertical
                float flipDirection = Mathf.Sign(dir.x); // Devuelve 1 si te mueves hacia la derecha y -1 si te mueves hacia la izquierda

                // Voltea el sprite verticalmente
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * flipDirection, transform.localScale.y, transform.localScale.z);

                rb.velocity = new Vector2(dir.x * currentMoveSpeed, dir.y * currentMoveSpeed); // Multiplica dir.x y dir.y por moveSpeed

                // Si quieres mantener el eje X constante, no necesitas modificar la rotaci�n en este caso.
            }
        }
        else
        {
            // El jugador no puede moverse, as� que su velocidad es cero
            rb.velocity = Vector2.zero;
        }

        if (!suiciding && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(0, 0);
            Suicide();
        }
    }






    public void Suicide()
    {
        suiciding = true;
        animatior.SetBool("Dies", true);
        if (firstTime)
        {
            StartCoroutine(Dying(firts_suicide));
            firstTime = false;
        }
        else
        {
            StartCoroutine(Dying(after_suicide));
        }
    }

    void Suicide_bar()
    {
        if (scMenu.gameStarted)
        {
            if (!suiciding)
            {
                currentTime -= Time.deltaTime;

                //Cuanto mas cerca de suicidarse mas rapido

                currentMoveSpeed = Mathf.Lerp(0f, moveSpeed, 1f - (currentTime / 15f));

                // Calcula el progreso como un valor entre 0 y 1 en funci�n del tiempo restante.
                float progress = Mathf.Clamp01(currentTime / timer_suicideMax);

                // Calcula el ancho actual de la barra de progreso.
                float currentWidth = maxProgressBarWidth * progress; // Asigna el valor a currentWidth

                progressBar.transform.localScale = new Vector3(currentWidth, progressBar.transform.localScale.y, progressBar.transform.localScale.z);

                // Interpola entre el color inicial y el color final basado en el progreso.
                spriteRenderer.color = Color.Lerp(baseColor, finalColor, progress);

                // Comprueba si el temporizador ha terminado
                if (currentTime <= 0)
                {
                    suiciding = true;
                    Suicide();
                    rb.velocity = new Vector2(0,0);
                    currentTime = timer; // Reinicia el tiempo
                    progressBar.gameObject.SetActive(true);
                }

                // Aseg�rate de que currentTime no exceda el valor m�ximo
                currentTime = Mathf.Min(currentTime, timer_suicideMax);
            }
        }
    }


    void CollectNPCsInScreen()
    {
        GameObject[] allNpcs = GameObject.FindGameObjectsWithTag("NPC");
        Camera mainCamera = Camera.main;
        npcsInScreen.Clear();

        foreach (GameObject npc in allNpcs)
        {
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(npc.transform.position);
            if (screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1)
            {
                npcsInScreen.Add(npc);
            }
        }
    }
    


    IEnumerator Dying(float time)
    {
        if (suiciding)
        {
            yield return new WaitForSeconds(time);
            GameObject nuevoObjeto = Instantiate(cuerpo, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);  

            CollectNPCsInScreen();

            if (npcsInScreen.Count > 0)
            {
                int randomBloodMarck = Random.Range(0, blood.Length);
                GameObject nuevoObjeto1 = Instantiate(blood[randomBloodMarck], new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                scCamera.CameraShake(0.3f, 1000.0f);
                GameObject randomNPC = npcsInScreen[Random.Range(0, npcsInScreen.Count)];
                
                Destroy(randomNPC);

                suiciding = false;

                

                progressBar.gameObject.SetActive(true);
                cristal.gameObject.SetActive(true);



                foreach (GameObject npc in npcsInScreen)
                {
                    NPC_Behaviour npc_script = npc.GetComponent<NPC_Behaviour>();
                    npc_script.GetScared();

                    int randomSoundIndex;
                    do
                    {
                        randomSoundIndex = Random.Range(0, scaredSounds.Length);
                    } while (playedSoundIndices.Contains(randomSoundIndex));

                    // Agrega el �ndice del sonido a la lista de sonidos reproducidos
                    playedSoundIndices.Add(randomSoundIndex);

                    // Aseg�rate de que la lista de �ndices de sonidos no crezca indefinidamente
                    if (playedSoundIndices.Count >= scaredSounds.Length)
                    {
                        playedSoundIndices.Clear();
                    }

                    AudioSource.PlayClipAtPoint(scaredSounds[randomSoundIndex], npc.transform.position);

                    currentTime = Mathf.Clamp(currentTime, 0, timer_suicideMax - 2);
                    currentTime += 1;
                    currentTime += (npcsInScreen.Count * 0.5f);
                }

                transform.position = randomNPC.transform.position;
            }
            else
            {
                SceneManager.LoadScene("GameOver");
                gameOver = true;
            }

            animatior.SetBool("Dies", false);
            // Crea una instancia del GameObject prefabACrear en la posici�n (0, 0, 0) y sin rotaci�n.


            Debug.Log("Despu�s de morir");
        }
    }

    public void ShootingSounds()
    {

        int randomShootSoundIndex = Random.Range(0, shootSounds.Length);
        audioSource.PlayOneShot(shootSounds[randomShootSoundIndex]);
        GameObject nuevoObjeto2 = Instantiate(bloodAnimation, new Vector3(transform.position.x - 0.5f, transform.position.y +1, transform.position.z), Quaternion.identity);
    }


    private void CheckWin()
    {
        if (AreAllNPCsCollected())
        {
            // El jugador gan�, carga la escena "winscreen"
            Suicide();
            SceneManager.LoadScene("winscreen");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathZone"))
        {
            BadEnding();
            isBadEnding= true;
        }
    }

    public void BadEnding()
    {
        SpawnParticles();
        audioSource.PlayOneShot(chainsaw);
    }
    void SpawnParticles()
    {
        // Instancia el prefab de part�culas en la posici�n actual y sin rotaci�n
        GameObject particleObject = Instantiate(particlePrefab, new Vector3(transform.position.x - 0.5f, transform.position.y + 1, transform.position.z), Quaternion.identity);

        // Aseg�rate de que el sistema de part�culas est� emitiendo (si no se inicia autom�ticamente)
        ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }

    private bool AreAllNPCsCollected()
    {
        GameObject[] allNpcs = GameObject.FindGameObjectsWithTag("NPC");
        return allNpcs.Length == 0;
    }
}
