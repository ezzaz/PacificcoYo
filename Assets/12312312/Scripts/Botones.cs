using UnityEngine;
using UnityEngine.SceneManagement;


public class Botones : MonoBehaviour
{
    public enum TipoBooleana
    {
        Si,
        No,
        Piensa,
        Pista,
        Transitorio
    }

    [Header("Configuración del Botón")]
    public TipoBooleana booleanaAActivar;

    [Header("Estado de las Booleanas")]
    public  bool respuesta_si = false;
    public  bool respuesta_no = false;
    public  bool respuesta_piensa = false;
    public  bool respuesta_pista = false;
    public  bool transicion = false;
    public bool tocoElPuntero;

    [Header("Elementos de los botones")]
    public Animator trans;

    private void Update()
    {
        if (tocoElPuntero && Input.GetMouseButtonDown(0))
        {
            ActivarBooleana();
        }

        if (transicion)
        {
            trans.SetTrigger("transicion");
            transicion = false;
        }
    }

    private void ActivarBooleana()
    {
        switch (booleanaAActivar)
        {
            case TipoBooleana.Si:
                respuesta_si = true;
                break;
            case TipoBooleana.No:
                respuesta_no = true;
                break;
            case TipoBooleana.Pista:
                respuesta_pista = true;
                break;
            case TipoBooleana.Piensa:
                respuesta_piensa = true;
                break;
            case TipoBooleana.Transitorio:
                transicion = true;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<DragAndDrop>(out DragAndDrop compo))
        {
            tocoElPuntero = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (tocoElPuntero)
        {
            tocoElPuntero = false;
        }
    }
}