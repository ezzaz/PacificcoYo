using UnityEngine;
using UnityEngine.SceneManagement;

public class Salida : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene("Minijuego2");
    }
}
