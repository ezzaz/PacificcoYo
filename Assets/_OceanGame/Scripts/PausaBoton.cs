using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PausaBoton : MonoBehaviour
{
    [Tooltip("Opcional: referencia directa al menú de pausa. Si se deja vacío se usa PauseMenuController.Instance.")]
    public PauseMenuController pauseMenu;

    // Conservado por compatibilidad con escenas antiguas (ya no se usa).
    public GameObject menuPausa;

    public void OnPause(InputValue value)
    {
        if (!value.isPressed)
            return;

        var menu = pauseMenu != null ? pauseMenu : PauseMenuController.Instance;
        if (menu != null)
        {
            menu.Toggle();
        }
        else
        {
            Debug.LogWarning("PausaBoton: no se encontró un PauseMenuController en la escena.");
        }
    }

    // Métodos públicos por si quieres enlazarlos a botones desde el Inspector.
    public void Reanudar()
    {
        var menu = pauseMenu != null ? pauseMenu : PauseMenuController.Instance;
        if (menu != null) menu.Resume();
    }

    public void Salir()
    {
        var menu = pauseMenu != null ? pauseMenu : PauseMenuController.Instance;
        if (menu != null) menu.Exit();
    }

}
