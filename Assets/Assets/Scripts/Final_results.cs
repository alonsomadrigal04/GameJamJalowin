using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Final_results : MonoBehaviour
{
    void Update()
    {
        // Detecta si se presiona la tecla espaciadora
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Carga la escena "Game"
            SceneManager.LoadScene("Game");
        }
    }
}
