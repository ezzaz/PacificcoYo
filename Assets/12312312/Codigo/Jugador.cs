using Unity.Mathematics;
using UnityEngine;

public class Jugador : MonoBehaviour
{
    public float velocidad = 5;
    public bool darVueltaControles;
    public Animator animator;

    private void Start()
    {
        Time.timeScale = 1f;
    }
    void Update()
    {
        float movimiento = Input.GetAxisRaw("Horizontal");
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        if (movimiento < 0 || movimiento > 0)
        {
            animator.SetBool("caminar", true);
        }
        else
        {
            animator.SetBool("caminar", false);
        }

        if (!darVueltaControles)
            rigidbody2D.linearVelocity = new Vector2(movimiento * velocidad, rigidbody2D.linearVelocity.y);
        else
            rigidbody2D.linearVelocity = new Vector2(movimiento * velocidad * -1, rigidbody2D.linearVelocity.y);

    }
}
