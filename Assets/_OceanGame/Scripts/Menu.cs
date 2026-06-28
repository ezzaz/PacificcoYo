using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
    [SerializeField]private GameObject m_Menu;
    [SerializeField] private GameObject m_Settings;

    [SerializeField] private GameObject m_SettingsCanvas;


    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(m_Menu);
    }
    private void Update()
    {

    }

    public void Inicio()
    {
        SceneManager.LoadScene("Minijuego1");
    }
    public void Opciones()
    {
        OpeSettings();
    }
    public void Salir()
    {
        Application.Quit();
    }
    public void OpeSettings()
    {
        m_SettingsCanvas.SetActive(true);

        EventSystem.current.SetSelectedGameObject(m_Settings);
    }
    public void Back()
    {
        m_SettingsCanvas.SetActive(false);
        EventSystem.current.SetSelectedGameObject(m_Menu);

    }

}
