using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_behaviour : MonoBehaviour
{
    // ------- MOVEMENT -------
    [SerializeField] float moveSpeed;

    // ------- SUICIDE -------
    public bool suiciding;
    public float shooting_time;
    private List<GameObject> npcsInScreen = new List<GameObject>();

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
                    suiciding = true;
                    Suicide();
                }
            }
        }
    }

    void Suicide()
    {
        StartCoroutine(Dying(shooting_time));
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
                    npc.scared = true;
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