using UnityEngine;

public class SubirBajarPisos : MonoBehaviour
{
    public GameObject arriba, abajo, jugador;
    public TpPisos tpArriba, tpAbajo;
    public Jugador jugadorMovimiento;
    public ControlDeCamara camaraPosiciones;
    public bool paraLaDerecha;
    public bool porNumero;
    public bool volearControles;
    public int numeroArriba, numeroAbajo;


    private void Update()
    {
        if (jugadorMovimiento.darVueltaControles && Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            jugadorMovimiento.darVueltaControles = false;
    }

    public void HaciaArriba()
    {
        tpArriba.soyUsable = false;
        if (volearControles)
        {
            if (!jugadorMovimiento.darVueltaControles)
            {
                jugadorMovimiento.darVueltaControles = true;
            }
            else
            {
                jugadorMovimiento.darVueltaControles = false;
            }
        }
        jugador.transform.position = arriba.transform.position;

        if (!porNumero)
            camaraPosiciones.camaraActual++;
        else
        {
            camaraPosiciones.camaraActual = numeroArriba;
        }
    }
    public void HaciaAbajo()
    {
        tpAbajo.soyUsable = false;
        
        if (volearControles)
        {
            if (!jugadorMovimiento.darVueltaControles)
            {
                jugadorMovimiento.darVueltaControles = true;
            }
            else
            {
                jugadorMovimiento.darVueltaControles = false;
            }
        }
        
        jugador.transform.position = abajo.transform.position;

        if (!porNumero)
            camaraPosiciones.camaraActual--;
        else
        {
            camaraPosiciones.camaraActual = numeroAbajo;
        }
    }
}
