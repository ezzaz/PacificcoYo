using UnityEngine;

public class SalidaJuego : MonoBehaviour
{
    public GameObject arriba, jugador;
    public SalidaHIjo tpArriba;
    public MenusVisibles menues;
    public Jugador jugadorMovimiento;
    public bool volearControles;


    private void Update()
    {
        if (jugadorMovimiento.darVueltaControles && Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            jugadorMovimiento.darVueltaControles = false;
    }

    public void Salir()
    {
        menues.HaciaSalir();
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
    }
}
