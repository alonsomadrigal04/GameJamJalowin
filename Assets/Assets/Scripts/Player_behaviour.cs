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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
   
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
        StartCoroutine(MiMetodoEspera(shooting_time));
        GameObject[] allNpcs = GameObject.FindGameObjectsWithTag("NPC");

        // Obtiene la cámara principal
        Camera mainCamera = Camera.main;

        // Limpia la lista de NPCs en pantalla
        npcsInScreen.Clear();
        foreach (GameObject npc in allNpcs)
        {
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(npc.transform.position);
            if (screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1)
            {
                // El NPC está dentro de la pantalla, agrégalo a la lista
                npcsInScreen.Add(npc);
            }
        }
    }

    IEnumerator MiMetodoEspera(float time)
    {
        if(suiciding)
        {
            Debug.Log("Antes de morir");
            yield return new WaitForSeconds(time); // Espera durante 2 segundos
            Debug.Log("Despues de morir");
        }
        
    }

}
