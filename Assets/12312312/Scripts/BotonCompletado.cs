using UnityEngine.SceneManagement;
using UnityEngine;

public class BotonCompletado : MonoBehaviour
{
    public string haciaELNivel;
    public Color clickeao;
    SpriteRenderer color;
    public bool tocoElPuntero;

    private void Start()
    {
        color = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (tocoElPuntero && Input.GetMouseButtonDown(0)) { /*Debug.Log("Hizo click");*/ color.color = clickeao; SceneManager.LoadScene(haciaELNivel);}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<DragAndDrop>(out DragAndDrop compo))
        {
            //Debug.Log("Estoy tocando el puntero"); 
            tocoElPuntero = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<DragAndDrop>(out DragAndDrop compo))
        {
            //Debug.Log("Ya no estoy tocando el puntero"); 
            tocoElPuntero = false;
        }
    }
}
