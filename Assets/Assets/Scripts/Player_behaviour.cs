using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_behaviour : MonoBehaviour
{
    // ------- MOVEMENT -------
    [SerializeField] float moveSpeed;

    // ------- SUICIDE -------
    public bool suiciding;
    public float firts_suicide;
    public float after_suicide;
    private List<GameObject> npcsInScreen = new List<GameObject>();
    private bool firstTime = true;

    // ------- SUICIDE_BAR -------
    [SerializeField] float timer = 15f; 
    public float currentTime;
    [SerializeField] float maxProgressBarWidth = 1.0f;
    [SerializeField] Image progressBar;



    // ------- RIGIDBODY -------
    private Rigidbody2D rb;

    // ------- SpriteRenderer -------
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        MovePlayer();

        Suicide_bar();
    }

    void MovePlayer()
    {
        if (!suiciding)
        {
            Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            rb.velocity = dir.normalized * moveSpeed;

            if (dir != Vector2.zero)
            {
                float angle = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            if (Input.anyKeyDown)
            {
                if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
                {
                    Suicide();
                    currentTime = 0;
                }
            }
        }
    }

    void Suicide()
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
        if (!suiciding)
        {
            currentTime += Time.deltaTime;

            // Calcula el progreso como un valor entre 0 y 1 en función del tiempo restante.
            float progress = Mathf.Clamp01(1 - currentTime / timer);

            // Calcula el ancho actual de la barra de progreso.
            float currentWidth = maxProgressBarWidth * progress;

            progressBar.transform.localScale = new Vector3(currentWidth, progressBar.transform.localScale.y, progressBar.transform.localScale.z);

            
            // Comprueba si el temporizador ha terminado
            if (currentTime >= timer)
            {
                suiciding = true;
                Suicide();
                currentTime = 0;
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
                foreach (GameObject npc in npcsInScreen)
                {
                    //npc.scared = true;
                }
            }
            else
            {
                GameOver();
            }

            Debug.Log("Después de morir");
        }
    }

    void GameOver()
    {
        Destroy(gameObject);  
    }

}