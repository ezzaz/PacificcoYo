using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Menú de pausa con libreta 3D (ps1_leather_notebook_open) como fondo.
/// - Desenfoca (blur) todo lo que NO sea la libreta usando un Volume con Depth of Field.
/// - La libreta entra deslizándose de ABAJO hacia ARRIBA.
/// - Botones Reanudar y Salir.
/// Todo es configurable desde el Inspector del prefab PauseMenu.
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController Instance;

    [Header("Referencias (asignadas dentro del prefab)")]
    [Tooltip("Objeto vacío que contiene el modelo 3D de la libreta.")]
    public Transform notebookRig;
    [Tooltip("Canvas (Screen Space Overlay) que contiene los botones.")]
    public Canvas pauseCanvas;
    [Tooltip("Contenedor de los botones que se desliza hacia arriba.")]
    public RectTransform buttonsContainer;
    [Tooltip("Volume global con el perfil de desenfoque (PauseBlurProfile).")]
    public Volume blurVolume;
    [Tooltip("AudioSource para reproducir el sonido al abrir el menú.")]
    public AudioSource pauseAudioSource;
    [Tooltip("Clip de sonido al abrir pausa.")]
    public AudioClip pauseOpenSound;
    [Header("Colocación de la libreta frente a la cámara")]
    [Tooltip("Distancia de la libreta respecto a la cámara (metros).")]
    public float notebookDistance = 0.5f;
    [Tooltip("Qué porción de la altura de pantalla ocupa la libreta (0-1).")]
    public float fillFraction = 0.85f;
    [Tooltip("Altura del modelo de la libreta en unidades locales (auto si es 0).")]
    public float notebookMeshHeight = 0.03f;
    [Tooltip("Ajuste de rotación para orientar la libreta hacia la cámara.")]
    public Vector3 notebookEulerOffset = new Vector3(90f, 0f, 0f);

    [Header("Animación de entrada (de abajo hacia arriba)")]
    [Tooltip("Duración del deslizamiento en segundos (tiempo real).")]
    public float slideDuration = 0.6f;
    [Tooltip("Posición Y inicial del contenedor de botones (abajo, fuera de pantalla).")]
    public float buttonsHiddenY = -1200f;
    [Tooltip("Posición Y final del contenedor de botones.")]
    public float buttonsShownY = 0f;

    private bool isPaused = false;
    private float slideT = 0f;              // 0 = abajo/oculto, 1 = arriba/visible
    private Camera targetCamera;
    private bool prevPostProcessing = false;
    private UniversalAdditionalCameraData camData;

    // Controles que se desactivan mientras el juego está en pausa
    private readonly List<MonoBehaviour> disabledControls = new List<MonoBehaviour>();

    private void Awake()
    {
        Instance = this;
        // Empezar oculto
        ApplyHiddenState();
    }

    private void ApplyHiddenState()
    {
        isPaused = false;
        slideT = 0f;
        if (pauseCanvas != null) pauseCanvas.enabled = false;
        if (notebookRig != null) notebookRig.gameObject.SetActive(false);
        if (blurVolume != null) blurVolume.enabled = false;
    }

    /// <summary> Alterna pausa / reanudar. Lo llama PausaBoton al pulsar Escape. </summary>
    public void Toggle()
    {
        if (isPaused) Resume();
        else Open();
    }

    public void Open()
    {
        if (isPaused) return;
        isPaused = true;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        targetCamera = Camera.main;
        if (targetCamera != null)
        {
            camData = targetCamera.GetComponent<UniversalAdditionalCameraData>();
            if (camData != null)
            {
                prevPostProcessing = camData.renderPostProcessing;
                camData.renderPostProcessing = true; // necesario para el blur
            }
        }
        if (pauseAudioSource != null && pauseOpenSound != null)
        {
            pauseAudioSource.PlayOneShot(pauseOpenSound);
        }

        if (blurVolume != null) blurVolume.enabled = true;
        if (pauseCanvas != null) pauseCanvas.enabled = true;
        if (notebookRig != null) notebookRig.gameObject.SetActive(true);

        DisablePlayerControls();

        StopAllCoroutines();
        StartCoroutine(SlideRoutine(true));
    }

    public void Resume()
    {
        if (!isPaused) return;
        isPaused = false;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (blurVolume != null) blurVolume.enabled = false;
        if (camData != null) camData.renderPostProcessing = prevPostProcessing;

        RestorePlayerControls();

        StopAllCoroutines();
        ApplyHiddenState();
    }

    public void Exit()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator SlideRoutine(bool opening)
    {
        float t = 0f;
        while (t < slideDuration)
        {
            t += Time.unscaledDeltaTime;   // funciona con timeScale = 0
            slideT = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / slideDuration));
            UpdateButtonsPosition();
            yield return null;
        }
        slideT = 1f;
        UpdateButtonsPosition();
    }

    private void UpdateButtonsPosition()
    {
        if (buttonsContainer != null)
        {
            float y = Mathf.Lerp(buttonsHiddenY, buttonsShownY, slideT);
            buttonsContainer.anchoredPosition = new Vector2(buttonsContainer.anchoredPosition.x, y);
        }
    }

    private void LateUpdate()
    {
        if (!isPaused || notebookRig == null) return;
        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera == null) return;

        // Escala automática para que ocupe fillFraction de la altura visible
        float meshH = Mathf.Max(0.0001f, notebookMeshHeight);
        float worldHeight = fillFraction * 2f * notebookDistance *
                            Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float scale = worldHeight / meshH;
        notebookRig.localScale = Vector3.one * scale;

        // Deslizamiento: empieza por debajo del encuadre y sube al centro
        float drop = worldHeight * 1.15f;
        float vertical = Mathf.Lerp(-drop, 0f, slideT);

        Vector3 basePos = targetCamera.transform.position
                          + targetCamera.transform.forward * notebookDistance
                          + targetCamera.transform.up * vertical;
        notebookRig.position = basePos;
        notebookRig.rotation = targetCamera.transform.rotation * Quaternion.Euler(notebookEulerOffset);
    }

    private void DisablePlayerControls()
    {
        disabledControls.Clear();
        CollectAndDisable(GameObject.Find("Player"));
        CollectAndDisable(GameObject.Find("Boat"));
    }

    private void CollectAndDisable(GameObject go)
    {
        if (go == null) return;
        foreach (var mb in go.GetComponents<MonoBehaviour>())
        {
            if (mb == null) continue;
            string n = mb.GetType().Name;
            if (n == "PlayerMovement" || n == "PlayerLook" || n == "PlayerPickUpDrop"
                || n == "WaterBoat" || n == "BoatCameraFollow" || n == "FishingMinigame")
            {
                if (mb.enabled) { mb.enabled = false; disabledControls.Add(mb); }
            }
        }
    }

    private void RestorePlayerControls()
    {
        foreach (var mb in disabledControls)
            if (mb != null) mb.enabled = true;
        disabledControls.Clear();
    }
}