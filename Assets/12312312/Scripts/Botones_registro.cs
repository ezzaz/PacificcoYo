using UnityEngine;
using UnityEngine.SceneManagement;

public class Botones_registro : MonoBehaviour
{
    public enum TipoBooleana
    {
        Opcion1,
        Opcion2,
        Opcion3,
        Opcion4,
    }

    [Header("Configuración del Botón")]
    public Color clickeao;
    public TipoBooleana booleanaAActivar;
    public Botones_registro2 padre;
    public bool tocoElPuntero;

    private void Update()
    {
        if (tocoElPuntero && Input.GetMouseButtonDown(0))
        {
            ActivarBooleana();
        }
    }

    private void ActivarBooleana()
    {
        switch (booleanaAActivar)
        {
            case TipoBooleana.Opcion1:
                padre.opcion1 = true;
                padre.opcion2 = false;
                padre.opcion3 = false;
                break;
            case TipoBooleana.Opcion2:
                padre.opcion1 = false;
                padre.opcion2 = true;
                padre.opcion3 = false;
                break;
            case TipoBooleana.Opcion3:
                padre.opcion1 = false;
                padre.opcion2 = false;
                padre.opcion3 = true;
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
