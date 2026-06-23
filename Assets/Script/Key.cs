using UnityEngine;

public class Key : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DoorManager.instance.CollectKey();
            Destroy(gameObject); // Desaparece la llave
        }
    }
}