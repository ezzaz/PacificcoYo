using UnityEngine;
using UnityEngine.SceneManagement;

public class ASiguienteNivel : MonoBehaviour
{
    public string haciaELNivel;
    public bool soyUtilizable;
    public GameObject[] objetosARecojer;
    public GameObject dialogo;
    public GameObject punteroInterqactivo;
    public float objetosRecogidos;
    public bool necesitaObjetos;

    private void Start()
    {
        soyUtilizable = false;
        punteroInterqactivo.SetActive(false);
    }

    private void Update()
    {
        if (necesitaObjetos)
        {
            if (objetosRecogidos >= objetosARecojer.Length)
            {
                dialogo.SetActive(false);
                if (soyUtilizable && Input.GetKeyDown(KeyCode.E))
                {
                    HaciaElNivel();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Jugador>(out Jugador jugador))
        {
            soyUtilizable = true;
            punteroInterqactivo.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Jugador>(out Jugador jugador))
        {
            soyUtilizable = false;
            punteroInterqactivo.SetActive(false);
        }
    }

    public void ObjetoMasMas()
    {
        objetosRecogidos++;
    }

    public void HaciaElNivel()
    {
        SceneManager.LoadScene(haciaELNivel);
    }
}
