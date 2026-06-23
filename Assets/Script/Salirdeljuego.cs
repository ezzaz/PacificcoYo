using UnityEngine;
using UnityEngine.SceneManagement;

public class Salirdeljuego : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene("Minijuego3");
    }
}
