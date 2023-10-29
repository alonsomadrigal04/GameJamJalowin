using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player_behaviour : MonoBehaviour
{
    // ------- MOVEMENT -------
    public float moveSpeed;

    // ------- SUICIDE -------
    public bool suiciding;
    public float firts_suicide;
    public float after_suicide;
    private List<GameObject> npcsInScreen = new List<GameObject>();
    private bool firstTime = true;

    // ------- SUICIDE_BAR -------
    [SerializeField] float timer;
    public float currentTime;
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
    public AudioSource audioSource;
    public AudioClip deathMusic;
    public AudioClip[] scaredSounds; // Array de efectos de sonido de NPC asustados
    public AudioClip[] shootSounds;
    private List<int> playedSoundIndices = new List<int>(); // Lista para realizar un seguimiento de los índices de sonidos ya reproducidos

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent <SpriteRenderer>();
        currentTime = timer_suicideMax;

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
        }
    }

    void MovePlayer()
    {
        if (!suiciding)
        {
            if (scMenu.gameStarted)
            {
                Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                rb.velocity = dir.normalized * moveSpeed;

                if (dir != Vector2.zero)
                {
                    float angle = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = new Vector2(0, 0);
                Suicide();
            }
        }
    }

    public void Suicide()
    {
        suiciding = true;
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

                // Calcula el progreso como un valor entre 0 y 1 en función del tiempo restante.
                float progress = Mathf.Clamp01(currentTime / timer_suicideMax);

                // Calcula el ancho actual de la barra de progreso.
                float currentWidth = maxProgressBarWidth * progress; // Asigna el valor a currentWidth

                progressBar.transform.localScale = new Vector3(currentWidth, progressBar.transform.localScale.y, progressBar.transform.localScale.z);

                // Comprueba si el temporizador ha terminado
                if (currentTime <= 0)
                {
                    suiciding = true;
                    Suicide();
                    currentTime = timer; // Reinicia el tiempo
                    progressBar.gameObject.SetActive(true);
                }

                // Asegúrate de que currentTime no exceda el valor máximo
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
            Debug.Log("Antes de morir");
            yield return new WaitForSeconds(time);

            CollectNPCsInScreen();



            if (npcsInScreen.Count > 0)
            {
                GameObject randomNPC = npcsInScreen[Random.Range(0, npcsInScreen.Count)];
                transform.position = randomNPC.transform.position;
                Destroy(randomNPC);
                suiciding = false;

                // Reproduce un sonido aleatorio de "shootSounds"
                int randomShootSoundIndex = Random.Range(0, shootSounds.Length);
                audioSource.PlayOneShot(shootSounds[randomShootSoundIndex]);

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

                    // Agrega el índice del sonido a la lista de sonidos reproducidos
                    playedSoundIndices.Add(randomSoundIndex);

                    // Asegúrate de que la lista de índices de sonidos no crezca indefinidamente
                    if (playedSoundIndices.Count >= scaredSounds.Length)
                    {
                        playedSoundIndices.Clear();
                    }

                    AudioSource.PlayClipAtPoint(scaredSounds[randomSoundIndex], npc.transform.position);

                    currentTime = Mathf.Clamp(currentTime, 0, timer_suicideMax - 2);
                    currentTime += (npcsInScreen.Count * 0.3f);
                }
            }
            else
            {
                SceneManager.LoadScene("GameOver");
                gameOver = true;
            }

            Debug.Log("Después de morir");
        }
    }

    private void CheckWin()
    {
        if (AreAllNPCsCollected())
        {
            // El jugador ganó, carga la escena "winscreen"
            SceneManager.LoadScene("winscreen");
        }
    }

    private bool AreAllNPCsCollected()
    {
        GameObject[] allNpcs = GameObject.FindGameObjectsWithTag("NPC");
        return allNpcs.Length == 0;
    }
}
