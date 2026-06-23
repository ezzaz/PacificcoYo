using UnityEngine;

public class TpPisos : MonoBehaviour
{
    public SubirBajarPisos subirBajarPisos;
    public bool soyElDeArriba;
    public bool soyUsable;
    public Jugador jugadorMovimiento;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Jugador>(out Jugador jugador) && soyUsable)
        {
            if (!soyElDeArriba)
            {
                subirBajarPisos.HaciaArriba();
            }
            else
            {
                subirBajarPisos.HaciaAbajo();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        soyUsable = true;

    }
}
