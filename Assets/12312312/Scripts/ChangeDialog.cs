using TMPro;
using UnityEngine;

public class ChangeDialog : MonoBehaviour
{
    public static ChangeDialog Instance;

    public bool esVerdadera, esSeleccionMultiple, esLaFinal;
    public GameObject tableroSeleccion, tableroVoF, tableroFinal;



    public TextMeshPro texto;
    public TextMeshProUGUI respuesta1, respuesta2, respuesta3, respuesta4;
    public TextMeshPro pista;
    

    public TestimoniosSO[] testimonios;

    public int index = 0;
    public bool answer;
    
    void Start()
    {
        Instance = this;
        Actualizar(index);
    }

    public void changeTetx()
    {
        index++;
        Actualizar(index);
    }

    public void changeTextBack()
    {
        index--;
        Actualizar(index);
    }

    public void changeTextSpecific(int numero)
    {
        index = numero;
        Actualizar(index);
    }

    public void Actualizar(int numero)
    {
        texto.text = testimonios[numero].pregunta;
        pista.text = testimonios[numero].pensamiento1;
        esVerdadera = testimonios[numero].esVerdadera;
        esSeleccionMultiple = testimonios[numero].esSeleccionMultiple;
        esLaFinal = testimonios[numero].esLafinal;
        if (esSeleccionMultiple && !esLaFinal)
        { 
            tableroSeleccion.SetActive(true); tableroVoF.SetActive(false); tableroFinal.SetActive(false);
            respuesta1.text = testimonios[numero].respuesta1;
            respuesta2.text = testimonios[numero].respuesta2;
            respuesta3.text = testimonios[numero].respuesta3;
            respuesta4.text = testimonios[numero].respuesta4;
        }
        else if (!esSeleccionMultiple && !esLaFinal)
        { tableroSeleccion.SetActive(false); tableroVoF.SetActive(true); tableroFinal.SetActive(false); }
        else if (esLaFinal)
        {
            tableroSeleccion.SetActive(false); tableroVoF.SetActive(false); tableroFinal.SetActive(true);
        }
    }
}