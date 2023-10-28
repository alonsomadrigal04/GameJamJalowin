using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu_Behaviour : MonoBehaviour
{
    [SerializeField] Image backgroundImage;
    [SerializeField] float fadeSpeed = 1.0f;
    [SerializeField] float startDelay = 2.0f;
    public TextMeshProUGUI pressKeyText;

    public bool gameStarted = false;

    private void Start()
    {
        StartCoroutine(BlinkText());
    }

    private void Update()
    {
        if (!gameStarted && Input.anyKeyDown)
        {
            StartCoroutine(StartGame());
        }
    }

    private IEnumerator StartGame()
    {
        gameStarted = true;

        // Espera un tiempo antes de iniciar el juego
        yield return new WaitForSeconds(startDelay);

        // Desvanece la imagen de fondo
        while (backgroundImage.color.a > 0)
        {
            Color color = backgroundImage.color;
            color.a -= fadeSpeed * Time.deltaTime;
            pressKeyText.enabled = false;
            backgroundImage.color = color;
            yield return null;
        }

        StartGameLogic();
    }

    private void StartGameLogic()
    {
        Player_behaviour playerBehavior = FindObjectOfType<Player_behaviour>();
        if (playerBehavior != null)
        {
            //playerBehavior.Suicide();
            
        }

        gameObject.SetActive(false);
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
