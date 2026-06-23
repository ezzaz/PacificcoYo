using UnityEngine;

public class mostrarONO : MonoBehaviour
{
    public bool isPlayerInRange;
    public GameObject felcha;

    private void Start()
    {
        felcha.SetActive(false);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Jugador>(out Jugador jugador))
            felcha.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Jugador>(out Jugador jugador))
            felcha.SetActive(false);
    }
}
