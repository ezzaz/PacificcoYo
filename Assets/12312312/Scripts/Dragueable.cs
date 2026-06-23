using UnityEngine;

public class Dragueable : MonoBehaviour
{
    public bool linked;
    public DragAndDrop puntero;
    public bool estoyTocandoElPuntero, noMeTocaronAMi;
    public float velLerp;



    void Start()
    {
        linked = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //drag
        if (linked)
            transform.position = Vector3.Lerp(transform.position, puntero.transform.position, velLerp * Time.fixedDeltaTime);


        if (puntero.clickeao && !estoyTocandoElPuntero)
            noMeTocaronAMi = true;
        else if (!puntero.clickeao)
            noMeTocaronAMi = false;
          
        if (puntero.clickeao && estoyTocandoElPuntero && !noMeTocaronAMi)
        {
            linked = true;
        }
        else if (!puntero.clickeao || noMeTocaronAMi) { linked = false; }
    }

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<DragAndDrop>(out DragAndDrop compo))
        {
            //Debug.Log("Estoy tocando el puntero");
            estoyTocandoElPuntero = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<DragAndDrop>(out DragAndDrop compo) && !puntero.clickeao)
        {
            //Debug.Log("Ya no estoy tocando el puntero");
            estoyTocandoElPuntero = false;
        }
    }


}
