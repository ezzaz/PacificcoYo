using UnityEngine;

public class MenusVisibles : MonoBehaviour
{
    public GameObject tablero, opciones, creditos, startMenu, salir;
    private bool pausado;

    private void Start()
    {
        tablero.SetActive(false);
        opciones.SetActive(false);
        creditos.SetActive(false);
        salir.SetActive(false);
        startMenu.SetActive(true);
    }

    public void HaciaTablero()
    {
        tablero.SetActive(true);
        opciones.SetActive(false);
        creditos.SetActive(false);
        salir.SetActive(false);
        startMenu.SetActive(false);
    }
    public void HaciaOpciones()
    {
        tablero.SetActive(false);
        opciones.SetActive(true);
        creditos.SetActive(false);
        salir.SetActive(false);
        startMenu.SetActive(false);
    }
    public void HaciaCreditos()
    {
        tablero.SetActive(false);
        opciones.SetActive(false);
        creditos.SetActive(true);
        salir.SetActive(false);
        startMenu.SetActive(false);
    }

    public void HaciaStartMenu()
    {
        tablero.SetActive(false);
        opciones.SetActive(false);
        creditos.SetActive(false);
        salir.SetActive(false);
        startMenu.SetActive(true);
    }
    public void HaciaSalir()
    {
        tablero.SetActive(false);
        opciones.SetActive(false);
        creditos.SetActive(false);
        salir.SetActive(true);
        startMenu.SetActive(false);
    }
    public void CerrarJuego()
    {
        Application.Quit();
    }
    public void Pausar()
    {
        Time.timeScale = 1f;
        pausado = true;
    }

    public void Reanudar()
    {
        Time.timeScale = 1f;
        pausado = false;
    }
}
