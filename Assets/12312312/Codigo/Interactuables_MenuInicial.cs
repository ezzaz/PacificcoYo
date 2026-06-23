using UnityEngine;

public class Interactuables_MenuInicial : MonoBehaviour
{
    public MenusVisibles menues;
    public GameObject flechaInterqactiva;
    public bool listoPaLaAccion;

    public bool aOpciones, aTablero, aCreditos;
    private void Start()
    {
        flechaInterqactiva.SetActive(false);
    }
    private void Update()
    {
        if (listoPaLaAccion && Input.GetKeyUp(KeyCode.E))
        {
                if (aOpciones)
                {
                    menues.HaciaOpciones();
                }
                else if (aCreditos)
                {
                    menues.HaciaCreditos();
                }
                else if (aTablero)
                {
                    menues.HaciaTablero();
                }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Jugador>(out Jugador jugador))
        {
            flechaInterqactiva.SetActive(true);
            listoPaLaAccion = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Jugador>(out Jugador jugador))
        {
            flechaInterqactiva.SetActive(false);
            listoPaLaAccion = false;
        }
    }

}
