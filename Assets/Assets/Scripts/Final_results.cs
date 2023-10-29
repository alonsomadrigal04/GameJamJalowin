using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Final_results : MonoBehaviour
{
    public TextMeshProUGUI pressKeyText;
    public TextMeshProUGUI score;
    public NPC_Behaviour npcBehaviour;

    private void Start()
    {
        StartCoroutine(BlinkText());
        score.text = "Have shot themselves " + npcBehaviour.deathsCount.ToString() + "people";


    }

    void Update()
    {
        
        // Detecta si se presiona la tecla espaciadora
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Carga la escena "Game"
            SceneManager.LoadScene("Alonso's_Scene");
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
