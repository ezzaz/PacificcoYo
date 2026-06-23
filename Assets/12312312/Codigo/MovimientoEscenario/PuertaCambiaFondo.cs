using UnityEngine;

public class PuertaCambiaFondo : MonoBehaviour
{
    public GameObject trianguloDeInteraccion;
    public PuertasPadre puertaPapa;
    public bool elJugadorMeToca, estaEsLaA;

    private void Start()
    {
        trianguloDeInteraccion.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Jugador>(out Jugador jugador))
        {
            trianguloDeInteraccion.SetActive(true);
            elJugadorMeToca = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Jugador>(out Jugador jugador))
        {
            trianguloDeInteraccion.SetActive(false);
            elJugadorMeToca = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (elJugadorMeToca && Input.GetKeyDown(KeyCode.E))
        {
            if (estaEsLaA)
            {
                puertaPapa.HaciaPuertaB();
            }
            else
            {
                puertaPapa.HaciaPuertaA();
            }
        }
    }
}
