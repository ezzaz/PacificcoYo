using UnityEngine;

public class MenuLateral : MonoBehaviour
{
    public RectTransform panelMenu;      // El panel que se mueve
    public float velocidadAnimacion = 5f; // Velocidad de la animación (ajústala según necesites)
    public Vector2 posAbierto;           // Posición del panel cuando está visible
    public Vector2 posCerrado;           // Posición fuera de pantalla
    private bool estaAbierto = false;    // Estado actual del panel
    private bool animando = false;       // Si está en medio de una animación
    private Vector2 posicionObjetivo;    // Posición hacia la que se mueve

    void Start()
    {
        // Inicialmente cerrado
        panelMenu.anchoredPosition = posCerrado;
        posicionObjetivo = posCerrado;
    }

    void Update()
    {
        if (animando)
        {
            // Interpolar suavemente hacia la posición objetivo
            panelMenu.anchoredPosition = Vector2.Lerp(
                panelMenu.anchoredPosition,
                posicionObjetivo,
                velocidadAnimacion * Time.deltaTime
            );

            // Detener la animación cuando esté muy cerca del objetivo
            if (Vector2.Distance(panelMenu.anchoredPosition, posicionObjetivo) < 0.1f)
            {
                panelMenu.anchoredPosition = posicionObjetivo;
                animando = false;
            }
        }
    }

    public void ToggleMenu()
    {
        if (estaAbierto)
        {
            // Cerrar panel
            posicionObjetivo = posCerrado;
            estaAbierto = false;
            animando = true;
        }
        else
        {
            // Abrir panel
            posicionObjetivo = posAbierto;
            estaAbierto = true;
            animando = true;
        }
    }
}