using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Final_results : MonoBehaviour
{
    public TextMeshProUGUI pressKeyText;
    public TextMeshProUGUI score;

    private void Start()
    {
        StartCoroutine(BlinkText());
        score.text = "Have shot themselves 40 people";


    }

    void Update()
    {
        
        // Detecta si se presiona la tecla espaciadora
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Carga la escena "Game"
            SceneManager.LoadScene("GameRoom");
        }
    }

    private IEnumerator BlinkText()
    {
        while (true)
        {
            pressKeyText.enabled = true;
            yield return new WaitForSeconds(0.8f);
            pressKeyText.enabled = false;
            yield return new WaitForSeconds(0.4f);
        }
    }
}
