using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Controla un objeto/marcador de selección animado para el menú de pausa.
/// Se mueve de forma suave (Lerp) hacia el botón sobre el que esté el ratón.
/// </summary>
public class PauseButtonSelector : MonoBehaviour
{
    [Header("Referencias del Marcador")]
    [Tooltip("El objeto que sirve como indicador (ej. pluma, brillo, marcador).")]
    public RectTransform indicator;

    [Header("Botones a rastrear")]
    public Button btnResume;
    public Button btnExit;

    [Header("Ajustes de Movimiento")]
    [Tooltip("Velocidad de deslizamiento entre opciones.")]
    public float moveSpeed = 12f;
    [Tooltip("Margen horizontal respecto al texto del botón (offset X).")]
    public float offsetX = -240f;
    [Tooltip("Margen vertical opcional (offset Y).")]
    public float offsetY = 0f;

    private Vector3 targetLocalPosition;
    private Vector3 baseScale;
    private Animator indicatorAnimator;
    private float breatheTimer = 0f;
    private bool isHoveringAny = false;

    private void Start()
    {
        if (indicator != null)
        {
            baseScale = indicator.localScale;
            indicatorAnimator = indicator.GetComponent<Animator>();

            // Empezar invisible o pequeño si no hay hover
            indicator.gameObject.SetActive(false);
        }

        // Registrar eventos de PointerEnter para mover el selector
        AddPointerEvents(btnResume);
        AddPointerEvents(btnExit);
    }

    private void AddPointerEvents(Button btn)
    {
        if (btn == null) return;

        EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = btn.gameObject.AddComponent<EventTrigger>();

        // Pointer Enter
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { OnButtonHover(btn); });
        trigger.triggers.Add(entryEnter);

        // Pointer Exit
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnButtonExit(btn); });
        trigger.triggers.Add(entryExit);
    }

    private void OnButtonHover(Button btn)
    {
        if (indicator == null || btn == null) return;

        isHoveringAny = true;
        indicator.gameObject.SetActive(true);

        // Calcular posición objetivo alineada a la izquierda del botón
        RectTransform btnRt = btn.GetComponent<RectTransform>();
        if (btnRt != null)
        {
            // El selector se coloca a la izquierda en X, y comparte la Y del botón
            targetLocalPosition = new Vector3(btnRt.anchoredPosition.x + offsetX, btnRt.anchoredPosition.y + offsetY, 0f);
        }

        // Si es el primer hover, saltar directamente a la posición para no deslizar desde lejos
        if (indicator.anchoredPosition.x == 0 && indicator.anchoredPosition.y == 0)
        {
            indicator.anchoredPosition = targetLocalPosition;
        }

        // Lanzar disparador del animador si existe
        if (indicatorAnimator != null)
        {
            indicatorAnimator.SetTrigger("OnHover");
            indicatorAnimator.SetBool("IsSelected", true);
        }
    }

    private void OnButtonExit(Button btn)
    {
        isHoveringAny = false;

        if (indicatorAnimator != null)
        {
            indicatorAnimator.SetBool("IsSelected", false);
        }
    }

    private void Update()
    {
        if (indicator == null) return;

        // 1. Deslizamiento suave (Lerp) hacia el botón activo
        if (isHoveringAny)
        {
            indicator.anchoredPosition = Vector3.Lerp(indicator.anchoredPosition, targetLocalPosition, Time.unscaledDeltaTime * moveSpeed);
        }

    }
}