using UnityEngine;

public class PuertasPadre : MonoBehaviour
{
    public GameObject puertaA, puertaB, jugador;
    public Jugador jugadorMovimiento;
    public Vector3 jugadorTamanoOriginal;
    public float multTamanoA, multTamanoB;
    public ControlDeCamara camaraPosiciones;
    public int numeroCamaraA, numeroCamaraB;

    private void Start()
    {
        jugadorTamanoOriginal = jugador.transform.localScale;
    }

    private void Update()
    {
        if (jugadorMovimiento.darVueltaControles && Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            jugadorMovimiento.darVueltaControles = false;
    }

    public void HaciaPuertaA()
    {
        Vector3 jugadorTamanoNuevo;
        jugadorTamanoNuevo = jugadorTamanoOriginal * multTamanoA;
        jugador.transform.position = puertaA.transform.position;
        jugador.transform.localScale = jugadorTamanoNuevo;
        camaraPosiciones.camaraActual = numeroCamaraA;
    }
    public void HaciaPuertaB()
    {
        Vector3 jugadorTamanoNuevo;
        jugadorTamanoNuevo = jugadorTamanoOriginal * multTamanoB;
        jugador.transform.position = puertaB.transform.position;
        jugador.transform.localScale = jugadorTamanoNuevo;
        camaraPosiciones.camaraActual = numeroCamaraB;
    }
}
