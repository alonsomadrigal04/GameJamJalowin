using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu_Behaviour : MonoBehaviour
{
    [SerializeField] Image backgroundImage;
    [SerializeField] float fadeSpeed = 1.0f;
    public TextMeshProUGUI pressKeyText;

    public bool gameStarted = false;

    // ------- SOUNDS -------
    public AudioSource audioSource;
    public AudioClip menuMusic; // Asigna la canci�n del men� en el Inspector
    public AudioClip metalMusic;

    private void Start()
    {
        StartCoroutine(BlinkText());
        audioSource.loop = true; // Establece la canci�n en bucle

        // Precargar la m�sica de metal
        metalMusic.LoadAudioData();

        // Aseg�rate de que el juego no ha comenzado, si es as�, reproduce la m�sica del men�
        if (!gameStarted)
        {
            audioSource.clip = menuMusic;
            audioSource.Play();
        }
        else
        {
            //audioSource.clip = metalMusic;
            //audioSource.Play();
        }
    }

    private void Update()
    {
        if (!gameStarted && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(StartGame());
        }


    }

    private IEnumerator StartGame()
    {
        gameStarted = true;

        // Fade Out de la m�sica del men�
        while (audioSource.volume > 0)
        {
            audioSource.volume -= fadeSpeed * Time.deltaTime;
            yield return null;
        }
        audioSource.Stop();

        // Fade In de la m�sica de metal
        audioSource.clip = metalMusic;
        audioSource.Play();
        while (audioSource.volume < 0.07f)
        {
            audioSource.volume += fadeSpeed * Time.deltaTime;
            yield return null;
        }
        //yield return new WaitForSeconds(startDelay);
        // Desvanece la imagen de fondo
        while (backgroundImage.color.a > 0)
        {
            Color color = backgroundImage.color;
            color.a -= fadeSpeed * Time.deltaTime;
            pressKeyText.enabled = false;
            backgroundImage.color = color;
            yield return null;
        }

        // Espera un tiempo antes de iniciar el juego



        StartGameLogic();
    }

    private void StartGameLogic()
    {
        Player_behaviour playerBehavior = FindObjectOfType<Player_behaviour>();
        if (playerBehavior != null)
        {
            //playerBehavior.Suicide();

        }

        // Det�n la m�sica del men�

        gameObject.SetActive(false);
        pressKeyText.enabled = false;
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