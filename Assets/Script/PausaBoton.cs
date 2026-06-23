using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PausaBoton : MonoBehaviour
{
    public GameObject menuPausa;

    private bool juegoPausado = false;


    public void OnPause(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (juegoPausado)
        {
            Reanudar();
            Debug.Log("Pausado");
        }
        else
        {
            Pausar();
            Debug.Log("No Pausado");

        }
    }

    public void Reanudar()
    {
        menuPausa.SetActive(false);
        Time.timeScale = 1f;
        juegoPausado = false;
    }

    public void Pausar()
    {
        menuPausa.SetActive(true);
        Time.timeScale = 0f;
        juegoPausado = true;
    }

    public void BacktotheMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}