using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class ElTodo : MonoBehaviour
{

    public Animator animaciones;
    public ChangeDialog dialogo;
    public int intentosMax, intentosActu, sigEscena;
    public string sigNivel;
    public float preguntaActual, preguntasMaximas;



    private void Update()
    {


        /*intentosActu--;
        animaciones.SetTrigger("Fallo");

        if (intentosActu <= 0)
        {
            animaciones.SetTrigger("perdiste");
        }*/
    }

    public void Morisiste()
    {
        SceneManager.LoadScene(sigNivel);
    }
    public void Correcto()
    {
        if (dialogo.esVerdadera || dialogo.esSeleccionMultiple)
        {
            preguntaActual++;
            dialogo.changeTetx();
        }
        else
        {
            intentosActu--;
            animaciones.SetTrigger("Fallo");
        }
    }
    public void Incorrecto()
    {
        if (!dialogo.esVerdadera && !dialogo.esSeleccionMultiple)
        {
            preguntaActual++;
            dialogo.changeTetx();
        }
        else if (dialogo.esVerdadera && !dialogo.esSeleccionMultiple)
        {
            intentosActu--;
            animaciones.SetTrigger("Fallo");
        }
        else
        {
            intentosActu--;
            animaciones.SetTrigger("Fallo");
        }
    }
}