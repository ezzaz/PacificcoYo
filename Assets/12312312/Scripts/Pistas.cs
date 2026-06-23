using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Pistas : MonoBehaviour
{
    public GameObject[] pistas;
    public Botones boton;
    public int cantidadActual, cantidadMaxima;


    private void Start()
    {
        cantidadActual = cantidadMaxima;
        for (int i = 0; i < pistas.Length; i++)
        {
            pistas[i].gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (cantidadActual > 0 && boton.respuesta_pista)
        {
            StartCoroutine(RevelarPista());
        }
        else if (cantidadActual <= 0 && boton.respuesta_pista)
        {
            Debug.Log("no quedan pistas!");
            boton.respuesta_pista = false;
        }
    }

    IEnumerator RevelarPista()
    {
        // Revisar si quedan pistas inactivas
        bool hayPistasDisponibles = false;
        for (int i = 0; i < pistas.Length; i++)
        {
            if (!pistas[i].activeInHierarchy)
            {
                hayPistasDisponibles = true;
                break;
            }
        }

        if (!hayPistasDisponibles)
        {
            Debug.Log("Ya no hay más pistas disponibles");
            yield break; // Sale de la corrutina
        }

        int rand;
        do
        {
            rand = Random.Range(0, pistas.Length);
        }
        while (pistas[rand].activeInHierarchy);

        pistas[rand].SetActive(true);
        Debug.Log("Se activo la pista numero " + rand);
        cantidadActual--;
        boton.respuesta_pista = false;
        yield return null;
    }
}
