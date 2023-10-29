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
        float totalTime = PlayerPrefs.GetFloat("TotalTime", 0.0f);
        score.text = "you have massacred the room in: " + totalTime;
        



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
