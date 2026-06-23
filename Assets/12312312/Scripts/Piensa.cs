using System.Collections;
using UnityEngine;

public class Piensa : MonoBehaviour
{
    public GameObject[] pensamientos;
    public Botones boton;
    public bool pensamientoActivo;


    private void Start()
    {
        for (int i = 0; i < pensamientos.Length; i++)
        {
            pensamientos[i].gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (boton.respuesta_piensa && !pensamientoActivo)
        {
            StartCoroutine(RevelarPensamiento());
        }
    }

    IEnumerator RevelarPensamiento()
    {
        pensamientoActivo = true;
        boton.respuesta_piensa = false;
        int rand = Random.Range(0, pensamientos.Length);
        pensamientos[rand].gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        pensamientos[rand].gameObject.SetActive(false);
        pensamientoActivo = false;
    }
}
