using UnityEngine;
using UnityEngine.InputSystem;

public class DragAndDrop : MonoBehaviour
{
    public GameObject puntero;
    public bool clickeao;


    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        // Convertir la posición del mouse de coordenadas de pantalla a coordenadas del mundo
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f; // Asegurar que esté en el plano correcto
        puntero.transform.position = worldPos;

        if (Input.GetMouseButtonDown(0)) { clickeao = true; }
            else if (Input.GetMouseButtonUp(0)) { clickeao = false; }
    }
}
