using Unity.VisualScripting;
using UnityEngine;

public class Slots : MonoBehaviour
{
    public bool respuestaCorrecta;
    public DragAndDrop dragAndDrop;
    public Completado complete;

    private void Start()
    {
        respuestaCorrecta = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Dragueable>(out Dragueable dragScript))
        {
            if (collision.name == gameObject.name)
            {
                collision.transform.position = gameObject.transform.position;
                collision.transform.rotation = gameObject.transform.rotation;
                dragScript.enabled = false;
                respuestaCorrecta = true;
                complete.numeroActual++;
                collision.TryGetComponent<Collider2D>(out Collider2D collider);
                Destroy(collider);
            }
        }
    }
}
