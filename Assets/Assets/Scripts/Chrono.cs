using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chrono : MonoBehaviour
{
    [SerializeField] Menu_Behaviour menu; 
    TextMeshProUGUI miTexto;

    float myChrono = 0.0f;

    void Start()
    {
        miTexto = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (menu.gameStarted)
        {
            miTexto.text = "Time: " + myChrono.ToString("F2");
            myChrono += Time.deltaTime;
        }
        else miTexto.text = "";
    }
}
