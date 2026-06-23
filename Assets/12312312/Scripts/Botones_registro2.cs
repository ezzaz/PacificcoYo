using UnityEngine;

public class Botones_registro2 : MonoBehaviour
{

    [Header("Estado de las Booleanas")]
    public bool opcion1 = false;
    public bool opcion2 = false;
    public bool opcion3 = false;

    [Header("Paginas")]
    public GameObject pagina1, pagina2, pagina3, pagina4;

    private void Start()
    {
        opcion1 = true;
        opcion2 = false;
        opcion3 = false;
    }

    private void Update()
    {

        if (opcion1)
        {
            pagina1.SetActive(true);
        }
        else
        {
            pagina1.SetActive(false);
        }
        if (opcion2)
        {
            pagina2.SetActive(true);
        }
        else
        {
            pagina2.SetActive(false);
        }
        if (opcion3)
        {
            pagina3.SetActive(true);
        }
        else
        {
            pagina3.SetActive(false);
        }
    }

    void DesactivarTodas()
    {
        opcion1 = false;
        opcion2 = false;
        opcion3 = false;
    }

}
