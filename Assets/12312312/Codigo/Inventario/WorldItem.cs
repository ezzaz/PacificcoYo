using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemData data;
    public bool soyUtilizable;
    public ASiguienteNivel nivel;
    public bool ObjetoImportante;
    public bool pallenarAlInicio, era;

    private void Update()
    {
        if (soyUtilizable && Input.GetKeyDown(KeyCode.E) && !pallenarAlInicio)
        {
            if (data != null)
            {
                if (DiaryManager.Instance != null)
                {
                    DiaryManager.Instance.AddPage(data);
                }
                else
                {
                    Debug.LogWarning("DiaryManager.Instance es null. Asegúrate de tener un DiaryManager en la escena.");
                }
            }
            else
            {
                Debug.LogWarning("WorldItem no tiene ItemData asignado.");
            }

            // Ocultar objeto recogido
            gameObject.SetActive(false);

            // Notificar al sistema de nivel (si aplica)
            if (ObjetoImportante)
                nivel.objetosRecogidos++;
        }
        if (era)
        {
            if (data != null)
            {
                if (DiaryManager.Instance != null)
                {
                    DiaryManager.Instance.AddPage(data);
                }
                else
                {
                    Debug.LogWarning("DiaryManager.Instance es null. Asegúrate de tener un DiaryManager en la escena.");
                }
            }
            else
            {
                Debug.LogWarning("WorldItem no tiene ItemData asignado.");
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Jugador>(out Jugador jugador))
        {
            soyUtilizable = true;
            // aquí puedes activar un prompt UI "Presiona E para recoger"
            if (pallenarAlInicio)
            {
                era = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Jugador>(out Jugador jugador))
        {
            soyUtilizable = false;
            // ocultar prompt UI
        }
    }
}